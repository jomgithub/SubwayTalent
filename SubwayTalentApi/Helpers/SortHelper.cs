using SubwayTalentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Helpers
{
    public static class SortHelper
    {

        public static List<T> Sort<T>(Sort sort, List<T> unOrderedList) 
        {
            if (unOrderedList != null)
            {
                var propInfo = typeof(T).GetProperty(sort.Key, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (propInfo == null)
                    throw new Exception(string.Format("sort category specified is not applicable for this search.  [{0}]", sort.Key));

                //Default order is ascending
                if (string.IsNullOrWhiteSpace(sort.Direction) || sort.Direction.ToUpper() == "ASC")
                    return unOrderedList.OrderBy(x => propInfo.GetValue(x, null)).ToList();

                if (sort.Direction.ToUpper() != "ASC" && sort.Direction.ToUpper() != "DES")
                    throw new Exception("Invalid sort direction.");

                if (sort.Direction.ToUpper() == "DES")
                    return unOrderedList.OrderByDescending(x => propInfo.GetValue(x, null)).ToList();
            }

            return unOrderedList;
        }
    }
}