using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BSOL.Pages.Shared
{
    [AllowAnonymous]
    public class ErrorModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
