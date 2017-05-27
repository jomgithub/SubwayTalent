using SubwayTalent.Core.Exceptions;
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
            var result = SubwayContext.Current.LookUpValuesRepo.GetAllValues(SubwayTalent.Contracts.Interfaces.LookUpValueType.Genres);

            ResponseModel = new ResponseModel
            {
                Status = Status.Success,
                Data = result,
                RecordCount = (result == null) ? 0 : result.Count
            };

            return Ok(ResponseModel);

        }

        [HttpGet]
        public IHttpActionResult GetAllSkills()
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

        [HttpGet]
        public IHttpActionResult GetAllEventTypes()
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

        [HttpGet]
        public IHttpActionResult GetFilterLookups()
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

        [HttpGet]
        public IHttpActionResult GetAllStates()
        {

            var stateList = SubwayContext.Current.LookUpValuesRepo.GetAllStates();

            return Ok(new ResponseModel
            {
                Status = Status.Success,
                Data = stateList,
                RecordCount = (stateList == null) ? 0 : stateList.Count
            });

        }

        [HttpGet]
        public IHttpActionResult GetCitiesByStateId(string id)
        {

            var cityList = SubwayContext.Current.LookUpValuesRepo.GetCityByStateId(id);

            return Ok(new ResponseModel
            {
                Status = Status.Success,
                Data = cityList,
                RecordCount = (cityList == null) ? 0 : cityList.Count
            });

        }

        [HttpGet]
        public IHttpActionResult GetPaymentMethods()
        {

            var methods = SubwayContext.Current.PaymentRepo.GetPaymentMethods();

            return Ok(new ResponseModel
            {
                Status = Status.Success,
                Data = methods,
                RecordCount = (methods == null) ? 0 : methods.Count
            });
        }

        [HttpGet]
        public IHttpActionResult GetPrivacyContent()
        {
            var content = SubwayContext.Current.LookUpValuesRepo.GetContent("PRIVACY_CONTENT");
            
            if (content == null)
                throw new SubwayTalentException("Exception Email : PRIVACY_CONTENT is missing.");

            return Ok(new ResponseModel
            {
                Status = Status.Success,
                Data = content.Content 
            });
        }

        [HttpGet]
        public IHttpActionResult GetTOCContent()
        {
            var content = SubwayContext.Current.LookUpValuesRepo.GetContent("TERMS_CONTENT");
            
            if (content == null)
                throw new SubwayTalentException("Exception Email : TERMS_CONTENT is missing.");

            return Ok(new ResponseModel
            {
                Status = Status.Success,
                Data = content.Content
            });
        }
    }
}
