using ASodium;
using MySql.Data.MySqlClient;

namespace SHSM_ServerApp.Helper
{
    public class MyOwnMySQLConnection
    {
        public MySqlConnection MyMySQLConnection = new MySqlConnection();
        public Boolean CheckConnection;
        public String SecretPath = "/Project/SHSM/db/SHSM_P.txt";

        public void setConnection() {
            MyMySQLConnection.ConnectionString = File.ReadAllText(SecretPath);
        }

        public Boolean LoadConnection(ref String Exception) {
            setConnection();
            try
            {
                MyMySQLConnection.Open();
                CheckConnection = true;
            }
            catch (MySqlException exception){
                CheckConnection = false;
                Exception = exception.ToString();
            }
            return CheckConnection;
        }

        public void ClearConnectionString() 
        {
            SodiumSecureMemory.SecureClearString(MyMySQLConnection.ConnectionString);
        }
    }
}
