using MySql.Data.MySqlClient;
using SubwayTalent.Contracts.Entities;
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
    public class PaymentRepo : IPaymentRepo
    {
        private string schema = ConfigurationManager.AppSettings["DBSchema"];
        private string connectionStr = ConfigurationManager.ConnectionStrings["SubwayTalentConnection"].ConnectionString;
        private List<PaymentMethod> _paymentMethodList;

        public List<PaymentMethod> GetPaymentMethods()
        {
            _paymentMethodList = new List<PaymentMethod>();

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetPaymentMethods", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                  
                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        _paymentMethodList.Add(new PaymentMethod
                        {
                            Id = Convert.ToInt32(dr["id"]),
                            Name = Convert.ToString(dr["payment_name"]),
                            Processor = Convert.ToString(dr["payment_processor"])
                        });
                    }
                    dr.Close();
                }
            }
            return _paymentMethodList;
        }


        public void AddTransactionIds(string transactionAuthId, string transactionIdCompleted, int eventPlannerId)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddPaymentTransactionAuthId", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("transactionAuthId", transactionAuthId));
                    cmd.Parameters.Add(new MySqlParameter("transactionIdCompleted", transactionIdCompleted));
                    cmd.Parameters.Add(new MySqlParameter("eventPlannerId", eventPlannerId));
                    cmd.Parameters.Add(new MySqlParameter("paymentStatus", 2));

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public void UpdateTalentPaymentStatus(int eventPlannerId, short statusId)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_UpdateTalentPaymentStatus", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("eventPlannerId", eventPlannerId));
                    cmd.Parameters.Add(new MySqlParameter("statusId", statusId));                   

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePlannerPaymentStatus(int eventPlannerId, short statusId)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_UpdateEventPlannerPayment", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("paymentStatus", statusId));
                    cmd.Parameters.Add(new MySqlParameter("eventPlannerId", eventPlannerId));
                   

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public int AddPaymentException(PaymentException paymentInfo)
        {
            int exceptionId = 0;

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddPaymentException", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("messageParam",paymentInfo.Message.Length >= 2000 ? paymentInfo.Message.Substring(0,2000) : paymentInfo.Message));
                    cmd.Parameters.Add(new MySqlParameter("eventPlannerId", paymentInfo.EventPlannerId));
                    cmd.Parameters.Add(new MySqlParameter("eventInvitesId", paymentInfo.EventInviteId ));
                    cmd.Parameters.Add(new MySqlParameter("stackTrace", paymentInfo.StackTrace.Length >= 2000 ? paymentInfo.StackTrace.Substring(0, 2000) : paymentInfo.StackTrace));
                    cmd.Parameters.Add(new MySqlParameter("eventName", paymentInfo.PaymentEventName.ToString()));
                    cmd.Parameters.Add(new MySqlParameter("otherInfo", paymentInfo.OtherInfo.Length >= 2000 ? paymentInfo.OtherInfo.Substring(0, 2000) : paymentInfo.OtherInfo));
                    cmd.Parameters.Add(new MySqlParameter("userId", paymentInfo.UserId));


                    exceptionId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return exceptionId;
        }
    }
}
