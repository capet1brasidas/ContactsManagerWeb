using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace ContactsManagerWeb.Filters.ActionFilters;

public class PersonsListActionFilter : IActionFilter
{
    private readonly ILogger<PersonsListActionFilter> _logger;
    
    
    public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
    {
        _logger = logger;
    }
    
    public void OnActionExecuting(ActionExecutingContext context)
    {
        
        _logger.LogInformation("{FilterName}.{MethodName} method",nameof(PersonsListActionFilter), nameof(OnActionExecuting));


        
        context.HttpContext.Items["arguments"] = context.ActionArguments;

        if (context.ActionArguments.ContainsKey("searchBy"))
        {
            string? searchBy = context.ActionArguments["searchBy"] as string;

            //validate search by parameter value
            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchByOptions = new List<string>()
                {
                    nameof(PersonResponse.PersonName),
                    nameof(PersonResponse.Email),
                    nameof(PersonResponse.DateOfBirth),
                    nameof(PersonResponse.Gender),
                    nameof(PersonResponse.CountryID),
                    nameof(PersonResponse.Address),
                    nameof(PersonResponse.ReceiveNewsLetters)
                };
                
                //reset searchby parameter value
                if(searchByOptions.Any(temp => temp == searchBy))
                {
                    _logger.LogInformation("searchBy actual value {searchBy}", searchBy);
                    
                    //replace certain parameter
                    context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                    
                    _logger.LogInformation("searchBy replaced with {searchBy}", nameof(PersonResponse.PersonName));
                }
            }
            
            
            
        }
        
        
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("{FilterName}.{MethodName} method",nameof(PersonsListActionFilter), nameof(OnActionExecuted));


        PersonsController personsController = context.Controller as PersonsController;

        IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

        if (parameters != null)
        {
            if (parameters.ContainsKey("searchBy"))
            {
                personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]);
            }
            
            if (parameters.ContainsKey("searchString"))
            {
                personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
            }
            
            if (parameters.ContainsKey("sortBy"))
            {
                personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
            }
            
            if (parameters.ContainsKey("sortOrder"))
            {
                personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
            }
            
        }
        
        
     
        
        
        

    }
}