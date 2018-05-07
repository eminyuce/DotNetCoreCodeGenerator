using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetCodeGenerator.Domain.Helpers
{
    public class GeneralHelper
    {
      
        public static string ConvertTypeToSQL(string s)
        {
            switch (s)
            {
                case "Guid":
                    return "UNIQUEIDENTIFIER";
                case "Int":
                    return "INT";
                case "int":
                    return "INT";
                case "Bool":
                    return "BIT";
                case "bool":
                    return "BIT";
                case "Boolean":
                    return "BIT";
                case "boolean":
                    return "BIT";
                case "DateTime":
                    return "DATETIME";
                case "String":
                    return "NVARCHAR(MAX)";
                case "string":
                    return "NVARCHAR(MAX)";
                case "Float":
                    return "DECIMAL(20,6)";
                case "Double":
                    return "DECIMAL(20,6)";
                case "Decimal":
                    return "DECIMAL(18,2)";
                case "float":
                    return "DECIMAL(20,6)";
                case "double":
                    return "DECIMAL(20,6)";
                case "decimal":
                    return "DECIMAL(18,2)";
                default:
                    return "";
            }
        }

        public static string FormatXml(string xml)
        {
            try
            {
                XDocument doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception)
            {
                return xml;
            }
        }

        public static String getObject(String name)
        {
            return name.Replace("id", "").Replace("Id", "");
        }
        public static string FirstCharacterToLower(string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
                return str;

            return Char.ToLowerInvariant(str[0]).ToString() + str.Substring(1);
        }

        public static readonly Regex CarriageRegex = new Regex(@"(\r\n|\r|\n)+");
        //remove carriage returns from the header name
        public static string RemoveCarriage(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return "";
            }
            return CarriageRegex.Replace(text, string.Empty).Trim();
        }
        public static string GetUrlString(string strIn)
        {
            // Replace invalid characters with empty strings. 
            strIn = strIn.ToLower();
            strIn = RemoveCarriage(strIn);
            char[] szArr = strIn.ToCharArray();
            var list = new List<char>();
            foreach (char c in szArr)
            {
                int ci = c;
                if ((ci >= 'a' && ci <= 'z') || (ci >= '0' && ci <= '9') || ci <= ' ')
                {
                    list.Add(c);
                }
            }
            return new String(list.ToArray()).Replace(" ", "_");
        }
        public static string ToTitleCase(string s)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
        }
        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
        public static string GetCleanEntityName(string m)
        {
            if (!String.IsNullOrEmpty(m))
            {
                var parts = m.Split(new string[] { "_" }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    m = "Nwm" + UppercaseFirst(parts[0]) + "" + UppercaseFirst(parts[1].TrimEnd('s'));
                }
                else
                {
                    m = parts[0];
                }
            }
            return m;
        }
        public static string GetEntityPrefixName(string m)
        {
            String k = "";
            if (!String.IsNullOrEmpty(m) && m.Contains("_"))
            {
                var parts = m.Split(new string[] { "_" }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    k = parts[0].Trim();
                }
            }
            return k;
        }
    }
}
