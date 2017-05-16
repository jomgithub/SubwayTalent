using MySql.Data.MySqlClient;
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
    public class SettingsRepo : ISettings
    {
        private string schema = ConfigurationManager.AppSettings["DBSchema"];
        private string connectionStr = ConfigurationManager.ConnectionStrings["SubwayTalentConnection"].ConnectionString;

        public void AddSettings(string key, string value)
        {
            var errMsgs = string.Empty;
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddSettings", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("settingName", key));
                    cmd.Parameters.Add(new MySqlParameter("settingValue", value));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        var result = Convert.ToInt16(dr[0]);

                        if (result == 0)
                            errMsgs = "setting already exist";

                    }
                    dr.Close();
                }
            }

            if (!string.IsNullOrWhiteSpace(errMsgs)) throw new Exception(errMsgs);
        }

        public void UpdateSettings(string key, string value)
        {
            var errMsgs = string.Empty;
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_UpdateSettings", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("settingName", key));
                    cmd.Parameters.Add(new MySqlParameter("settingValue", value));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        var result = Convert.ToInt16(dr[0]);

                        if (result == 0)
                            errMsgs = "setting doesn't exist";

                    }
                    dr.Close();
                }
            }

            if (!string.IsNullOrWhiteSpace(errMsgs)) throw new Exception(errMsgs);
        }


        public void DeleteSettings(string key)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetSettings()
        {
            var settings = new Dictionary<string, string>();
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetSettings", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        settings.Add(Convert.ToString(dr["setting_name"]), Convert.ToString(dr["setting_value"]));
                    }
                    dr.Close();
                }
            }

            return settings;
        }

        public string GetSetting(string key)
        {
            throw new NotImplementedException();
        }
    }
}
