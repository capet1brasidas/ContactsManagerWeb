using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ServiceContracts;

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