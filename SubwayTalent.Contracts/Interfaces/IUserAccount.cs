using SubwayTalent.Contracts;
using SubwayTalent.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Data.Interfaces
{
    public interface IUserAccount
    {
        UserAccount GetUserDetails(string userId);

        void AddUser(UserAccount user);

        void DeleteUser(string userid);

        void UpdateUser(UserAccount user);

        UserAccount LoginUser(string userid, string password);

        List<UserAccount> GetAllUsers(string userId);

        void AddFile(string userId, string fileType, string filePath, string fileName, string thumPath);

        List<Files> GetFiles(string userId);

        string DeleteUserFile(string userId, int id, string fileName);

        PlannerCounts GetPlannerCounts(string userId);

        TalentCounts GetTalentCounts(string userId);

        List<UserAccount> SearchTalent(string searchString, string genreList, string skillList, string userId = null);

        void UpdateUserFile(string userId, string fileName, string fileType);

        void SetProfilePic(string userId, int fileId, string perspective);

        void ChangePassword(string oldPassword, string newPassword, string userId);

        List<RatingComments> GetUserRatingsFeedback(string userId, string userType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceId"></param>
        /// <param name="deviceType">1-IOS, 2-Android</param>
        void AddDeviceID(string userId, string deviceId, Int16 deviceType);

        void RemoveDeviceID(string userId, string deviceId);

        List<UserDevice> GetUserDevices(string userId);

        void ChangeSettings(Int16 allowNotif, Int16 allowEmail, string userId);

        void AddUserNotification(string userId, int eventId, Int16 statusId, string updatedBy, Int16 notifType);

        List<UserNotification> GetUserNotification(string userId);

        void DeleteUserNotification(int notificationId);

        void UpdatePassword(string userId, string password);
        void AddHelp(string userId, string senderName, string senderEmail, string subject, string message);
    }
}
