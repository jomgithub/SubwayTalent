using SubwayTalent.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Models
{
    public class SearchFilterModel
    {
        public List<LookUpValues> Skills { get; set; }
        public List<LookUpValues> Genres { get; set; }
        public List<LookUpValues> EventCategories { get; set; }
    }
}