using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace BSOL.Helpers
{
    public class ValidateActionAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public string FormName { get; set; }
        public Rights Event { get; set; }

        public ValidateActionAttribute(string formName, Rights events = Rights.View)
        {
            FormName = formName;
            Event = events;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            AppUser appUser = (AppUser)context.HttpContext.RequestServices.GetService(typeof(AppUser));

            if (!FormName.IsValid() || appUser.IsPowerUser)
                return;

            if (!await appUser.HasAccess(FormName, Event))
            {
                context.Result = new ContentResult() { Content = "Access Denied", ContentType = MediaTypeNames.Text.Plain, StatusCode = (int)HttpStatusCode.Forbidden };
                return;
            }
        }
    }
}
