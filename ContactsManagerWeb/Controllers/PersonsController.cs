using ContactsManagerWeb.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
  
   
  
  [Route("[controller]")]
  [TypeFilter(typeof(ResponseHeaderActionFilter),Arguments = new object[]
  {
    "controller-key", "controller-value",3
  }, Order = 3)]
  public class PersonsController : Controller
  {
    //private fields
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly ILogger<PersonsController> _logger;

    //constructor
    public PersonsController(IPersonsService personsService, ICountriesService countriesService,ILogger<PersonsController> logger)
    {
      _personsService = personsService;
      _countriesService = countriesService;
      _logger = logger;
    }
    

    //Url: persons/index
    [Route("[action]")]
    [Route("/")]
    [TypeFilter(typeof(PersonsListActionFilter))] //This filter will be executed before the action method
    [TypeFilter(typeof(ResponseHeaderActionFilter),Arguments = new object[]
    {
      "X-Custom-Header", "Custom Header Value",1
    }, Order = 1)]
    public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
      _logger.LogInformation("Index action method of PersonsController called");
      
      _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");
      
      
      //Search
      ViewBag.SearchFields = new Dictionary<string, string>()
      {
        { nameof(PersonResponse.PersonName), "Person Name" },
        { nameof(PersonResponse.Email), "Email" },
        { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
        { nameof(PersonResponse.Gender), "Gender" },
        { nameof(PersonResponse.CountryID), "Country" },
        { nameof(PersonResponse.Address), "Address" }
      };
      List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);
      // ViewBag.CurrentSearchBy = searchBy;
      // ViewBag.CurrentSearchString = searchString;

      //Sort
      List<PersonResponse> sortedPersons =await  _personsService.GetSortedPersons(persons, sortBy, sortOrder);
      // ViewBag.CurrentSortBy = sortBy;
      // ViewBag.CurrentSortOrder = sortOrder.ToString();

      return View(sortedPersons); //Views/Persons/Index.cshtml
    }
    
    //API endpoint: returns all persons as JSON
    //Url: /persons/getall
    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetAll()
    {
      List<PersonResponse> persons = await _personsService.GetAllPersons();
      return Json(persons);
    }



    //Executes when the user clicks on "Create Person" hyperlink (while opening the create view)
    //Url: persons/create
    [Route("[action]")]
    [HttpGet]
    [TypeFilter(typeof(ResponseHeaderActionFilter),Arguments = new object[]
    {
      "my-key", "my-value",4
    })]
    public async Task<IActionResult> Create()
    {
      List<CountryResponse> countries =await _countriesService.GetAllCountries();
      ViewBag.Countries = countries.Select(temp => 
        new SelectListItem() {  Text = temp.CountryName, Value = temp.CountryID.ToString() }
      );

      //new SelectListItem() { Text="Harsha", Value="1" }
      //<option value="1">Harsha</option>
      return View();
    }

    [HttpPost]
    //Url: persons/create
    [Route("[action]")]
    public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
    {
      if (!ModelState.IsValid)
      {
        List<CountryResponse> countries =await _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(temp =>
        new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

        ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        return View(personAddRequest);
      }

      //call the service method
      PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
      
      //navigate to Index() action method (it makes another get request to "persons/index"
      return RedirectToAction("Index", "Persons");
    }

    [HttpGet]
    [Route("[action]/{personID}")] //Eg: /persons/edit/1
    public async Task<IActionResult> Edit(Guid personID)
    {
      PersonResponse? personResponse =await _personsService.GetPersonByPersonID(personID);
      if (personResponse == null)
      {
        return RedirectToAction("Index");
      }

      PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

      List<CountryResponse> countries =await _countriesService.GetAllCountries();
      ViewBag.Countries = countries.Select(temp =>
      new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

      return View(personUpdateRequest);
    }


    [HttpPost]
    [Route("[action]/{personID}")]
    public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
    {
      PersonResponse? personResponse =await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

      if (personResponse == null)
      {
        return RedirectToAction("Index");
      }

      if (ModelState.IsValid)
      {
        PersonResponse updatedPerson =await _personsService.UpdatePerson(personUpdateRequest);
        return RedirectToAction("Index");
      }
      else
      {
        List<CountryResponse> countries =await _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(temp =>
        new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

        ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        return View(personResponse.ToPersonUpdateRequest());
      }
    }


    [HttpGet]
    [Route("[action]/{personID}")]
    public async Task<IActionResult> Delete(Guid? personID)
    {
      PersonResponse? personResponse =await _personsService.GetPersonByPersonID(personID);
      
      _logger.LogInformation($"(GUID)Delete action method of PersonsController called for personID: {personID}");
      _logger.LogInformation($"personResponse: {personResponse}");
      if (personResponse == null)
        return RedirectToAction("Index");

      bool deleted = await _personsService.DeletePerson(personID);
      if (!deleted)
      {
        _logger.LogError($"Failed to delete person with personID: {personID}");
      }
      return View(personResponse);
    }

    [HttpPost]
    [Route("[action]/{personID}")]
    public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateResult)
    {
      PersonResponse? personResponse =await _personsService.GetPersonByPersonID(personUpdateResult.PersonID);
      _logger.LogInformation($"(GUID)Delete action method of PersonsController called for personID: {personUpdateResult.PersonID}");
      _logger.LogInformation($"personResponse: {personResponse}");
      if (personResponse == null)
        return RedirectToAction("Index");

      bool deleted = await _personsService.DeletePerson(personUpdateResult.PersonID);
      if (!deleted)
      {
        _logger.LogError($"Failed to delete person with personID: {personUpdateResult.PersonID}");
      }
      return RedirectToAction("Index");
    }

    [Route("PersonsPDF")]
    public async Task<IActionResult> PersonsPDF()
    {
      //get list of persons
      List<PersonResponse> persons =await _personsService.GetAllPersons();
      
      
      
      return new ViewAsPdf("PersonsPDF",persons, ViewData)
      {
        PageMargins = new Margins()
        {
          Bottom = 20,
          Top = 20,
          Right = 20,
          Left = 20
        },
        PageOrientation = Orientation.Landscape
      };
      
    }

    [Route("PersonsCSV")]
    public async Task<IActionResult> PersonsCSV()
    {
      MemoryStream personsCsv =  await _personsService.GerPersonsCSV();
      
      return File(personsCsv, "application/octet-stream", "Persons.csv");
    }
    
    [Route("PersonsExcel")]
    public async Task<IActionResult> PersonsExcel()
    {
      MemoryStream personsCsv =  await _personsService.GerPersonsExcel();
      
      return File(personsCsv, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Persons.xlsx");
    }
  }
}
