using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Models
{
    public class ResponseModel
    {

        public Status Status { get; set; }

        public string ErrorMessage { get; set; }

        public int RecordCount { get; set; }

        public dynamic Data { get; set; }
    }

    public enum Status : byte 
    { 
        Success = 1,
        Failed = 0
    }

    public enum InviteStatus: byte 
    {
        Invited = 0,
        Accepted = 1,
        Rejected = 2,
        Requested = 3
    }

    public enum MediaType : byte
    {
        Picture = 0,
        Video = 1
    }

}