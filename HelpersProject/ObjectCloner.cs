using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace HelpersProject
{
    public static class ObjectCloner
    {
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
        public static T Clone<T>(object obj, bool deep = false) where T : new()
        {
            if (!(obj is T))
            {
                throw new Exception("Cloning object must match output type");
            }

            return (T)Clone(obj, deep);
        }

        public static object Clone(object obj, bool deep)
        {
            if (obj == null)
            {
                return null;
            }

            Type objType = obj.GetType();

            if (objType.IsPrimitive || objType == typeof(string) || objType.GetConstructors().FirstOrDefault(x => x.GetParameters().Length == 0) == null)
            {
                return obj;
            }

            List<PropertyInfo> properties = objType.GetProperties().ToList();
            if (deep)
            {
                properties.AddRange(objType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic));
            }

            object newObj = Activator.CreateInstance(objType);

            foreach (var prop in properties)
            {
                if (prop.GetSetMethod() != null)
                {
                    object propValue = prop.GetValue(obj, null);
                    object clone = Clone(propValue, deep);
                    prop.SetValue(newObj, clone, null);
                }
            }

            return newObj;
        }
    }
}
