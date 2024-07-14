using BSOL.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BSOL.Helpers
{
    public static class Extensions
    {
        #region Enum extensions

        public static string Description(this Enum Value)
        {
            FieldInfo field = Value.GetType().GetField(Value.ToString());

            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? Value.ToString() : attribute.Description;
        }

        public static int ToInt(this Enum Value)
        {
            return Convert.ToInt32(Value);
        }

        public static DateTime ToMonthStart(this DateTime Value)
        {
            return new DateTime(Value.Year, Value.Month, 1);
        }
        #endregion

        #region String extenstions

        public static bool IsValid(this string Value)
        {
            return !string.IsNullOrWhiteSpace(Value);
        }

        public static long ToLong(this string Value)
        {
            long _Value;
            long.TryParse(Value, out _Value);
            return _Value;
        }
        public static object Get(this Dictionary<string, object> value, string key)
        {
            return value.Where(x => x.Key == key).Select(x => x.Value).FirstOrDefault();
        }
        #endregion
    }
}
