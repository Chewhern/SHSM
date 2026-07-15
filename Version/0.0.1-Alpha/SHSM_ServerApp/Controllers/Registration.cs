using ASodium;
using BCASodium;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SHSM_ServerApp.ActualSHSM;
using SHSM_ServerApp.Helper;
using SHSM_ServerApp.PostDataModel;
using SHSM_ServerApp.RSACredentials;
using SHSM_ServerApp.SHSMDataModel;
using SHSM_ServerApp.SPKIDataModel;
using SimplifiedArweaveSDK.ArweaveHelper;
using SimplifiedArweaveSDK.ArweaveSubHelper;
using System.Text;
using System.Xml.Linq;

namespace SHSM_ServerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Registration : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection;

        //GetTransactionDataHelper.GetTransactionData(ArweaveID);

        //Will have concurrent issues..
        //Will revisit back in future..
        [HttpPost]
        public String RegisterUser(RegisteredUserModel MyPostModel) 
        {
            Boolean AbleToRegister = RegisteredUsersHelper.users.TryAdd(MyPostModel.User_ID, MyPostModel);
            if (AbleToRegister) 
            {
                UserSHSMModel UserSHSM = new UserSHSMModel();
                //Do somethimg..
                String ArweaveTXID = MyPostModel.AUInfoModelArweaveID;
                String ArweaveTXData = GetTransactionDataHelper.GetTransactionData(ArweaveTXID);
                Byte[] DecodedArweaveTXData = Base64URLEncodeDecodeHelper.Decode(ArweaveTXData);
                String AUInfoArweaveTXData = Encoding.UTF8.GetString(DecodedArweaveTXData);
                AUInfoModel UserInfo = JsonConvert.DeserializeObject<AUInfoModel>(AUInfoArweaveTXData);
                DateTime AnchoredDateTime = new DateTime(UserInfo.ValidDate_Year, UserInfo.ValidDate_Month, UserInfo.ValidDate_Day);
                DateTime CurrentDateTime = DateTime.UtcNow.AddHours(8);
                if (CurrentDateTime.CompareTo(AnchoredDateTime) < 0) 
                {
                    ArweaveTXID = MyPostModel.SubSignedDSPKArweaveID;
                    ArweaveTXData = GetTransactionDataHelper.GetTransactionData(ArweaveTXID);
                    DecodedArweaveTXData = Base64URLEncodeDecodeHelper.Decode(ArweaveTXData);
                    String AUSubSignedPKTXData = Encoding.UTF8.GetString(DecodedArweaveTXData);
                    SubSignedPKModel SubSignedPKInfo = JsonConvert.DeserializeObject<SubSignedPKModel>(AUSubSignedPKTXData);
                    AnchoredDateTime = new DateTime(SubSignedPKInfo.ValidDate_Year, SubSignedPKInfo.ValidDate_Month, SubSignedPKInfo.ValidDate_Day);
                    CurrentDateTime = DateTime.UtcNow.AddHours(8);
                    if (CurrentDateTime.CompareTo(AnchoredDateTime) < 0) 
                    {
                        UserSHSM.RegisteredUser = MyPostModel;
                        UserSHSM.SecretKeyForEncryption = IntPtr.Zero;
                        UserSHSM.SecretKeyForMAC = IntPtr.Zero;
                        UserSHSM.ED25519DigitalSignatureKP = null;
                        UserSHSM.ED255119SecretKey = IntPtr.Zero;
                        UserSHSM.ED448DigitalSignatureRKP = null;
                        UserSHSM.ED448SecretKey = null;
                        UserSHSM.RSAKeyParts = null;
                        UserSHSM.X25519EncryptionSecretKey = IntPtr.Zero;
                        UserSHSM.EncryptionKEMKPSecretKey = IntPtr.Zero;
                        UserSHSM.ValidDateTime = DateTime.MinValue;
                        UserSHSM.TempServerSHSM = null;
                        SHSMOpsHelper.ListsOfRegisteredUsers.Add(UserSHSM);
                        RegisteredUsersHelper.usersindices.Add(MyPostModel.User_ID, SHSMOpsHelper.ListsOfRegisteredUsers.Count - 1);
                        return "Success: Able to add this to SHSM list.";
                    }
                    else 
                    {
                        return "Error: Sub signed digital signature public key for this user had expired.";
                    }
                }
                else 
                {
                    return "Error: The anchored information had already expired.";
                }
            }
            else 
            {
                return "Error: This user was already exist..";
            }
        }

        //Allow changing of signed publickeys..
        [HttpPost("Update")]
        public String UpdatePublicKeysForTargetUser(RegistrationUpdateSignedPublicKeysModel PostModel) 
        {
            Boolean IsUserExist = RegisteredUsersHelper.users.ContainsKey(PostModel.User_ID);
            String ResultString = "";
            if (IsUserExist) 
            {
                String ArweaveTXID = SHSMOpsHelper.ListsOfRegisteredUsers[RegisteredUsersHelper.usersindices[PostModel.User_ID]].RegisteredUser.SubSignedDSPKArweaveID;
                String ArweaveTXData = GetTransactionDataHelper.GetTransactionData(ArweaveTXID);
                Byte[] DecodedArweaveTXData = Base64URLEncodeDecodeHelper.Decode(ArweaveTXData);
                String SubSignedPKModelArweaveTXData = Encoding.UTF8.GetString(DecodedArweaveTXData);
                SubSignedPKModel UserSubSignedPKInfo = JsonConvert.DeserializeObject<SubSignedPKModel>(SubSignedPKModelArweaveTXData);
                ArweaveTXID = SHSMOpsHelper.ListsOfRegisteredUsers[RegisteredUsersHelper.usersindices[PostModel.User_ID]].RegisteredUser.AUInfoModelArweaveID;
                ArweaveTXData = GetTransactionDataHelper.GetTransactionData(ArweaveTXID);
                DecodedArweaveTXData = Base64URLEncodeDecodeHelper.Decode(ArweaveTXData);
                String AUInfoArweaveTXData = Encoding.UTF8.GetString(DecodedArweaveTXData);
                AUInfoModel UserInfo = JsonConvert.DeserializeObject<AUInfoModel>(AUInfoArweaveTXData);
                String UserLongTermEDSigningPublicKeyB64 = UserInfo.Sign_PK;
                Byte[] UserLongTermEDSigningPublicKey = Convert.FromBase64String(UserLongTermEDSigningPublicKeyB64);
                String UserShortTermSignedEDGeneralDigitalSignaturePublicKeyB64 = UserSubSignedPKInfo.SignedDigitalSignaturePublicKeyB64;
                Byte[] UserShortTermSignedEDGeneralDigitalSignaturePublicKey = Convert.FromBase64String(UserShortTermSignedEDGeneralDigitalSignaturePublicKeyB64);
                Byte[] UserShortTermEDGeneralDigitalSignaturePublicKey = new Byte[] { };
                Byte[] SignedChallenge = Convert.FromBase64String(PostModel.SignedChallengeB64);
                Byte[] Challenge = new Byte[] { };
                Boolean AbleToVerifyShortTermSignedEDGeneralDSPublicKey = true;
                Boolean AbleToVerifySignedChallenge = true;
                try 
                {
                    if (UserLongTermEDSigningPublicKey.Length == 32)
                    {
                        UserShortTermEDGeneralDigitalSignaturePublicKey = SodiumPublicKeyAuth.Verify(UserShortTermSignedEDGeneralDigitalSignaturePublicKey, UserLongTermEDSigningPublicKey);
                    }
                    else
                    {
                        UserShortTermEDGeneralDigitalSignaturePublicKey = SecureED448.GetMessageFromSignatureMessage(UserLongTermEDSigningPublicKey, UserShortTermSignedEDGeneralDigitalSignaturePublicKey, new Byte[] { });
                    }
                }
                catch 
                {
                    AbleToVerifyShortTermSignedEDGeneralDSPublicKey = false;
                }
                if (AbleToVerifyShortTermSignedEDGeneralDSPublicKey) 
                {
                    try 
                    {
                        if (UserShortTermEDGeneralDigitalSignaturePublicKey.Length == 32)
                        {
                            Challenge = SodiumPublicKeyAuth.Verify(SignedChallenge, UserShortTermEDGeneralDigitalSignaturePublicKey);
                        }
                        else
                        {
                            Challenge = SecureED448.GetMessageFromSignatureMessage(UserShortTermEDGeneralDigitalSignaturePublicKey, SignedChallenge, new Byte[] { });
                        }
                    }
                    catch 
                    {
                        AbleToVerifySignedChallenge = false;
                    }
                    if (AbleToVerifySignedChallenge) 
                    {
                        //Close the connection..
                        //clear the connection string..
                        MySqlCommand MySQLGeneralQuery = new MySqlCommand();
                        String ExceptionString = "";
                        int Count = 0;
                        myMyOwnMySQLConnection = new MyOwnMySQLConnection();
                        myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                        MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `User_Challenge` WHERE `User_ID`=@User_ID AND `Challenge`=@Challenge";
                        MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = PostModel.User_ID;
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
                            MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = PostModel.User_ID;
                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                            MySQLGeneralQuery.Prepare();
                            DatabaseDateTime = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                            MyTimeSpan = CurrentDateTime.Subtract(DatabaseDateTime);
                            if (MyTimeSpan.TotalMinutes <= 8)
                            {
                                MySQLGeneralQuery = new MySqlCommand();
                                MySQLGeneralQuery.CommandText = "DELETE FROM `User_Challenge` WHERE `User_ID`=@User_ID";
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = PostModel.User_ID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                MySQLGeneralQuery.ExecuteNonQuery();
                                SHSMOpsHelper.ListsOfRegisteredUsers[RegisteredUsersHelper.usersindices[PostModel.User_ID]].RegisteredUser.UserSignedPublicKeys = PostModel.DataUpdateModel;
                                ResultString = "Success: Public Keys had been updated";
                            }
                            else
                            {
                                MySQLGeneralQuery = new MySqlCommand();
                                MySQLGeneralQuery.CommandText = "DELETE FROM `User_Challenge` WHERE `User_ID`=@User_ID";
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = PostModel.User_ID;
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
                    ResultString = "Error: Unable to verify user signed short term general digital signature public key";
                }
            }
            else 
            {
                ResultString = "Error: This user does not exist in SHSM list..";
            }
            return ResultString;
        }
    }
}
