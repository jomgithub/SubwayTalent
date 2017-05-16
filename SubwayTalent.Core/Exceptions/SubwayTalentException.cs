using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Core.Exceptions
{
    public class SubwayTalentException : Exception
    {
        public SubwayTalentException() : base() { }
        public SubwayTalentException(string message) : base(message) { }
        public SubwayTalentException(string message, Exception innerEx) : base(message, innerEx) { }  
    }
}
