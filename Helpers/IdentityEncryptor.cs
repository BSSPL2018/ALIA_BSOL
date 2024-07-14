
using BSOL.Helpers;
using Newtonsoft.Json;
using System;
using System.Security.Claims;

namespace BSOL.Core.Helpers
{
    public class IdentityEncryptor : JsonConverter
    {
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = Convert.ToString(reader.Value);
            if (value.IsValid())
                return Convert.ChangeType(IdentityDecryptor.DecryptParam(value), Nullable.GetUnderlyingType(objectType) ?? objectType);

            return Activator.CreateInstance(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(EncryptParam(value));
        }

        public static string EncryptParam(object paramValue, long? employeeId = null)
        {
            return Convert.ToString(paramValue) == "0" ? null : ("P" + Utilities.Encrypt(Convert.ToString(paramValue), (employeeId.HasValue ? employeeId.Value.ToString() : AppUser.Current.User.FindFirstValue(ClaimType.EmployeeId.ToString())) + Constants.CRYPTO_KEY_FOR_ID, true));
        }
    }
}
