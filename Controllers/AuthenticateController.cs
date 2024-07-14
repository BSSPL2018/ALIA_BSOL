using BSOL.Core;
using BSOL.Core.Models.Common;
using BSOL.Helpers;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BSOL.Controllers
{
    public class AuthenticateController : BaseController
    {
        private readonly IConfiguration _config;
        public AuthenticateController(BSOLContext context, BSOLWebContext Webcontext, AppUser appUser, IConfiguration configuration) : base(context, Webcontext, appUser)
        {
            _config = configuration;
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromForm] string userName, [FromForm] string password, [FromForm] string? grant_type = "")
        {
            int temp;
            User user = new User();

            if (userName.StartsWith("A") && userName.Length == 7 && int.TryParse(userName.Replace('A', ' '), out temp))
            {
                user = await _context.ExecuteSingleAsync<User>("spBSOL_LoginAuthentication", new { Optn = "POS_LOGIN_GUEST", uid = userName });
                if (Utilities.Encrypt(password, user.EmpID + Constants.CRYPTO_KEY) != user.Password)
                    return new JsonResult(new { HasError = true, Message = "Invalid credentials." });
            }
            else
            {
                user = await _Webcontext.ExecuteSingleAsync<User>("spGeneral_Authenticate", new { Option = "AUTHENTICATE", UserName = userName });
                if (Utilities.Encrypt(password, user.EmployeeID + Constants.CRYPTO_KEY) != user.Password)
                    return new JsonResult(new { HasError = true, Message = "Invalid credentials." });
            }

            if (user == null)
                return new JsonResult(new { HasError = true, Message = "Invalid credentials." });



            string[] rights = new List<string>().ToArray();
            if (!user.IsPowerUser)
                rights = (await _context.ExecuteSpAsync<StringTypeModel>("spADMIN_UserRightsView", new { user.UserID, user.IsGuest })).Select(x => x.Value).ToArray();

            ClaimsIdentity claims = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.Name, user.EmpID.ToString()),
                    new Claim(ClaimType.ShortName.ToString(), user.ShortName+" - "+user.EmpID),
                    new Claim(ClaimType.FullName.ToString(), user.FullName),
                    new Claim(ClaimType.IsPowerUser.ToString(), user.IsPowerUser.ToString()),
                    new Claim(ClaimType.Rights.ToString(), rights.Any() ? string.Join(",", rights) : ""),
                    new Claim(ClaimType.UserID.ToString(), user.UserID.ToString()),
                    new Claim(ClaimType.CompanyId.ToString(), user.CompanyId.ToString()),
                    new Claim(ClaimType.PrimaryCurrency.ToString(), user.PrimaryCurrency.ToString()),
                    new Claim(ClaimType.ConversionRate.ToString(), user.ConversionRate.ToString()),
                    new Claim(ClaimType.UniqueID.ToString(), user.UniqueID.ToString()),
                    new Claim(ClaimType.IsGuest.ToString(), user.IsGuest.ToString()),
                    new Claim(ClaimType.EmployeeId.ToString(), user.EmployeeId.ToString()),
                    new Claim(ClaimType.ShopId.ToString(), (user.ShopId ?? 0).ToString()),
                    new Claim(ClaimType.ShopName.ToString(), user.ShopName ?? ""),
                    new Claim(ClaimType.ShopGST.ToString(), user.ShopGST.ToString()),
                    new Claim(ClaimType.ShopGSTInAccountId.ToString(), user.ShopGSTInAccountId.ToString()),
                    new Claim(ClaimType.ShopGSTOutAccountId.ToString(), user.ShopGSTOutAccountId.ToString()),
                    new Claim(ClaimType.ShopCount.ToString(), user.ShopCount.ToString()),
                    new Claim(ClaimType.ShopCode.ToString(), user.ShopCode ?? ""),
                    new Claim(ClaimType.ServiceCharge.ToString(), user.ServiceCharge.ToString()),
                    new Claim(ClaimType.EntityID.ToString(), user.EntityID.ToString()),
                    new Claim(ClaimType.BaseCurrency.ToString(), user.BaseCurrency.ToString())
            }, "login");

            if (grant_type == "cookie")
            {
                ClaimsPrincipal princ = new ClaimsPrincipal(claims);
                await HttpContext.SignInAsync(princ);

                string url = "";



                //if (user.IsPowerUser)
                //{
                //    Response.Cookies.Append("MAIN_MENU", "CRM Suite");
                //    url = Url.Page("/CRM/Customer");
                //}

                //else if (rights.Any(x => x == Forms.CRM.Customer))
                //{
                //    Response.Cookies.Append("MAIN_MENU", "CRM Suite");
                //    url = Url.Page("/CRM/Customer");
                //}

                //else if (rights.Any(x => x == Forms.Accounts.Remittance))
                //{
                //    Response.Cookies.Append("MAIN_MENU", "Finance");
                //    url = Url.Page("/Accounts/Remittance");
                //}

                //else if (rights.Any(x => x == Forms.Logistics.LorryPayment))
                //{
                //    url = Url.Page("/Logistics/LorryPayment");
                //}
                //else

                if (rights.Any(x => x == Forms.Accounts.Remittance))
                {
                    Response.Cookies.Append("MAIN_MENU", "Retail");
                    url = Url.Page("/Accounts/Remittance");
                }

                else if (rights.Any(x => x == Forms.Procurement.Items))
                {
                    Response.Cookies.Append("MAIN_MENU", "Material Management");
                    url = Url.Page("/Procurements/Items");
                }
                else
                {
                    Response.Cookies.Append("MAIN_MENU", "CRM Suite");
                    url = Url.Page("/CRM/Customer");
                }

                return new JsonResult(new { HasError = false, Message = url });
            }
            else
            {
                var expires = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("JWTAuth:Expiry"));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = expires,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.GetValue<string>("JWTAuth:Secret"))), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                string jwt = tokenHandler.WriteToken(token);

                return new JsonResult(new
                {
                    AccessToken = jwt,
                    ExpiresIn = expires.Ticks
                });
            }
        }

        [HttpGet]
        public async Task<RedirectToPageResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToPage("/Index");
        }

        public async Task<string> SubscribeBrowser(string id)
        {
            var user = await _Webcontext.Users.FindAsync(_appUser.UserID);
            user.BSOLBrowserID = id;
            await _context.SaveChangesAsync();

            return "Subscribed";
        }

        public async Task<string> UnSubscribeBrowser()
        {
            var user = await _Webcontext.Users.FindAsync(_appUser.UserID);
            user.BSOLBrowserID = null;
            await _context.SaveChangesAsync();

            return "UnSubscribed";
        }
    }
}
