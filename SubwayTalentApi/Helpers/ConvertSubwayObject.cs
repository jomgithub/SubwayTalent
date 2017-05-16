using SubwayTalent.Contracts;
using SubwayTalentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Helpers
{
    public static class ConvertSubwayObject
    {

        public static Event ConvertToEvent(EventModel eventModel)
        {

            return new Event
            {
                Id = eventModel.Id,
                Location = eventModel.Location,
                Description = eventModel.Description,
                DateEnd = eventModel.DateEnd,
                DateStart = eventModel.DateStart,
                DateCreated = eventModel.DateCreated,
                Name = eventModel.Name,
                Status = eventModel.Status,
                Title = eventModel.Title,
                Type = (eventModel.Type == null) ? null : new EventType
                {
                    Id = eventModel.Type.Id,
                    Name = eventModel.Type.Name
                },
                Latitude = eventModel.Latitude,
                Longitude = eventModel.Longitude,
                Picture = eventModel.Picture,
                Planners = (eventModel.Planners == null) ? null : new List<UserAccount>(
                                eventModel.Planners.Select(planner =>
                                {
                                    return new UserAccount
                                    {
                                        UserId = planner.UserId,
                                        TalentName = planner.TalentName,
                                        Rating = planner.Rating,
                                        Comments = planner.Comments,
                                        ProfilePic = planner.ProfilePic,
                                        ProfilePicTalent = planner.ProfilePicTalent
                                    };
                                })),
                Talents = (eventModel.Talents == null) ? null : new List<UserAccount>(
                             eventModel.Talents.Select(talent =>
                             {
                                 return new UserAccount
                                 {
                                     UserId = talent.UserId,
                                     TalentName = talent.TalentName,
                                     Rating = talent.Rating,
                                     Comments = talent.Comments,
                                     ProfilePic = talent.ProfilePic,
                                     ProfilePicTalent = talent.ProfilePicTalent
                                 };
                             })),
                PreferredGenres = (eventModel.PreferredGenres == null) ? null : new List<LookUpValues>(
                               eventModel.PreferredGenres.Select(genre =>
                               {
                                   return new LookUpValues
                                   {
                                       Id = genre.Id
                                   };
                               })),
                PreferredSkills = (eventModel.PreferredSkills == null) ? null : new List<LookUpValues>(
                               eventModel.PreferredSkills.Select(skill =>
                               {
                                   return new LookUpValues
                                   {
                                       Id = skill.Id
                                   };
                               }))
            };
        }

        public static UserAccount ConvertToUserAccount(UserModel user)
        {
            var userAccount = new UserAccount();
            foreach (var prop in user.GetType().GetProperties())
            {
                foreach (var userProp in userAccount.GetType().GetProperties())
                {
                    var propName = prop.Name;


                    if (userProp.Name == "Skills" && user.Skills != null)
                    {
                        userAccount.Skills = new List<LookUpValues>(user.Skills.Select(skill =>
                        {
                            return new LookUpValues
                            {
                                Id = skill.Id
                            };
                        }));
                        continue;
                    }

                    if (userProp.Name == "Genres" && user.Genres != null)
                    {
                        userAccount.Genres = new List<LookUpValues>(user.Genres.Select(genre =>
                        {
                            return new LookUpValues
                            {
                                Id = genre.Id
                            };
                        }));
                        continue;
                    }

                    if (userProp.Name == prop.Name)
                    {
                        userProp.SetValue(userAccount, prop.GetValue(user));
                        continue;
                    }

                }
            }

            return userAccount;
        }

        public static UserModel ConvertToUserModel(UserAccount user)
        {
            var userModel = new UserModel();
            foreach (var prop in user.GetType().GetProperties())
            {
                foreach (var userProp in userModel.GetType().GetProperties())
                {
                    var propName = prop.Name;


                    if (userProp.Name == "Skills" && user.Skills != null)
                    {
                        userModel.Skills = new List<Skills>(user.Skills.Select(skill =>
                        {
                            return new Skills
                            {
                                Id = skill.Id
                            };
                        }));
                        continue;
                    }

                    if (userProp.Name == "Genres" && user.Genres != null)
                    {
                        userModel.Genres = new List<Genre>(user.Genres.Select(genre =>
                        {
                            return new Genre
                            {
                                Id = genre.Id
                            };
                        }));
                        continue;
                    }

                    if (userProp.Name == prop.Name)
                    {
                        userProp.SetValue(userModel, prop.GetValue(user));
                        continue;
                    }

                }
            }

            return userModel;
        }

    }
}