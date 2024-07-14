using BSOL.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace BSOL.Core.Helpers
{
    public class IdentityDecryptor : IModelBinder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IdentityDecryptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var value = valueProviderResult.FirstValue; // get the value as string

            long tempId;
            if (long.TryParse(value, out tempId) && tempId != 0 && bindingContext.FieldName.ToLower() != "referenceid")
                throw new UnauthorizedAccessException();

            if (value.IsValid())
            {
                try
                {
                    bindingContext.Result = ModelBindingResult.Success(Convert.ChangeType(DecryptParam(value), Nullable.GetUnderlyingType(bindingContext.ModelType) ?? bindingContext.ModelType));
                }
                catch
                {
                    throw new UnauthorizedAccessException();
                }
            }

            return Task.CompletedTask;
        }

        public static string DecryptParam(object paramValue, long? employeeId = null)
        {
            paramValue = paramValue != null && paramValue.ToString().Length > 1 ? paramValue.ToString().Substring(1, paramValue.ToString().Length - 1) : "";
            return Utilities.Decrypt(Convert.ToString(paramValue), (employeeId.HasValue ? employeeId.Value.ToString() : AppUser.Current.User.FindFirstValue(ClaimType.EmployeeId.ToString())) + Constants.CRYPTO_KEY_FOR_ID, true);
        }
    }
}
