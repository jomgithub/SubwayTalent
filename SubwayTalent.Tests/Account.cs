using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubwayTalent.Data;
using System.Collections.Generic;
using System.IO;
using SubwayTalentApi.Helpers;
using SubwayTalentApi.Models;
using System.Device.Location;
using SubwayTalent.Contracts;
using System.Linq;
using SubwayTalent.Core;
using SubwayTalent.Core.Interfaces;
using System.Configuration;
using System.Text;

namespace SubwayTalent.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            try
            {

                //var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
               // ffMpeg.GetVideoThumbnail(@"C:\Users\jom\Downloads\Video\DaneJones.mp4", @"C:\Users\jom\Downloads\Video\thum2.jpg", 10.5f);

                //var path = "http://localhost:8002/UploadedFiles/newUser1/2af23456-55dd-484e-be75-0d4db5287a45.mp4.jpg";

                //var file = Path.GetFileName(path);
                //14.525065, 121.003339
                //var geo = new GeoCoordinate(14.536802, 121.003394);

                //var password = System.Web.Security.Membership.GeneratePassword(10, 3);
                //var distance = geo.GetDistanceTo(new GeoCoordinate(14.537137, 121.003426));

                //var emailProvider = ProviderFactory.CreateProvider<IEmailProvider>();
                //emailProvider.Subject = "G Suite testing";
                //emailProvider.Body = "testing gsuite";
                //emailProvider.To = new List<string> { "jeromecorpuz84@gmail.com" };
                //emailProvider.SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
                //emailProvider.Username = "bens@subwaytalentapp.com";
                //emailProvider.Password = "LTrain17";
                //emailProvider.HTMLBody = "testing html body gsuite. info";
                //emailProvider.From = new System.Net.Mail.MailAddress("Info@SubwayTalentApp.Com", "SubwayTalent");
                //emailProvider.Cc = new List<string>();

                //emailProvider.Send();
                var authOn = "::kjgkhjk";
                var split = authOn.Split(':');
                var bytes = Encoding.Default.GetBytes("newUser1|12345|sdfgsdfg");
                var base64 = Convert.ToBase64String(bytes);


                var decoded = Encoding.Default.GetString(Convert.FromBase64String(base64));
                var a = decoded;

            }
            catch (Exception ex)
            { 
            
            }


        }
    }
}
