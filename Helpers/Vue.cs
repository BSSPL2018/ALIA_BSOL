using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Helpers
{
    public class Vue
    {
        #region Static Methods

        public static HtmlString Encode(object Value)
        {
            //return new MvcHtmlString(JsonConvert.SerializeObject(Value, new JavaScriptDateTimeConverter()));
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new JavaScriptDateTimeConverter());
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
            return new HtmlString(JsonConvert.SerializeObject(Value, settings));
        }

        #endregion
    }
}
