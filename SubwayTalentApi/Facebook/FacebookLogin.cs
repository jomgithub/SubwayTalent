using SubwayTalentApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace SubwayTalentApi.Facebook
{
    public class FacebookLogin
    {
        public UserModel GetUser(string accessToken)
        {

            // Exchange the code for an extended access token
            Uri eatTargetUri = new Uri("https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id=" + ConfigurationManager.AppSettings["FacebookAppId"] + "&client_secret=" + ConfigurationManager.AppSettings["FacebookAppSecret"] + "&fb_exchange_token=" + accessToken);
            HttpWebRequest eat = (HttpWebRequest)HttpWebRequest.Create(eatTargetUri);

            StreamReader eatStr = new StreamReader(eat.GetResponse().GetResponseStream());
            string eatToken = eatStr.ReadToEnd().ToString().Replace("access_token=", "");

            // Split the access token and expiration from the single string
            string[] eatWords = eatToken.Split('&');
            string extendedAccessToken = eatWords[0];

            // Request the Facebook user information
            Uri targetUserUri = new Uri("https://graph.facebook.com/me?fields=first_name,last_name,gender,locale,link,email&access_token=" + accessToken);
            HttpWebRequest user = (HttpWebRequest)HttpWebRequest.Create(targetUserUri);
            
            // Read the returned JSON object response
            StreamReader userInfo = new StreamReader(user.GetResponse().GetResponseStream());
            string jsonResponse = string.Empty;
            jsonResponse = userInfo.ReadToEnd();


           
            

            // Deserialize and convert the JSON object to the Facebook.User object type
            JavaScriptSerializer sr = new JavaScriptSerializer();
            string jsondata = jsonResponse;
            jsondata = jsondata.Replace("first_name", "FirstName").Replace("last_name", "LastName").Replace("id","UserId");
           
            UserModel converted = sr.Deserialize<UserModel>(jsondata);
            converted.FacebookUser = true;

            //// Request the Facebook user information
            //Uri targetUserUriFriends = new Uri("https://graph.facebook.com/"+ converted.UserId +"/friends?access_token=" + accessToken);
            //HttpWebRequest userFriends = (HttpWebRequest)HttpWebRequest.Create(targetUserUriFriends);


            //// Read the returned JSON object response
            //StreamReader friendsStream = new StreamReader(userFriends.GetResponse().GetResponseStream());
            //jsonResponse = friendsStream.ReadToEnd();

            // Return the current Facebook user
            return converted;
        }
    }
}