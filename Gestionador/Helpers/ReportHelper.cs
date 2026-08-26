using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace Gestionador.Helpers
{
    public class ReportHelper
    {
        public static (DateTime, DateTime) ConvertToDateUtc(DateTime startDate, DateTime endDate)
        {
            var convertStartDate = startDate.Date.ToUniversalTime();

            DateTime convertEndDate;

            if (startDate.Date == endDate.Date)
            {
                convertEndDate = startDate.Date.ToUniversalTime().AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            else
            {
                convertEndDate = endDate.Date.ToUniversalTime().AddDays(1).AddTicks(-1);
            }
            return (convertStartDate, convertEndDate);
        }

        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}