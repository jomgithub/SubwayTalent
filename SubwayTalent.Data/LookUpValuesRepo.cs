using MySql.Data.MySqlClient;
using SubwayTalent.Contracts;
using SubwayTalent.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Data
{
   
    public class LookUpValuesRepo : ILookUpValues
    {
        private string schema = ConfigurationManager.AppSettings["DBSchema"];
        private string connectionStr = ConfigurationManager.ConnectionStrings["SubwayTalentConnection"].ConnectionString;
        IList<LookUpValues> _lookupValues;

        public IList<Contracts.LookUpValues> GetAllValues(LookUpValueType lookUpType)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {               
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetAll" + lookUpType.ToString(), conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State == ConnectionState.Closed)
                        cmd.Connection.Open();                

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    _lookupValues = new List<LookUpValues>();

                    while (dr.Read())
                    {     
                        _lookupValues.Add(new LookUpValues
                        {
                             Id = Convert.ToInt16(dr["Id"]),
                              Name = Convert.ToString(dr["Name"])
                        });             
                    }
                    dr.Close();
                }
            }

            return _lookupValues;
        }

        public IList<States> GetAllStates()
        {
            var statesList = new List<States>();

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetStates", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State == ConnectionState.Closed)
                        cmd.Connection.Open();

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    

                    while (dr.Read())
                    {
                        statesList.Add(new States
                        {
                            Id = Convert.ToString(dr["state_id"]),
                            Name = Convert.ToString(dr["state_name"])
                        });
                    }
                    dr.Close();
                }
            }

            return statesList;
        }

        public IList<City> GetCityByStateId(string stateId)
        {
            var cityList = new List<City>();

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetCitiesByStateId", conn))
                {
                    if(cmd.Connection.State == ConnectionState.Closed)
                        cmd.Connection.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("stateId", stateId));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        cityList.Add(new City
                        {
                            Id = Convert.ToInt32(dr["us_cities_id"]),
                            Name = Convert.ToString(dr["city"])
                        });
                    }
                    dr.Close();
                }
            }

            return cityList;
        }

        public void AddLookUpValue(Contracts.LookUpValues lookupValue, LookUpValueType lookUpType)
        {
            throw new NotImplementedException();
        }

        public void DeleteLookUpValue(Contracts.LookUpValues lookupValue, LookUpValueType lookUpType)
        {
            throw new NotImplementedException();
        }

        public void UpdateLookUpValue(Contracts.LookUpValues lookupValue, LookUpValueType lookUpType)
        {
            throw new NotImplementedException();
        }


        public IList<City> GetCityInfoByCityStateId(string cityStateIds)
        {

            var cityList = new List<City>();

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetLatLongByCityStateId", conn))
                {
                    if (cmd.Connection.State == ConnectionState.Closed)
                        cmd.Connection.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("cityStateList", cityStateIds));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        cityList.Add(new City
                        {
                            Id = Convert.ToInt32(dr["us_cities_id"]),
                            Name = Convert.ToString(dr["city"]),
                            Longitude = Convert.ToString(dr["longitude"]),
                            Latitude = Convert.ToString(dr["latitude"])
                        });
                    }
                    dr.Close();
                }
            }

            return cityList;
        }
    }
}
