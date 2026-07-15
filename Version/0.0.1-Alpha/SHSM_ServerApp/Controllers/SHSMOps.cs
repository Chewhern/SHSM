using ASodium;
using BCASodium;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SHSM_ServerApp.Helper;
using SHSM_ServerApp.SPKIDataModel;
using SimplifiedArweaveSDK.ArweaveHelper;
using SimplifiedArweaveSDK.ArweaveSubHelper;
using System.Text;

namespace SHSM_ServerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SHSMOps : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection;

        [HttpGet]
        public String DeleteSHSM(String User_ID, String SignedChallengeB64)
        {
            Boolean IsUserExist = RegisteredUsersHelper.users.ContainsKey(User_ID);
            String ResultString = "";
            if (IsUserExist)
            {
                int SpecificIndex = RegisteredUsersHelper.usersindices[User_ID];
                String ArweaveTXID = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RegisteredUser.AUInfoModelArweaveID;
                String ArweaveTXData = GetTransactionDataHelper.GetTransactionData(ArweaveTXID);
                Byte[] DecodedArweaveTXData = Base64URLEncodeDecodeHelper.Decode(ArweaveTXData);
                String AUInfoArweaveTXData = Encoding.UTF8.GetString(DecodedArweaveTXData);
                AUInfoModel UserInfo = JsonConvert.DeserializeObject<AUInfoModel>(AUInfoArweaveTXData);
                String UserLongTermEDAuthPublicKeyB64 = UserInfo.Auth_PK;
                Byte[] UserLongTermEDAuthPublicKey = Convert.FromBase64String(UserLongTermEDAuthPublicKeyB64);
                Byte[] SignedChallenge = Convert.FromBase64String(SignedChallengeB64);
                Byte[] Challenge = new Byte[] { };
                Boolean AbleToVerifySignedChallenge = true;
                try
                {
                    if (UserLongTermEDAuthPublicKey.Length == 32)
                    {
                        Challenge = SodiumPublicKeyAuth.Verify(SignedChallenge, UserLongTermEDAuthPublicKey);
                    }
                    else
                    {
                        Challenge = SecureED448.GetMessageFromSignatureMessage(UserLongTermEDAuthPublicKey, SignedChallenge, new Byte[] { });
                    }
                }
                catch
                {
                    AbleToVerifySignedChallenge = false;
                }
                if (AbleToVerifySignedChallenge)
                {
                    MySqlCommand MySQLGeneralQuery = new MySqlCommand();
                    String ExceptionString = "";
                    int Count = 0;
                    myMyOwnMySQLConnection = new MyOwnMySQLConnection();
                    myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                    MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `User_Challenge` WHERE `User_ID`=@User_ID AND `Challenge`=@Challenge";
                    MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = User_ID;
                    MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(Challenge);
                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                    MySQLGeneralQuery.Prepare();
                    Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                    if (Count == 1)
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
                            MySQLGeneralQuery.CommandText = "DELETE FROM `User_Challenge` WHERE `User_ID`=@User_ID";
                            MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = User_ID;
                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                            MySQLGeneralQuery.Prepare();
                            MySQLGeneralQuery.ExecuteNonQuery();
                            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption != IntPtr.Zero ||
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero ||
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey != null ||
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.CheckIsInvalid() == false ||
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP != null ||
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts != null ||
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey != IntPtr.Zero) 
                            {
                                RemoveSHSMHelper.PartialRemoveSHSM(SpecificIndex, User_ID);
                                ResultString = "Success: Able to successfully delete SHSM.";
                            }
                            else 
                            {
                                ResultString = "Error: This user is not elligible in removing its own SHSM";
                            }
                        }
                        else
                        {
                            MySQLGeneralQuery = new MySqlCommand();
                            MySQLGeneralQuery.CommandText = "DELETE FROM `User_Challenge` WHERE `User_ID`=@User_ID";
                            MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = User_ID;
                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                            MySQLGeneralQuery.Prepare();
                            MySQLGeneralQuery.ExecuteNonQuery();
                            ResultString = "Error: Deleting the generated challenge that expired..";
                        }
                    }
                    else
                    {
                        ResultString = "Error: This verified challenge does not exist";
                    }
                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                    myMyOwnMySQLConnection.ClearConnectionString();
                }
                else
                {
                    ResultString = "Error: Unable to verify signed challenge";
                }
            }
            else
            {
                ResultString = "Error: This user does not exist..";
            }
            return ResultString;
        }
    }
}
