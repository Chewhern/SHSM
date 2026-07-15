using ASodium;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SHSM_ServerApp.Helper;

namespace SHSM_ServerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChallengeRequestor : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection;

        [HttpGet]
        public String GenerateOrFetchChallenge(String User_ID)
        {
            if (RegisteredUsersHelper.users.ContainsKey(User_ID))
            {
                MySqlCommand MySQLGeneralQuery = new MySqlCommand();
                String ExceptionString = "";
                int Count = 0;
                String ResultString = "";
                myMyOwnMySQLConnection = new MyOwnMySQLConnection();
                myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `User_Challenge` WHERE `User_ID`=@User_ID";
                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = User_ID;
                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                MySQLGeneralQuery.Prepare();
                Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                if (Count == 0)
                {
                    Byte[] Challenge = new Byte[] { };
                    Challenge = SodiumRNG.GetRandomBytes(128);
                    MySQLGeneralQuery = new MySqlCommand();
                    MySQLGeneralQuery.CommandText = "INSERT INTO `User_Challenge`(`User_ID`, `Challenge`, `Valid_Duration`) VALUES (@User_ID, @Challenge, @Valid_Duration)";
                    MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = User_ID;
                    MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(Challenge);
                    MySQLGeneralQuery.Parameters.Add("@Valid_Duration", MySqlDbType.DateTime).Value = DateTime.UtcNow.AddHours(8);
                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                    MySQLGeneralQuery.Prepare();
                    MySQLGeneralQuery.ExecuteNonQuery();
                    ResultString = Convert.ToBase64String(Challenge);
                }
                else
                {
                    DateTime CurrentDateTime = DateTime.UtcNow.AddHours(8);
                    DateTime DatabaseDateTime = new DateTime();
                    TimeSpan MyTimeSpan = new TimeSpan();
                    MySQLGeneralQuery = new MySqlCommand();
                    MySQLGeneralQuery.CommandText = "SELECT `Valid_Duration` FROM `User_Challenge` WHERE `User_ID`=@User_ID";
                    MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = User_ID;
                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                    MySQLGeneralQuery.Prepare();
                    DatabaseDateTime = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                    MyTimeSpan = CurrentDateTime.Subtract(DatabaseDateTime);
                    if (MyTimeSpan.TotalMinutes <= 8)
                    {
                        MySQLGeneralQuery = new MySqlCommand();
                        MySQLGeneralQuery.CommandText = "SELECT `Challenge` FROM `User_Challenge` WHERE `User_ID`=@User_ID";
                        MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = User_ID;
                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                        MySQLGeneralQuery.Prepare();
                        ResultString = MySQLGeneralQuery.ExecuteScalar().ToString();
                    }
                    else
                    {
                        MySQLGeneralQuery = new MySqlCommand();
                        MySQLGeneralQuery.CommandText = "DELETE FROM `User_Challenge` WHERE `User_ID`=@User_ID";
                        MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = User_ID;
                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                        MySQLGeneralQuery.Prepare();
                        MySQLGeneralQuery.ExecuteNonQuery();
                        ResultString = "Error: This specified user had exceeded the valid duration.. Deleting the generated challenge";
                    }
                }
                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                myMyOwnMySQLConnection.ClearConnectionString();
                return ResultString;
            }
            else
            {
                return "Error: This user is not in SHSM list..";
            }
        }
    }
}
