using ASodium;
using BCASodium;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SHSM_ServerApp.ActualSHSM;
using SHSM_ServerApp.Helper;
using SHSM_ServerApp.PostDataModel;
using SHSM_ServerApp.SHSMDataModel;
using SHSM_ServerApp.SPKIDataModel;
using SimplifiedArweaveSDK.ArweaveHelper;
using SimplifiedArweaveSDK.ArweaveSubHelper;
using System.Text;

namespace SHSM_ServerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ETLS : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection;

        //Explore OS/Process isolation whenever applicable
        //and other applicable measures that can be enforced on software..
        //Initialize
        //HandShake
        //Delete

        [HttpGet]
        public ServerInitialETLSDataModel InitializeETLS(String User_ID, Boolean IsKEM = true) 
        {
            Boolean IsUserExist = RegisteredUsersHelper.users.ContainsKey(User_ID);
            ServerInitialETLSDataModel MyModel = new ServerInitialETLSDataModel();
            if (IsUserExist) 
            {
                int SpecificIndex = RegisteredUsersHelper.usersindices[User_ID];
                if (IsKEM) 
                {
                    KeyPair ServerKEMKeyPair = SodiumKEM.GenerateKeyPair();
                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM == null) 
                    {
                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM = new ServerSHSMModel();
                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MACX25519KeyPair = new KeyPair();
                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair = new KeyPair();
                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair = ServerKEMKeyPair;
                        MyModel.KEMPublicKeyB64 = Convert.ToBase64String(SHSMOpsHelper.ListsOfRegisteredUsers
                            [SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPublicKey());
                        MyModel.EncryptionX25519PublicKeyB64 = "";
                        MyModel.MACX25519PublicKeyB64 = "";
                        MyModel.IsKEMOrX25519 = true;
                    }
                    else 
                    {
                        MyModel.EncryptionX25519PublicKeyB64 = "";
                        MyModel.MACX25519PublicKeyB64 = "";
                        MyModel.KEMPublicKeyB64 = "";
                        MyModel.IsKEMOrX25519 = true;
                    }
                }
                else 
                {
                    KeyPair MACX25519KeyPair = SodiumPublicKeyBox.GenerateKeyPair();
                    KeyPair EncryptionX25519KeyPair = SodiumPublicKeyBox.GenerateKeyPair();
                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM == null)
                    {
                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM = new ServerSHSMModel();
                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MACX25519KeyPair = MACX25519KeyPair;
                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair = EncryptionX25519KeyPair;
                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair = new KeyPair();
                        MyModel.KEMPublicKeyB64 = "";
                        MyModel.EncryptionX25519PublicKeyB64 = Convert.ToBase64String(SHSMOpsHelper.ListsOfRegisteredUsers
                            [SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPublicKey());
                        MyModel.MACX25519PublicKeyB64 = Convert.ToBase64String(SHSMOpsHelper.ListsOfRegisteredUsers
                            [SpecificIndex].TempServerSHSM.MACX25519KeyPair.GetPublicKey());
                        MyModel.IsKEMOrX25519 = false;
                    }
                    else
                    {
                        MyModel.EncryptionX25519PublicKeyB64 = "";
                        MyModel.MACX25519PublicKeyB64 = "";
                        MyModel.KEMPublicKeyB64 = "";
                        MyModel.IsKEMOrX25519 = false;
                    }
                }
            }
            else 
            {
                MyModel.EncryptionX25519PublicKeyB64 = "";
                MyModel.MACX25519PublicKeyB64 = "";
                MyModel.KEMPublicKeyB64 = "";
                MyModel.IsKEMOrX25519 = true;
            }
            return MyModel;
        }

        [HttpGet("DeleteETLS")]
        public String DeleteETLS(String User_ID,String SignedChallengeB64) 
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
                            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM != null) 
                            {
                                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.CheckIsInvalid()==false) 
                                {
                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.Clear();
                                }
                                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MACX25519KeyPair.CheckIsInvalid() == false)
                                {
                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MACX25519KeyPair.Clear();
                                }
                                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.CheckIsInvalid() == false)
                                {
                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.Clear();
                                }
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM = null;
                                ResultString = "Success: This ETLS had been deleted..";
                            }
                            else 
                            {
                                ResultString = "Error: This ETLS had not been specified for this user..";
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
