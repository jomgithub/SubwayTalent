using SubwayTalentApi.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Helpers
{
    public static class LocationHelper
    {

        public static GeoCoordinate IsValidLocation(string latitude, string longitude)
        {
            double lat, lon;

            if (!double.TryParse(latitude, out lat))
                throw new Exception("Invalid latitude");


            if (!double.TryParse(longitude, out lon))
                throw new Exception("Invalid longitude");


            return new GeoCoordinate(lat, lon);
        }

        public static List<T> FilterDistace<T>(SearchModel searchModel, List<T> result, string searchType)
        {
            var list = new List<T>();

            if (searchModel.DistanceInMeters > 0 && (searchModel.CityStateID != null || searchModel.CurrentLocation != null))
            {

                foreach (var item in result)
                {
                    var longPropInfo = item.GetType().GetProperty("Longitude");
                    var latPropInfo = item.GetType().GetProperty("Latitude");
                    var distancePropInfo = item.GetType().GetProperty("Distance");
                    var itemId = (item.GetType().GetProperty("Id") == null) ? item.GetType().GetProperty("UserId") : item.GetType().GetProperty("Id");
                    var longValue = longPropInfo.GetValue(item);
                    var latValue = latPropInfo.GetValue(item);


                    if (!string.IsNullOrWhiteSpace(longValue.ToString()) && !string.IsNullOrWhiteSpace(latValue.ToString()))
                    {
                        try
                        {
                            //check if the event has a valid location. if not go to the next event.
                            var itemLoc = LocationHelper.IsValidLocation(latValue.ToString(), longValue.ToString());

                            //if there are locations selected in the filters.
                            if (searchModel.CityStateID != null && searchModel.CityStateID.Count > 0)
                            {
                                var cityStateIdList = string.Join(",", searchModel.CityStateID);
                                var cityInfo = SubwayContext.Current.LookUpValuesRepo.GetCityInfoByCityStateId(cityStateIdList);

                                foreach (var city in cityInfo)
                                {
                                    var cityLocation = LocationHelper.IsValidLocation(city.Latitude, city.Longitude);

                                    //multiplied by 1609.34 because Client-side is now passing units in Mi.
                                    if (itemLoc.GetDistanceTo(cityLocation) <= searchModel.DistanceInMeters * 1609.34)
                                    {
                                        distancePropInfo.SetValue(item, itemLoc.GetDistanceTo(cityLocation));
                                        list.Add(item);
                                    }
                                }
                            }
                            else
                            {
                                //it's using the CurrentLocation passed from the web service
                                //if currentlocation with regards to event is less than the radius.
                                var currentLoc = LocationHelper.IsValidLocation(searchModel.CurrentLocation.Latitude, searchModel.CurrentLocation.Longitude);
                                if (itemLoc.GetDistanceTo(currentLoc) <= searchModel.DistanceInMeters)
                                {
                                    distancePropInfo.SetValue(item, itemLoc.GetDistanceTo(currentLoc));
                                    list.Add(item);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            SubwayContext.Current.Logger.Log(string.Format("{0} search error. Invalid location for [{1}]. {2}", searchType, itemId.GetValue(item), ex.Message));
                            continue;
                        }
                    }
                }
            }
            else
                return result;

            return list;
        }
    }
}