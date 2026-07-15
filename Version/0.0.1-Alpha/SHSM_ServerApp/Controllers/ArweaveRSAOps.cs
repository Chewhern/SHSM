using ASodium;
using BCASodium;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using SHSM_ServerApp.Helper;
using SHSM_ServerApp.PostDataModel;
using SHSM_ServerApp.SPKIDataModel;
using SimplifiedArweaveSDK.ArweaveHelper;
using SimplifiedArweaveSDK.ArweaveLocalHelper;
using SimplifiedArweaveSDK.ArweaveSubHelper;
using System.Text;

namespace SHSM_ServerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArweaveRSAOps : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection;
        //Import RSA via sealedbox
        //Help upload data to arweave then clear..
        /*
        (String Status, String TransactionID) MyResults = await ArweaveSecureCreateAndPostDataHelper.UploadData(JSONString, Base64URLEncodeDecodeHelper.Encode(ModulusBytes),
                        ModulusBytes, ExponentBytes, DBytes, PBytes, QBytes, DPBytes, DQBytes, InverseQBytes);
        */
        [HttpPost]
        public async Task<String> UploadDataToArweave(ArweaveRSAOpsDataModel PostDataModel) 
        {
            Boolean IsUserExist = RegisteredUsersHelper.users.ContainsKey(PostDataModel.User_ID);
            String ResultString = "";
            if (IsUserExist)
            {
                int SpecificIndex = RegisteredUsersHelper.usersindices[PostDataModel.User_ID];
                String ArweaveTXID = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RegisteredUser.SubSignedDSPKArweaveID;
                String ArweaveTXData = GetTransactionDataHelper.GetTransactionData(ArweaveTXID);
                Byte[] DecodedArweaveTXData = Base64URLEncodeDecodeHelper.Decode(ArweaveTXData);
                String SubSignedPKModelArweaveTXData = Encoding.UTF8.GetString(DecodedArweaveTXData);
                SubSignedPKModel UserSubSignedPKInfo = JsonConvert.DeserializeObject<SubSignedPKModel>(SubSignedPKModelArweaveTXData);
                ArweaveTXID = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RegisteredUser.AUInfoModelArweaveID;
                ArweaveTXData = GetTransactionDataHelper.GetTransactionData(ArweaveTXID);
                DecodedArweaveTXData = Base64URLEncodeDecodeHelper.Decode(ArweaveTXData);
                String AUInfoArweaveTXData = Encoding.UTF8.GetString(DecodedArweaveTXData);
                AUInfoModel UserInfo = JsonConvert.DeserializeObject<AUInfoModel>(AUInfoArweaveTXData);
                String UserLongTermEDSigningPublicKeyB64 = UserInfo.Sign_PK;
                Byte[] UserLongTermEDSigningPublicKey = Convert.FromBase64String(UserLongTermEDSigningPublicKeyB64);
                String UserShortTermSignedEDGeneralDigitalSignaturePublicKeyB64 = UserSubSignedPKInfo.SignedDigitalSignaturePublicKeyB64;
                Byte[] UserShortTermSignedEDGeneralDigitalSignaturePublicKey = Convert.FromBase64String(UserShortTermSignedEDGeneralDigitalSignaturePublicKeyB64);
                Byte[] UserShortTermEDGeneralDigitalSignaturePublicKey = new Byte[] { };
                Byte[] SignedChallenge = Convert.FromBase64String(PostDataModel.SignedChallengeB64);
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
                        MySqlCommand MySQLGeneralQuery = new MySqlCommand();
                        String ExceptionString = "";
                        int Count = 0;
                        myMyOwnMySQLConnection = new MyOwnMySQLConnection();
                        myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                        MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `User_Challenge` WHERE `User_ID`=@User_ID AND `Challenge`=@Challenge";
                        MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = PostDataModel.User_ID;
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
                            MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = PostDataModel.User_ID;
                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                            MySQLGeneralQuery.Prepare();
                            DatabaseDateTime = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                            MyTimeSpan = CurrentDateTime.Subtract(DatabaseDateTime);
                            if (MyTimeSpan.TotalMinutes <= 8)
                            {
                                MySQLGeneralQuery = new MySqlCommand();
                                MySQLGeneralQuery.CommandText = "DELETE FROM `User_Challenge` WHERE `User_ID`=@User_ID";
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = PostDataModel.User_ID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                MySQLGeneralQuery.ExecuteNonQuery();
                                (String Status, String TransactionID) MyResults = await ArweaveSecureCreateAndPostDataHelper.UploadData(PostDataModel.JSONDataString, Base64URLEncodeDecodeHelper.Encode(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Modulus),
                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Modulus, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Exponent, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.D, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.P, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Q, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DP, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DQ, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.InverseQ);
                                if(MyResults.Status.CompareTo("OK") == 0) 
                                {
                                    ResultString = MyResults.TransactionID;
                                }
                                else 
                                {
                                    ResultString = "Error: " + MyResults.Status;
                                }
                            }
                            else
                            {
                                MySQLGeneralQuery = new MySqlCommand();
                                MySQLGeneralQuery.CommandText = "DELETE FROM `User_Challenge` WHERE `User_ID`=@User_ID";
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = PostDataModel.User_ID;
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
                ResultString = "Error: This user does not exist..";
            }
            return ResultString;
        }
    }
}
