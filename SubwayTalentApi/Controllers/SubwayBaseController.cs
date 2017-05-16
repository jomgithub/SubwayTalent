using SubwayTalent.Contracts;
using SubwayTalentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SubwayTalentApi.Controllers
{
    public class SubwayBaseController : ApiController
    {

        public ResponseModel ResponseModel { get; set; }

       
    }
}
