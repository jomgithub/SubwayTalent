using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Contracts
{
    public class LookUpValues 
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class States
    {
        public string Id { get; set; }
        public string Name { get; set; }

    }

    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Latitude{ get; set; }
        public string Longitude { get; set; }
    }
}
