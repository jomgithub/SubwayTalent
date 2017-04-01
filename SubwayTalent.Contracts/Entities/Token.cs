

using System;
using System.Collections.Generic;

namespace SubwayTalent.Contracts
{ 
    
    public class Token
    {
        public int TokenId { get; set; }
        public string  UserId { get; set; }
        public string AuthToken { get; set; }
        public DateTime IssuedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
    
        public virtual UserAccount User { get; set; }
    }
}
