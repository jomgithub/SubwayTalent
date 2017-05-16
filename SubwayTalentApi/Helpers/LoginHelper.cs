using SubwayTalent.Contracts;
using SubwayTalent.Core.Exceptions;
using SubwayTalentApi.Facebook;
using SubwayTalentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Helpers
{
    public static class LoginHelper
    {


        /// <summary>
        /// For Authentication filters
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        public static UserModel AuthenticateUser(UserModel userDetails)
        {
            //if user logged in using normal process
            if (string.IsNullOrWhiteSpace(userDetails.AccessToken))
            {
                var result = ValidateLogin(userDetails);

                if (result.Count > 0)
                    throw new SubwayTalentException(string.Join(" ", result.ToArray()));

                var DtoUserDetails = SubwayContext.Current.UserRepo.LoginUser(userDetails.UserId, userDetails.Password);

                if (DtoUserDetails == null)
                    throw new SubwayTalentException("user not found");
                userDetails.Birthday = DtoUserDetails.Birthday;
                userDetails.Email = DtoUserDetails.Email;
                userDetails.FacebookUser = DtoUserDetails.FacebookUser;
                userDetails.FirstName = DtoUserDetails.FirstName;
                userDetails.LastName = DtoUserDetails.LastName;
                DtoUserDetails.Device = userDetails.Device;
                DtoUserDetails.DeviceToken = userDetails.DeviceToken;

                return ConvertSubwayObject.ConvertToUserModel(DtoUserDetails);
            }
            else
            {
                var login = new FacebookLogin();
                var user = login.GetUser(userDetails.AccessToken);
                user.Device = userDetails.Device;
                user.DeviceToken = userDetails.DeviceToken;
                return user;
            }            
        }

        public static UserModel Authenticate(UserModel userDetails)
        {
            //if user logged in using normal process
            if (string.IsNullOrWhiteSpace(userDetails.AccessToken))
            {
                var result = ValidateLogin(userDetails);

                if (result.Count > 0)
                    throw new SubwayTalentException(string.Join(" ", result.ToArray()));

                var DtoUserDetails = SubwayContext.Current.UserRepo.LoginUser(userDetails.UserId, userDetails.Password);

                if (DtoUserDetails == null)
                    throw new SubwayTalentException("user not found");

                if (userDetails.Device != 0 && !string.IsNullOrWhiteSpace(userDetails.DeviceToken))
                    SubwayContext.Current.UserRepo.AddDeviceID(userDetails.UserId, userDetails.DeviceToken, userDetails.Device);

                userDetails.Birthday = DtoUserDetails.Birthday;
                userDetails.Email = DtoUserDetails.Email;
                userDetails.FacebookUser = DtoUserDetails.FacebookUser;
                userDetails.FirstName = DtoUserDetails.FirstName;
                userDetails.LastName = DtoUserDetails.LastName;
                DtoUserDetails.Device = userDetails.Device;
                DtoUserDetails.DeviceToken = userDetails.DeviceToken;

                return ConvertSubwayObject.ConvertToUserModel(DtoUserDetails);
            }
            else
            {
                var login = new FacebookLogin();
                var user = login.GetUser(userDetails.AccessToken);
                user.Device = userDetails.Device;
                user.DeviceToken = userDetails.DeviceToken;

                AddFBUser(user);
                user.DeviceToken = userDetails.DeviceToken;
                return user;
            }
        }

        private static List<string> ValidateLogin(UserModel user)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(user.UserId))
                errors.Add("userid is null");
            if (string.IsNullOrWhiteSpace(user.Password))
                errors.Add("password is null");
            //if (user.Device == 0)
            //    errors.Add("Device is required.");
            //else
            //    if (string.IsNullOrWhiteSpace(user.DeviceToken))
            //        errors.Add("DeviceToken is required. ");


            return errors;
        }

        private static void AddFBUser(UserModel userDetails)
        {
            //if (userDetails.Device == 0)
            //    throw new Exception("Device is required.");
            //else
            //    if (string.IsNullOrWhiteSpace(userDetails.DeviceToken))
            //        throw new Exception("DeviceToken is required. ");

            var userobj = SubwayContext.Current.UserRepo.GetUserDetails(userDetails.UserId);

            if (userobj == null)
            {
                               
                var DtoUserDetails = new UserAccount
                {
                    Birthday = userDetails.Birthday,
                    Email = userDetails.Email,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    UserId = userDetails.UserId,
                    FacebookUser = true,
                    LastLoggedInDate = DateTime.UtcNow
                };

                SubwayContext.Current.UserRepo.AddUser(DtoUserDetails);
            }
            if (userDetails.Device != 0 && !string.IsNullOrWhiteSpace(userDetails.DeviceToken))
                SubwayContext.Current.UserRepo.AddDeviceID(userDetails.UserId, userDetails.DeviceToken, userDetails.Device);
        }

    }
}