using KendoNET.DynamicLinq;
using System;
using System.Linq;

namespace Gestionador.Helpers
{
    public static class KendoHelpers
    {
        public static DataSourceRequest SetDateTimeToDateFilters(DataSourceRequest request)
        {
            try
            {
                if (request.Filter != null)
                {
                    int i = 0;
                    foreach (var item in request.Filter.Filters)
                    {
                        if (item.Value != null)
                        {
                            if (item.Value.GetType().Equals(typeof(DateTime)))
                                request.Filter.Filters.ElementAt(i).Value = Convert.ToDateTime(item.Value).Date;
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return request;
        }
    }
}