using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUDExample.Controllers;

[Route("[controller]")]
public class CountriesController: Controller
{
    private readonly ICountriesService _countriesService;
    
    public CountriesController(ICountriesService countriesService)
    {
        _countriesService = countriesService;
    }
    
    [Route("UploadFromExcel")]
    public IActionResult UploadFromExcel()
    {


        return View();
    }

    [HttpGet]
    [Route("GetAllCountries")]
    public async Task<IActionResult> GetAllCountries()
    {
        List<CountryResponse> countries =await _countriesService.GetAllCountries();
        
        return Json(countries);
    }

    [HttpPost]
    [Route("UploadFromExcel")]
    public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
    {
        if (excelFile == null || excelFile.Length == 0)
        {
            ViewBag.ErrorMessage = "Please select a file to upload";
            return View();
        }

        if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx",StringComparison.OrdinalIgnoreCase))
        {
            ViewBag.ErrorMessage = "Please select an xlsx file to upload";
            return View();
        }
        
        int countriesInserted =   await  _countriesService.UploadCountryFromExcelFile(excelFile);
        
        ViewBag.Message = $"{countriesInserted} countries inserted successfully";

        return View();


    }
    
    
}