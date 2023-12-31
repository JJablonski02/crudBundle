﻿using crudBundle.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace crudBundle.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : ActionFilterAttribute
    {
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonCreateAndEditPostActionFilter> _logger;
        public PersonCreateAndEditPostActionFilter(ICountriesService countriesService, ILogger<PersonCreateAndEditPostActionFilter> logger)
        {
            _countriesService = countriesService;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before logic
            if (context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countriesService.GetAllCountries();
                    personsController.ViewBag.Countries = countries.Select(temp =>
                    new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

                    var personRequest = context.ActionArguments["personRequest"];
                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                    context.Result = personsController.View(personRequest); //short-aircircuits or skips the subsequent action filters & action method
                }
                else
                {
                    await next(); // calls the subseqent filter or action method
                }
            }
            else
            {
                await next();
            }

            //Before logic
            _logger.LogInformation("In after logic of PersonCreateAndEditActionFilter");
        }
    }
}
