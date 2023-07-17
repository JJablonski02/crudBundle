using Microsoft.AspNetCore.Mvc.Filters;

namespace crudBundle.Filters.ResultFilters
{
    public class TokenResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Cookies.Append("Auth_Key", "A100");
        }
    }
}
