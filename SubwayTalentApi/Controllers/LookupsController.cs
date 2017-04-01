using SubwayTalentApi.ActionFilters;
using SubwayTalentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SubwayTalentApi.Controllers
{
   
    public class LookupsController : SubwayBaseController
    {

        [HttpGet]
        public IHttpActionResult GetAllGenres()
        {
            try
            {

                var result = SubwayContext.Current.LookUpValuesRepo.GetAllValues(SubwayTalent.Contracts.Interfaces.LookUpValueType.Genres);


                ResponseModel = new ResponseModel
                {
                    Status = Status.Success,
                    Data = result,
                    RecordCount = (result == null) ? 0 : result.Count
                };

                return Ok(ResponseModel);
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;

                ResponseModel = new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                };

                return Ok(ResponseModel);
            }
        }

        [HttpGet]
        public IHttpActionResult GetAllSkills()
        {
            try
            {

                var result = SubwayContext.Current.LookUpValuesRepo.GetAllValues(SubwayTalent.Contracts.Interfaces.LookUpValueType.Skills);


                ResponseModel = new ResponseModel
                {
                    Status = Status.Success,
                    Data = result,
                    RecordCount = (result == null) ? 0 : result.Count
                };

                return Ok(ResponseModel);
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;

                ResponseModel = new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                };

                return Ok(ResponseModel);
            }
        }

        [HttpGet]
        public IHttpActionResult GetAllEventTypes()
        {
            try
            {

                var result = SubwayContext.Current.LookUpValuesRepo.GetAllValues(SubwayTalent.Contracts.Interfaces.LookUpValueType.EventTypes);


                ResponseModel = new ResponseModel
                {
                    Status = Status.Success,
                    Data = result,
                    RecordCount = (result == null) ? 0 : result.Count
                };

                return Ok(ResponseModel);
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;

                ResponseModel = new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                };

                return Ok(ResponseModel);
            }
        }

        [HttpGet]
        public IHttpActionResult GetFilterLookups()
        {
            try
            {

                var eventTypes = SubwayContext.Current.LookUpValuesRepo.GetAllValues(SubwayTalent.Contracts.Interfaces.LookUpValueType.EventTypes);
                var genres = SubwayContext.Current.LookUpValuesRepo.GetAllValues(SubwayTalent.Contracts.Interfaces.LookUpValueType.Genres);
                var skills = SubwayContext.Current.LookUpValuesRepo.GetAllValues(SubwayTalent.Contracts.Interfaces.LookUpValueType.Skills);

                var searchFilterModel = new SearchFilterModel
                {
                    EventCategories = eventTypes.ToList(),
                    Genres = genres.ToList(),
                    Skills = skills.ToList()
                };

                ResponseModel = new ResponseModel
                {
                    Status = Status.Success,
                    Data = searchFilterModel
                };

                return Ok(ResponseModel);
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;

                ResponseModel = new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                };

                return Ok(ResponseModel);
            }
        }

        [HttpGet]
        public IHttpActionResult GetAllStates()
        {
            try
            {
                var stateList = SubwayContext.Current.LookUpValuesRepo.GetAllStates();

                return Ok(new ResponseModel
                {
                    Status = Status.Success,
                    Data = stateList,
                    RecordCount = (stateList == null) ? 0 : stateList.Count
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpGet]
        public IHttpActionResult GetCitiesByStateId(string id)
        {
            try
            {
                var cityList = SubwayContext.Current.LookUpValuesRepo.GetCityByStateId(id);

                return Ok(new ResponseModel
                {
                    Status = Status.Success,
                    Data = cityList,
                    RecordCount = (cityList == null) ? 0 : cityList.Count
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = ex.Message
                });
            }
        }
    }
}
