using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelpersProject
{
    public static  class EnumHelper
    {
        public static Nullable<T> Parse<T>(String value, Boolean ignoreCase = true) where T : struct
        {
            return String.IsNullOrEmpty(value) ? null : (Nullable<T>)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static IEnumerable<T> GetValues<T>()
        {
            return System.Enum.GetValues(typeof(T)).Cast<T>();
        }
        public static IEnumerable<SelectListItem> ToSelectList(this System.Enum enumValue)
        {
            return from System.Enum e in System.Enum.GetValues(enumValue.GetType())
                   select new SelectListItem
                   {
                       Selected = e.Equals(enumValue),
                       Text = e.ToDescription(),
                       Value = e.ToString()
                   };
        }

        public static string ToDescription(this System.Enum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
        public static string ToDisplayName(this System.Enum value)
        {
            var attributes = (DisplayAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DisplayAttribute), false);
            return attributes.Length > 0 ? attributes[0].Name : value.ToString();
        }
        public static IEnumerable<SelectListItem> ToSelectListWithId(this System.Enum enumValue)
        {
            return from System.Enum e in System.Enum.GetValues(enumValue.GetType())
                   select new SelectListItem
                   {
                       Selected = e.Equals(enumValue),
                       Text = e.ToDescription(),
                        // Value = Convert.ToInt32(e).ToStr()
                        Value = e.ToStr()
                   };
        }
        public static T ParseEnum<T>(string value, T defaultValue) where T : struct
        {
            try
            {
                T enumValue;
                if (!System.Enum.TryParse(value, true, out enumValue))
                {
                    return defaultValue;
                }
                return enumValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
