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
    public class TokenRepo : ITokenRepo
    {
        private string connectionStr = ConfigurationManager.ConnectionStrings["SubwayTalentConnection"].ConnectionString;

        public void AddToken(Token token)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand("subwaytalent.spSubway_AddUserToken", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("userIdParam", token.UserId));
                    cmd.Parameters.Add(new MySqlParameter("token", token.AuthToken));
                    cmd.Parameters.Add(new MySqlParameter("issuedOn", token.IssuedOn));
                    cmd.Parameters.Add(new MySqlParameter("expiresOn", token.ExpiresOn));
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Token GetToken(string tokenId, DateTime expireReference)
        {
            var token = new Token();

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand("subwaytalent.spSubway_GetUserToken", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("tokenId", tokenId));
                    cmd.Parameters.Add(new MySqlParameter("expireDateRef", expireReference));   
                    
                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {

                        token = new Token
                        {
                            AuthToken = Convert.ToString(dr["auth_token"]),
                            TokenId = Convert.ToInt32(dr["id"]),
                            UserId = Convert.ToString(dr["user_id"]),
                            IssuedOn = Convert.ToDateTime(dr["issued_on"]),
                            ExpiresOn = Convert.ToDateTime(dr["expires_on"])
                        };
                    }
                }
            }

            return token;
        }

        public void DeleteToken(string tokenId)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand("subwaytalent.spSubway_DeleteUserToken", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("token", tokenId));              

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateToken(Token token)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand("subwaytalent.spSubway_UpdateUserToken", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("userIdParam", token.UserId));
                    cmd.Parameters.Add(new MySqlParameter("token", token.AuthToken));
                    cmd.Parameters.Add(new MySqlParameter("expiresOn", token.ExpiresOn));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTokenByUserId(string userId)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand("subwaytalent.spSubway_DeleteTokenByUserId", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("userIdParam", userId));                   

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
