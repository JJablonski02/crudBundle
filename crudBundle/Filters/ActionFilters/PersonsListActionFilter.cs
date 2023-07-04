using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace crudBundle.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;
        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context) // Secondly
        {
            //Here the method can:
            //manipulate the ViewData
            //change the result returned from the action method
            //throw exceptions to either return the exception to the exception filter(if exists) or return the error response to the browser :)
            _logger.LogInformation("PersonsListActionFilter.PersonOnActionExecuted");

            
        }

        public void OnActionExecuting(ActionExecutingContext context) // Firstly
        {
            //Here the method can:
            //Validate action method parameteres, short-circuit the action(prevent action from execution) and return a different IActionResult,
            //access the action method parameters, read them and do necessary manipulations on them
            //To do: add before logic here
            _logger.LogInformation("PersonsListActionFilter.PersonOnActionExecuting");
            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                //validate the searchBy parametere value
                if(searchBy is not null)
                {
                    var searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryID),
                        nameof(PersonResponse.Address),
                    };

                    //resettuing the searchBy parameter value
                    if (searchByOptions.Any(temp => temp == searchBy) == false)
                    {
                        _logger.LogInformation("searchBy actual value {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("searchBy updated value {searchBy}", context.ActionArguments["searchBy"]);
                    }

                }
            }

            

        }
    }
}
