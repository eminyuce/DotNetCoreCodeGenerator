using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

// https://cloud.tencent.com/developer/article/1030316
namespace DotNetCodeGenerator.Domain.Helpers
{

    //Limitations
    //It doesn’t work with, has relation instances
    //Only when the header and type of a DataTables column matches the name and type of a property of a user defined type, DataTable to List conversion takes place.
    //Works fine with plain objects like:
    //public class Student
    //{
    //    public long Id { get; set; }
    //    public string Name { get; set; }
    //    public short Age { get; set; }
    //    public DateTime DateOfCreation { get; set; }
    //    public bool? IsActive { get; set; }
    //}
    // https://www.codeproject.com/Tips/784090/Conversion-Between-DataTable-and-List-in-Csharp
    public static class DataConverter
    {

        public static List<TSource> ToList<TSource>(this DataTable dataTable) where TSource : new()
        {
            var dataList = new List<TSource>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            var objFieldNames = (from PropertyInfo aProp in typeof(TSource).GetProperties(flags)
                                 select new
                                 {
                                     Name = aProp.Name,
                                     Type = Nullable.GetUnderlyingType(aProp.PropertyType) ??
                         aProp.PropertyType
                                 }).ToList();
            var dataTblFieldNames = (from DataColumn aHeader in dataTable.Columns
                                     select new
                                     {
                                         Name = aHeader.ColumnName,
                                         Type = aHeader.DataType
                                     }).ToList();
            var commonFields = objFieldNames.Intersect(dataTblFieldNames).ToList();
            using (DataTable dt = dataTable)
            {
                foreach (DataRow dataRow in dt.Rows)
                {
            //foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            //{
                var aTSource = new TSource();
                foreach (var aField in commonFields)
                {
                    PropertyInfo propertyInfos = aTSource.GetType().GetProperty(aField.Name);
                    var value = (dataRow[aField.Name] == DBNull.Value) ?
                    null : dataRow[aField.Name]; //if database field is nullable
                    propertyInfos.SetValue(aTSource, value, null);
                }
                dataList.Add(aTSource);
            }
            }
            return dataList;
        }
    }
}
