using ASodium;
using BCASodium;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SHSM_ServerApp.Helper;
using SHSM_ServerApp.PostDataModel;
using SHSM_ServerApp.RSACredentials;
using SHSM_ServerApp.SHSMDataModel;
using SHSM_ServerApp.SPKIDataModel;
using SimplifiedArweaveSDK.ArweaveHelper;
using SimplifiedArweaveSDK.ArweaveSubHelper;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace SHSM_ServerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicKeyCryptography : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection;

        //Generate DSA private keys
        //Import DSA private keys
        //Sign
        //Export..
        //Import SealedBox private keys
        //Decrypt
        //Import KEM private keys
        //Decrypt via KEM
        //Extend..

        //ED25519,ED448,RSA
        [HttpGet]
        public String InitializeDSAKeysPair(String User_ID, String SignedChallengeB64, int DigitalSignatureAlgorithmsIndex = 0)
        {
            Boolean IsUserExist = RegisteredUsersHelper.users.ContainsKey(User_ID);
            String ResultString = "";
            if (IsUserExist)
            {
                int SpecificIndex = RegisteredUsersHelper.usersindices[User_ID];
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
                Byte[] SignedChallenge = Convert.FromBase64String(SignedChallengeB64);
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
                                if (DigitalSignatureAlgorithmsIndex == 0) 
                                {
                                    if(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP == null) 
                                    {
                                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP = SodiumPublicKeyAuth.GenerateKeyPair();
                                        if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey() != IntPtr.Zero) 
                                        {
                                            ResultString = "Success: ED25519 digital signature algorithm's key pair had been generated";
                                        }
                                        else 
                                        {
                                            ResultString = "Error: Try again later as this's system constraint..";
                                        }
                                    }
                                    else 
                                    {
                                        if(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP != null ||
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts !=null) 
                                        {
                                            ResultString = "Error: Digital Signature key pair had been generated previously..";
                                        }
                                    }
                                }
                                else if(DigitalSignatureAlgorithmsIndex == 1) 
                                {
                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP == null)
                                    {
                                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP = SecureED448.GenerateED448RevampedKeyPair();
                                        ResultString = "Success: ED448 digital signature algorithm's key pair had been generated";
                                    }
                                    else
                                    {
                                        if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP != null ||
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts != null)
                                        {
                                            ResultString = "Error: Digital Signature key pair had been generated previously..";
                                        }
                                    }
                                }
                                else 
                                {
                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts == null)
                                    {
                                        using (RSA rsa = RSA.Create(4096))
                                        {
                                            RSAParameters MyRSAParams = rsa.ExportParameters(true);
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts = new RSACredentialsModel();
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Modulus = MyRSAParams.Modulus;
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Exponent = MyRSAParams.Exponent;
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.D = MyRSAParams.D;
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.P = MyRSAParams.P;
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Q = MyRSAParams.Q;
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DP = MyRSAParams.DP;
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DQ = MyRSAParams.DQ;
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.InverseQ = MyRSAParams.InverseQ;
                                        }
                                        ResultString = "Success: RSA key pair had been generated";
                                    }
                                    else
                                    {
                                        if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP != null ||
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP != null)
                                        {
                                            ResultString = "Error: Digital Signature key pair had been generated previously..";
                                        }
                                    }
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
                    ResultString = "Error: Unable to verify user signed short term general digital signature public key";
                }
                if (ResultString.Contains("Error") == false) 
                {
                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ValidDateTime = DateTime.UtcNow.AddHours(9);
                }
            }
            else
            {
                ResultString = "Error: This user does not exist..";
            }
            return ResultString;
        }

        [HttpPost("ImportKeys")]
        public String ImportKeys(PublicKeyCryptographyKeysImportModel PostDataModel)
        {
            Boolean IsUserExist = RegisteredUsersHelper.users.ContainsKey(PostDataModel.User_ID);
            String ResultString = "";
            if (IsUserExist)
            {
                int SpecificIndex = RegisteredUsersHelper.usersindices[PostDataModel.User_ID];
                String ArweaveTXID = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RegisteredUser.AUInfoModelArweaveID;
                String ArweaveTXData = GetTransactionDataHelper.GetTransactionData(ArweaveTXID);
                Byte[] DecodedArweaveTXData = Base64URLEncodeDecodeHelper.Decode(ArweaveTXData);
                String AUInfoArweaveTXData = Encoding.UTF8.GetString(DecodedArweaveTXData);
                AUInfoModel UserInfo = JsonConvert.DeserializeObject<AUInfoModel>(AUInfoArweaveTXData);
                String UserLongTermEDAuthPublicKeyB64 = UserInfo.Auth_PK;
                Byte[] UserLongTermEDAuthPublicKey = Convert.FromBase64String(UserLongTermEDAuthPublicKeyB64);
                Byte[] SignedChallenge = Convert.FromBase64String(PostDataModel.SignedChallengeB64);
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
                            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM == null)
                            {
                                ResultString = "Error: ETLS had not been initiated.";
                            }
                            else
                            {
                                if (PostDataModel.IsKEMOrSealedBox)
                                {
                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.CheckIsInvalid()==false) 
                                    {
                                        Boolean AbleToDecrypt = true;
                                        if (PostDataModel.IsDSOrSealedBoxOrKEMKeyType == 0)
                                        {
                                            Byte[] EncryptedPrivateKey = new Byte[] { };
                                            Byte[] EncapsulatedSharedSecretForEPK = new Byte[SodiumKEM.GetCipherTextBytesLength()];
                                            IntPtr SharedSecretForEPK = IntPtr.Zero;
                                            Byte[] TrimmedEncryptedPrivateKey = new Byte[] { };
                                            Byte[] Nonce = new Byte[] { };
                                            Byte[] ActualPrivateKey = new Byte[] { };
                                            Boolean IsZero = true;
                                            IntPtr ActualPrivateKeyIntPtr = IntPtr.Zero;
                                            if (PostDataModel.EncryptedPrivateKeyB64.CompareTo("") != 0)
                                            {
                                                EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.EncryptedPrivateKeyB64);
                                            }
                                            if (PostDataModel.IsED25519OrED448OrRSA == 0)
                                            {
                                                if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                    TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                    Array.Copy(EncryptedPrivateKey, 0, EncapsulatedSharedSecretForEPK, 0, EncapsulatedSharedSecretForEPK.LongLength);
                                                    Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                    try
                                                    {
                                                        SharedSecretForEPK = SodiumKEM.DecapsulateSharedSecret(EncapsulatedSharedSecretForEPK, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPrivateKey());
                                                    }
                                                    catch
                                                    {
                                                        AbleToDecrypt = false;
                                                    }
                                                    if (AbleToDecrypt)
                                                    {
                                                        if (SharedSecretForEPK == IntPtr.Zero)
                                                        {
                                                            ResultString = "Error: The server faced some system constraint.. Try again later..";
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBox.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                ActualPrivateKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero, ActualPrivateKey.Length);
                                                                if (IsZero)
                                                                {
                                                                    ResultString = "Error: Unable to decrypt properly due to system constraint. Try again later..";
                                                                }
                                                                else
                                                                {
                                                                    Marshal.Copy(ActualPrivateKey, 0, ActualPrivateKeyIntPtr, ActualPrivateKey.Length);
                                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(ActualPrivateKeyIntPtr);
                                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey = ActualPrivateKeyIntPtr;
                                                                    ResultString = "Success: Able to import ED25519 private key";
                                                                }
                                                                SodiumSecureMemory.SecureClearBytes(ActualPrivateKey);
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt submitted encrypted private key";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to decrypt temporary shared secret..";
                                                    }
                                                }
                                                else
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                    TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                    Array.Copy(EncryptedPrivateKey, 0, EncapsulatedSharedSecretForEPK, 0, EncapsulatedSharedSecretForEPK.LongLength);
                                                    Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                    try
                                                    {
                                                        SharedSecretForEPK = SodiumKEM.DecapsulateSharedSecret(EncapsulatedSharedSecretForEPK, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPrivateKey());
                                                    }
                                                    catch
                                                    {
                                                        AbleToDecrypt = false;
                                                    }
                                                    if (AbleToDecrypt)
                                                    {
                                                        if (SharedSecretForEPK == IntPtr.Zero)
                                                        {
                                                            ResultString = "Error: The server faced some system constraint.. Try again later..";
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                ActualPrivateKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero, ActualPrivateKey.Length);
                                                                if (IsZero)
                                                                {
                                                                    ResultString = "Error: Unable to decrypt properly due to system constraint. Try again later..";
                                                                }
                                                                else
                                                                {
                                                                    Marshal.Copy(ActualPrivateKey, 0, ActualPrivateKeyIntPtr, ActualPrivateKey.Length);
                                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(ActualPrivateKeyIntPtr);
                                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey = ActualPrivateKeyIntPtr;
                                                                    ResultString = "Success: Able to import ED25519 private key";
                                                                }
                                                                SodiumSecureMemory.SecureClearBytes(ActualPrivateKey);
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt submitted encrypted private key";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to decrypt temporary shared secret..";
                                                    }
                                                }
                                            }
                                            else if (PostDataModel.IsED25519OrED448OrRSA == 1)
                                            {
                                                if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                    TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                    Array.Copy(EncryptedPrivateKey, 0, EncapsulatedSharedSecretForEPK, 0, EncapsulatedSharedSecretForEPK.LongLength);
                                                    Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                    try
                                                    {
                                                        SharedSecretForEPK = SodiumKEM.DecapsulateSharedSecret(EncapsulatedSharedSecretForEPK, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPrivateKey());
                                                    }
                                                    catch
                                                    {
                                                        AbleToDecrypt = false;
                                                    }
                                                    if (AbleToDecrypt)
                                                    {
                                                        if (SharedSecretForEPK == IntPtr.Zero)
                                                        {
                                                            ResultString = "Error: The server faced some system constraint.. Try again later..";
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBox.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey = ActualPrivateKey;
                                                                ResultString = "Success: Able to import ED448 private key";
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt submitted encrypted private key";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to decrypt temporary shared secret..";
                                                    }
                                                }
                                                else
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                    TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                    Array.Copy(EncryptedPrivateKey, 0, EncapsulatedSharedSecretForEPK, 0, EncapsulatedSharedSecretForEPK.LongLength);
                                                    Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                    try
                                                    {
                                                        SharedSecretForEPK = SodiumKEM.DecapsulateSharedSecret(EncapsulatedSharedSecretForEPK, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPrivateKey());
                                                    }
                                                    catch
                                                    {
                                                        AbleToDecrypt = false;
                                                    }
                                                    if (AbleToDecrypt)
                                                    {
                                                        if (SharedSecretForEPK == IntPtr.Zero)
                                                        {
                                                            ResultString = "Error: The server faced some system constraint.. Try again later..";
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey = ActualPrivateKey;
                                                                ResultString = "Success: Able to import ED448 private key";
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt submitted encrypted private key";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to decrypt temporary shared secret..";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                /*
                                                if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                {
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts = new RSACredentialsModel();
                                                    EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedExponentB64);
                                                    Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                    TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                    Array.Copy(EncryptedPrivateKey, 0, EncapsulatedSharedSecretForEPK, 0, EncapsulatedSharedSecretForEPK.LongLength);
                                                    Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                    try
                                                    {
                                                        SharedSecretForEPK = SodiumKEM.DecapsulateSharedSecret(EncapsulatedSharedSecretForEPK, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPrivateKey());
                                                    }
                                                    catch
                                                    {
                                                        AbleToDecrypt = false;
                                                    }
                                                    if (AbleToDecrypt)
                                                    {
                                                        if (SharedSecretForEPK == IntPtr.Zero)
                                                        {
                                                            ResultString = "Error: The server faced some system constraint.. Try again later..";
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBox.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Exponent = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's Exponent";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedModulusB64);
                                                            Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBox.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Modulus = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's modulus";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedDB64);
                                                            Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBox.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.D = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's D";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedPB64);
                                                            Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBox.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.P = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's P";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedQB64);
                                                            Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBox.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Q = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's Q";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedDPB64);
                                                            Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBox.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DP = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's DP";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedDQB64);
                                                            Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBox.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DQ = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's DQ";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedInverseQB64);
                                                            Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBox.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.InverseQ = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's InverseQ";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to decrypt temporary shared secret that encrypt RSA's Exponent..";
                                                    }
                                                }
                                                else
                                                {
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts = new RSACredentialsModel();
                                                    EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedExponentB64);
                                                    Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                    TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                    Array.Copy(EncryptedPrivateKey, 0, EncapsulatedSharedSecretForEPK, 0, EncapsulatedSharedSecretForEPK.LongLength);
                                                    Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                    try
                                                    {
                                                        SharedSecretForEPK = SodiumKEM.DecapsulateSharedSecret(EncapsulatedSharedSecretForEPK, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPrivateKey());
                                                    }
                                                    catch
                                                    {
                                                        AbleToDecrypt = false;
                                                    }
                                                    if (AbleToDecrypt)
                                                    {
                                                        if (SharedSecretForEPK == IntPtr.Zero)
                                                        {
                                                            ResultString = "Error: The server faced some system constraint.. Try again later..";
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Exponent = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's Exponent";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedModulusB64);
                                                            Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Modulus = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's modulus";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedDB64);
                                                            Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.D = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's D";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedPB64);
                                                            Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.P = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's P";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedQB64);
                                                            Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Q = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's Q";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedDPB64);
                                                            Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DP = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's DP";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedDQB64);
                                                            Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DQ = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's DQ";
                                                            }
                                                            EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedInverseQB64);
                                                            Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                            TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                            Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                            try
                                                            {
                                                                ActualPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            }
                                                            catch
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                            if (AbleToDecrypt)
                                                            {
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.InverseQ = ActualPrivateKey;
                                                            }
                                                            else
                                                            {
                                                                ResultString = "Error: Failed to decrypt encrypted RSA's InverseQ";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to decrypt temporary shared secret that encrypt RSA's Exponent..";
                                                    }
                                                }
                                                if (ResultString.Contains("Error") == false)
                                                {
                                                    ResultString = "Success: Able to import whole RSA key parts after decrypting successfully..";
                                                }
                                                */
                                                ResultString = "Error: Unable to support import of RSA keys via KEM";
                                            }
                                        }
                                        else if (PostDataModel.IsDSOrSealedBoxOrKEMKeyType == 1)
                                        {
                                            Byte[] EncryptedPrivateKey = new Byte[] { };
                                            Byte[] EncapsulatedSharedSecretForEPK = new Byte[SodiumKEM.GetCipherTextBytesLength()];
                                            IntPtr SharedSecretForEPK = IntPtr.Zero;
                                            Byte[] TrimmedEncryptedPrivateKey = new Byte[] { };
                                            Byte[] Nonce = new Byte[] { };
                                            Byte[] ActualPrivateKey = new Byte[] { };
                                            Boolean IsZero = true;
                                            IntPtr ActualPrivateKeyIntPtr = IntPtr.Zero;
                                            if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                            {
                                                EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.EncryptedPrivateKeyB64);
                                                Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                Array.Copy(EncryptedPrivateKey, 0, EncapsulatedSharedSecretForEPK, 0, EncapsulatedSharedSecretForEPK.LongLength);
                                                Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                try
                                                {
                                                    SharedSecretForEPK = SodiumKEM.DecapsulateSharedSecret(EncapsulatedSharedSecretForEPK, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPrivateKey());
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    if (SharedSecretForEPK == IntPtr.Zero)
                                                    {
                                                        ResultString = "Error: The server faced some system constraint.. Try again later..";
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            ActualPrivateKey = SodiumSecretBox.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            if (ActualPrivateKey.Length != 32)
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                        }
                                                        catch
                                                        {
                                                            AbleToDecrypt = false;
                                                        }
                                                        if (AbleToDecrypt)
                                                        {
                                                            ActualPrivateKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero, ActualPrivateKey.Length);
                                                            if (IsZero)
                                                            {
                                                                ResultString = "Error: Unable to decrypt properly due to system constraint. Try again later..";
                                                            }
                                                            else
                                                            {
                                                                Marshal.Copy(ActualPrivateKey, 0, ActualPrivateKeyIntPtr, ActualPrivateKey.Length);
                                                                SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(ActualPrivateKeyIntPtr);
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].X25519EncryptionSecretKey = ActualPrivateKeyIntPtr;
                                                                ResultString = "Success: Able to import sealedbox private key";
                                                            }
                                                            SodiumSecureMemory.SecureClearBytes(ActualPrivateKey);
                                                        }
                                                        else
                                                        {
                                                            ResultString = "Error: Failed to decrypt encrypted sealedbox private key or it's not sealedbox private key";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt temporary shared secret that encrypt sealedbox private key";
                                                }
                                            }
                                            else
                                            {
                                                EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.EncryptedPrivateKeyB64);
                                                Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                Array.Copy(EncryptedPrivateKey, 0, EncapsulatedSharedSecretForEPK, 0, EncapsulatedSharedSecretForEPK.LongLength);
                                                Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                try
                                                {
                                                    SharedSecretForEPK = SodiumKEM.DecapsulateSharedSecret(EncapsulatedSharedSecretForEPK, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPrivateKey());
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    if (SharedSecretForEPK == IntPtr.Zero)
                                                    {
                                                        ResultString = "Error: The server faced some system constraint.. Try again later..";
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            ActualPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            if (ActualPrivateKey.Length != 32)
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                        }
                                                        catch
                                                        {
                                                            AbleToDecrypt = false;
                                                        }
                                                        if (AbleToDecrypt)
                                                        {
                                                            ActualPrivateKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero, ActualPrivateKey.Length);
                                                            if (IsZero)
                                                            {
                                                                ResultString = "Error: Unable to decrypt properly due to system constraint. Try again later..";
                                                            }
                                                            else
                                                            {
                                                                Marshal.Copy(ActualPrivateKey, 0, ActualPrivateKeyIntPtr, ActualPrivateKey.Length);
                                                                SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(ActualPrivateKeyIntPtr);
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].X25519EncryptionSecretKey = ActualPrivateKeyIntPtr;
                                                                ResultString = "Success: Able to import sealedbox private key";
                                                            }
                                                            SodiumSecureMemory.SecureClearBytes(ActualPrivateKey);
                                                        }
                                                        else
                                                        {
                                                            ResultString = "Error: Failed to decrypt encrypted sealedbox private key or it's not sealedbox private key";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt temporary shared secret that encrypt sealedbox private key";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            /*
                                            Byte[] EncryptedPrivateKey = new Byte[] { };
                                            Byte[] EncapsulatedSharedSecretForEPK = new Byte[SodiumKEM.GetCipherTextBytesLength()];
                                            IntPtr SharedSecretForEPK = IntPtr.Zero;
                                            Byte[] TrimmedEncryptedPrivateKey = new Byte[] { };
                                            Byte[] Nonce = new Byte[] { };
                                            Byte[] ActualPrivateKey = new Byte[] { };
                                            Boolean IsZero = true;
                                            IntPtr ActualPrivateKeyIntPtr = IntPtr.Zero;
                                            if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                            {
                                                EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.EncryptedPrivateKeyB64);
                                                Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                Array.Copy(EncryptedPrivateKey, 0, EncapsulatedSharedSecretForEPK, 0, EncapsulatedSharedSecretForEPK.LongLength);
                                                Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                try
                                                {
                                                    SharedSecretForEPK = SodiumKEM.DecapsulateSharedSecret(EncapsulatedSharedSecretForEPK, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPrivateKey());
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    if (SharedSecretForEPK == IntPtr.Zero)
                                                    {
                                                        ResultString = "Error: The server faced some system constraint.. Try again later..";
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            ActualPrivateKey = SodiumSecretBox.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            if (ActualPrivateKey.Length != 32)
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                        }
                                                        catch
                                                        {
                                                            AbleToDecrypt = false;
                                                        }
                                                        if (AbleToDecrypt)
                                                        {
                                                            ActualPrivateKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero, ActualPrivateKey.Length);
                                                            if (IsZero)
                                                            {
                                                                ResultString = "Error: Unable to decrypt properly due to system constraint. Try again later..";
                                                            }
                                                            else
                                                            {
                                                                Marshal.Copy(ActualPrivateKey, 0, ActualPrivateKeyIntPtr, ActualPrivateKey.Length);
                                                                SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(ActualPrivateKeyIntPtr);
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey = ActualPrivateKeyIntPtr;
                                                                ResultString = "Success: Able to import kem private key";
                                                            }
                                                            SodiumSecureMemory.SecureClearBytes(ActualPrivateKey);
                                                        }
                                                        else
                                                        {
                                                            ResultString = "Error: Failed to decrypt encrypted kem private key or it's not kem private key";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt temporary shared secret that encrypt kem private key";
                                                }
                                            }
                                            else
                                            {
                                                EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.EncryptedPrivateKeyB64);
                                                Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                TrimmedEncryptedPrivateKey = new Byte[EncryptedPrivateKey.LongLength - EncapsulatedSharedSecretForEPK.LongLength - Nonce.LongLength];
                                                Array.Copy(EncryptedPrivateKey, 0, EncapsulatedSharedSecretForEPK, 0, EncapsulatedSharedSecretForEPK.LongLength);
                                                Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength, Nonce, 0, Nonce.LongLength);
                                                Array.Copy(EncryptedPrivateKey, EncapsulatedSharedSecretForEPK.LongLength + Nonce.LongLength, TrimmedEncryptedPrivateKey, 0, TrimmedEncryptedPrivateKey.LongLength);
                                                try
                                                {
                                                    SharedSecretForEPK = SodiumKEM.DecapsulateSharedSecret(EncapsulatedSharedSecretForEPK, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPrivateKey());
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    if (SharedSecretForEPK == IntPtr.Zero)
                                                    {
                                                        ResultString = "Error: The server faced some system constraint.. Try again later..";
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            ActualPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKey, Nonce, SharedSecretForEPK, true);
                                                            if (ActualPrivateKey.Length != 32)
                                                            {
                                                                AbleToDecrypt = false;
                                                            }
                                                        }
                                                        catch
                                                        {
                                                            AbleToDecrypt = false;
                                                        }
                                                        if (AbleToDecrypt)
                                                        {
                                                            ActualPrivateKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero, ActualPrivateKey.Length);
                                                            if (IsZero)
                                                            {
                                                                ResultString = "Error: Unable to decrypt properly due to system constraint. Try again later..";
                                                            }
                                                            else
                                                            {
                                                                Marshal.Copy(ActualPrivateKey, 0, ActualPrivateKeyIntPtr, ActualPrivateKey.Length);
                                                                SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(ActualPrivateKeyIntPtr);
                                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey = ActualPrivateKeyIntPtr;
                                                                ResultString = "Success: Able to import kem private key";
                                                            }
                                                            SodiumSecureMemory.SecureClearBytes(ActualPrivateKey);
                                                        }
                                                        else
                                                        {
                                                            ResultString = "Error: Failed to decrypt encrypted kem private key or it's not kem private key";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt temporary shared secret that encrypt kem private key";
                                                }
                                            }
                                            */
                                            ResultString = "Error: KEM key import was not supported for now..";
                                        }
                                    }
                                    else 
                                    {
                                        ResultString = "Error: Can't import with KEM as ETLS had not initiated with it.";
                                    }
                                }
                                else
                                {
                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.CheckIsInvalid()==false) 
                                    {
                                        Byte[] EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.EncryptedPrivateKeyB64);
                                        Byte[] X25519PublicKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPublicKey();
                                        Byte[] ActualPrivateKey = new Byte[] { };
                                        Boolean IsZero = true;
                                        IntPtr ActualPrivateKeyIntPtr = IntPtr.Zero;
                                        Boolean AbleToDecrypt = true;
                                        if (PostDataModel.IsDSOrSealedBoxOrKEMKeyType == 0)
                                        {
                                            if (PostDataModel.IsED25519OrED448OrRSA == 0)
                                            {
                                                try
                                                {
                                                    if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                    else
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    ActualPrivateKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero, ActualPrivateKey.Length);
                                                    if (IsZero)
                                                    {
                                                        ResultString = "Error: Unable to import keys due to system constraint. Try again later..";
                                                    }
                                                    else
                                                    {
                                                        Marshal.Copy(ActualPrivateKey, 0, ActualPrivateKeyIntPtr, ActualPrivateKey.Length);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(ActualPrivateKeyIntPtr);
                                                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey = ActualPrivateKeyIntPtr;
                                                        ResultString = "Success: Able to import ED25519 private key";
                                                    }
                                                    SodiumSecureMemory.SecureClearBytes(ActualPrivateKey);
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt encrypted private key (SealedBox encrypted ED25519)";
                                                }
                                            }
                                            else if (PostDataModel.IsED25519OrED448OrRSA == 1)
                                            {
                                                try
                                                {
                                                    if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                    else
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey = ActualPrivateKey;
                                                    ResultString = "Success: Able to import ED448 private key";
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt encrypted private key (SealedBox encrypted ED448)";
                                                }
                                            }
                                            else
                                            {
                                                EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedExponentB64);
                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts = new RSACredentialsModel();
                                                try
                                                {
                                                    if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                    else
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Exponent = ActualPrivateKey;
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt encrypted exponent";
                                                }
                                                EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedModulusB64);
                                                try
                                                {
                                                    if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                    else
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Modulus = ActualPrivateKey;
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt encrypted modulus";
                                                }
                                                EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedDB64);
                                                try
                                                {
                                                    if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                    else
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.D = ActualPrivateKey;
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt encrypted D";
                                                }
                                                EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedPB64);
                                                try
                                                {
                                                    if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                    else
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.P = ActualPrivateKey;
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt encrypted P";
                                                }
                                                EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedQB64);
                                                try
                                                {
                                                    if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                    else
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Q = ActualPrivateKey;
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt encrypted Q";
                                                }
                                                EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedDPB64);
                                                try
                                                {
                                                    if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                    else
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DP = ActualPrivateKey;
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt encrypted DP";
                                                }
                                                EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedDQB64);
                                                try
                                                {
                                                    if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                    else
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DQ = ActualPrivateKey;
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt encrypted DQ";
                                                }
                                                EncryptedPrivateKey = Convert.FromBase64String(PostDataModel.MyRSAKey.EncryptedInverseQB64);
                                                try
                                                {
                                                    if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                    else
                                                    {
                                                        ActualPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                    }
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.InverseQ = ActualPrivateKey;
                                                }
                                                else
                                                {
                                                    ResultString = "Error: Unable to decrypt encrypted inverse Q";
                                                }
                                                if (ResultString.Contains("Error") == false)
                                                {
                                                    ResultString = "Success: Able to import RSA key.";
                                                }
                                            }
                                        }
                                        else if (PostDataModel.IsDSOrSealedBoxOrKEMKeyType == 1)
                                        {
                                            try
                                            {
                                                if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                {
                                                    ActualPrivateKey = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                }
                                                else
                                                {
                                                    ActualPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                }
                                            }
                                            catch
                                            {
                                                AbleToDecrypt = false;
                                            }
                                            if (AbleToDecrypt)
                                            {
                                                ActualPrivateKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero, ActualPrivateKey.Length);
                                                if (IsZero)
                                                {
                                                    ResultString = "Error: Unable to import keys due to system constraint. Try again later..";
                                                }
                                                else
                                                {
                                                    Marshal.Copy(ActualPrivateKey, 0, ActualPrivateKeyIntPtr, ActualPrivateKey.Length);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(ActualPrivateKeyIntPtr);
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].X25519EncryptionSecretKey = ActualPrivateKeyIntPtr;
                                                    ResultString = "Success: Able to import sealedbox private key";
                                                }
                                                SodiumSecureMemory.SecureClearBytes(ActualPrivateKey);
                                            }
                                            else
                                            {
                                                ResultString = "Error: Unable to decrypt encrypted private key";
                                            }
                                        }
                                        else
                                        {
                                            /*
                                            try
                                            {
                                                if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                {
                                                    ActualPrivateKey = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                }
                                                else
                                                {
                                                    ActualPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKey, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey()); ;
                                                }
                                            }
                                            catch
                                            {
                                                AbleToDecrypt = false;
                                            }
                                            if (AbleToDecrypt)
                                            {
                                                ActualPrivateKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero, ActualPrivateKey.Length);
                                                if (IsZero)
                                                {
                                                    ResultString = "Error: Unable to import keys due to system constraint. Try again later..";
                                                }
                                                else
                                                {
                                                    Marshal.Copy(ActualPrivateKey, 0, ActualPrivateKeyIntPtr, ActualPrivateKey.Length);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(ActualPrivateKeyIntPtr);
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey = ActualPrivateKeyIntPtr;
                                                    ResultString = "Success: Able to import kem private key";
                                                }
                                                SodiumSecureMemory.SecureClearBytes(ActualPrivateKey);
                                            }
                                            else
                                            {
                                                ResultString = "Error: Unable to decrypt encrypted private key";
                                            }
                                            */
                                            ResultString = "Error: KEM key import was not supported for now..";
                                        }
                                    }
                                    else 
                                    {
                                        ResultString = "Error: Can't import with ETLS's sealedbox as it's not initiated..";
                                    }
                                }
                            }
                            if (ResultString.Contains("Error") == false) 
                            {
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ValidDateTime = DateTime.UtcNow.AddHours(9);
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
                ResultString = "Error: This user does not exist..";
            }
            return ResultString;
        }

        //Use appropriate model..
        [HttpPost("SignData")]
        public String SignData(PublicKeyCryptographySignDataModel PostDataModel) 
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
                Boolean AbleToVerifySignedChallenge = true;
                Boolean AbleToVerifyShortTermSignedEDGeneralDSPublicKey = true;
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
                                Byte[] DataBytes = Convert.FromBase64String(PostDataModel.DataB64);
                                Byte[] SignedDataBytes = new Byte[] { };
                                if (PostDataModel.IsED25519OrED448OrRSA == 0)
                                {
                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero)
                                    {
                                        SignedDataBytes = SodiumPublicKeyAuth.Sign(DataBytes, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                                    }
                                    else if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP != null)
                                    {
                                        SignedDataBytes = SodiumPublicKeyAuth.Sign(DataBytes, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey());
                                    }
                                    else
                                    {
                                        ResultString = "Error: ED25519 had not been initiated or private key had not been imported for this SHSM";
                                    }
                                }
                                else if (PostDataModel.IsED25519OrED448OrRSA == 1)
                                {
                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey != null)
                                    {
                                        SignedDataBytes = SecureED448.GenerateSignatureMessage(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey, DataBytes, new Byte[] { });
                                    }
                                    else if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP != null)
                                    {
                                        SignedDataBytes = SecureED448.GenerateSignatureMessage(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP.PrivateKey, DataBytes, new Byte[] { });
                                    }
                                    else
                                    {
                                        ResultString = "Error: ED25519 had not been initiated or private key had not been imported for this SHSM";
                                    }
                                }
                                else
                                {
                                    ResultString = "Info: RSA for now only support Arweave upload data operations. If you're a cryptographer/cybersecurity expert, kindly inform me what I need to support for RSA signing operations..";
                                }
                                if (ResultString.Contains("Error") == false && ResultString.Contains("Info") == false)
                                {
                                    ResultString = Convert.ToBase64String(SignedDataBytes);
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
                    ResultString = "Error: Unable to verify signed short term digital signature public key";
                }
            }
            else
            {
                ResultString = "Error: This user does not exist..";
            }
            return ResultString;
        }

        [HttpPost("SealedBoxDecrypt")]
        public String SealedBoxDecryptData(PublicKeyCryptographyDecryptDataModel PostDataModel) 
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
                                if (PostDataModel.IsSealedBoxOrKEM)
                                {
                                    Byte[] EncryptedDataBytes = Convert.FromBase64String(PostDataModel.EncryptedDataB64);
                                    Byte[] DataBytes = new Byte[] { };
                                    Byte[] X25519PublicKey = new Byte[] { };
                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].X25519EncryptionSecretKey != IntPtr.Zero)
                                    {
                                        X25519PublicKey = SodiumPublicKeyBox.GeneratePublicKey(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].X25519EncryptionSecretKey);
                                        if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                        {
                                            DataBytes = SodiumSealedPublicKeyBox.Open(EncryptedDataBytes, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].X25519EncryptionSecretKey);
                                        }
                                        else
                                        {
                                            DataBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedDataBytes, X25519PublicKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].X25519EncryptionSecretKey);
                                        }
                                    }
                                    else
                                    {
                                        ResultString = "Error: You had not import X25519 private key for this SHSM.";
                                    }
                                    if (ResultString.Contains("Error") == false)
                                    {
                                        ResultString = Convert.ToBase64String(DataBytes);
                                    }
                                }
                                else
                                {
                                    ResultString = "Error: Kindly use another API endpoint as this's for Sealedbox..";
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
                    ResultString = "Error: Unable to verify short term signed digital signature public key";
                }
            }
            else
            {
                ResultString = "Error: This user does not exist..";
            }
            return ResultString;
        }

        /*
        [HttpPost("KEMDecrypt")]
        public String KEMDecryptData(PublicKeyCryptographyDecryptDataModel PostDataModel) 
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
                                if (PostDataModel.IsSealedBoxOrKEM == false)
                                {
                                    if (PostDataModel.KEMEncryptionPKB64.CompareTo("") != 0)
                                    {
                                        Byte[] KEMPKBytes = Convert.FromBase64String(PostDataModel.KEMEncryptionPKB64);
                                        Byte[] Nonce = new Byte[] { };
                                        Byte[] EncryptedDataBytes = Convert.FromBase64String(PostDataModel.EncryptedDataB64);
                                        Byte[] EncapsulatedSharedSecret = new Byte[SodiumKEM.GetCipherTextBytesLength()];
                                        IntPtr DecapsulatedSharedSecret = IntPtr.Zero;
                                        Byte[] TrimmedEncryptedDataBytes = new Byte[EncryptedDataBytes.LongLength - EncapsulatedSharedSecret.LongLength];
                                        Byte[] DataBytes = new Byte[] { };
                                        if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey == IntPtr.Zero)
                                        {
                                            ResultString = "Error: You had not import KEM private key for this SHSM.";
                                        }
                                        else
                                        {
                                            Nonce = SodiumGenericHash.ComputeHash(64, KEMPKBytes);
                                            Array.Copy(EncryptedDataBytes, 0, EncapsulatedSharedSecret, 0, EncapsulatedSharedSecret.LongLength);
                                            Array.Copy(EncryptedDataBytes, EncapsulatedSharedSecret.LongLength, TrimmedEncryptedDataBytes, 0, TrimmedEncryptedDataBytes.LongLength);
                                            DecapsulatedSharedSecret = SodiumKEM.DecapsulateSharedSecret(EncapsulatedSharedSecret, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey);
                                            if (DecapsulatedSharedSecret == IntPtr.Zero)
                                            {
                                                ResultString = "Error: System faced some constraints. Try again later..";
                                            }
                                            else
                                            {
                                                if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                                {
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    DataBytes = SodiumSecretBox.Open(TrimmedEncryptedDataBytes, Nonce, DecapsulatedSharedSecret,true);
                                                }
                                                else
                                                {
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    DataBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedDataBytes, Nonce, DecapsulatedSharedSecret,true);
                                                }
                                                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(DecapsulatedSharedSecret);
                                                SodiumGuardedHeapAllocation.Sodium_Free(DecapsulatedSharedSecret);
                                            }
                                        }
                                        if (ResultString.Contains("Error") == false)
                                        {
                                            ResultString = Convert.ToBase64String(DataBytes);
                                        }
                                    }
                                    else
                                    {
                                        ResultString = "Error: Unable to decrypt if there's no given public key..";
                                    }
                                }
                                else
                                {
                                    ResultString = "Error: Kindly use another API endpoint as this's for KEM..";
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
                    ResultString = "Error: Unable to verify short term signed digital signature public key";
                }
            }
            else
            {
                ResultString = "Error: This user does not exist..";
            }
            return ResultString;
        }
        */

        [HttpGet("ExtendDuration")]
        public String ExtendDuration(String User_ID, String SignedChallengeB64)
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
                            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero ||
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey != null ||
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP != null ||
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP != null ||
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts != null ||
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey != IntPtr.Zero)
                            {
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ValidDateTime = DateTime.UtcNow.AddHours(12);
                                ResultString = "Success: Extended the duration for public key cryptography..";
                            }
                            else
                            {
                                ResultString = "Error: You had not initialized any private keys or key pairs for public key cryptography..";
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

        /*
        [HttpPost("ExportDSAKeys")]
        public PublicKeysExportModel ExportDSAKeys(PublicKeyCryptographyExportDSAPostModel MyPostModel)
        {
            PublicKeysExportModel MyModel = new PublicKeysExportModel();
            MyModel.EncryptedRSAKey = new EncryptedRSACredentialsModel();
            Boolean IsUserExist = RegisteredUsersHelper.users.ContainsKey(MyPostModel.User_ID);
            if (IsUserExist)
            {
                int SpecificIndex = RegisteredUsersHelper.usersindices[MyPostModel.User_ID];
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
                String UserLongTermEDAuthPublicKeyB64 = UserInfo.Auth_PK;
                Byte[] UserLongTermEDAuthPublicKey = Convert.FromBase64String(UserLongTermEDAuthPublicKeyB64);
                String UserLongTermEDSigningPublicKeyB64 = UserInfo.Sign_PK;
                Byte[] UserLongTermEDSigningPublicKey = Convert.FromBase64String(UserLongTermEDSigningPublicKeyB64);
                String UserShortTermSignedEDGeneralDigitalSignaturePublicKeyB64 = UserSubSignedPKInfo.SignedDigitalSignaturePublicKeyB64;
                Byte[] UserShortTermSignedEDGeneralDigitalSignaturePublicKey = Convert.FromBase64String(UserShortTermSignedEDGeneralDigitalSignaturePublicKeyB64);
                Byte[] UserShortTermEDGeneralDigitalSignaturePublicKey = new Byte[] { };
                Byte[] SignedChallenge = Convert.FromBase64String(MyPostModel.SignedChallengeB64);
                Byte[] Challenge = new Byte[] { };
                Boolean AbleToVerifyShortTermSignedEDGeneralDSPublicKey = true;
                Boolean AbleToVerifySignedChallenge = true;
                Boolean AbleToVerifyShortTermSignedPublicKeys = true;
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
                        MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = MyPostModel.User_ID;
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
                            MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = MyPostModel.User_ID;
                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                            MySQLGeneralQuery.Prepare();
                            DatabaseDateTime = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                            MyTimeSpan = CurrentDateTime.Subtract(DatabaseDateTime);
                            if (MyTimeSpan.TotalMinutes <= 8)
                            {
                                MySQLGeneralQuery = new MySqlCommand();
                                MySQLGeneralQuery.CommandText = "DELETE FROM `User_Challenge` WHERE `User_ID`=@User_ID";
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = MyPostModel.User_ID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                MySQLGeneralQuery.ExecuteNonQuery();
                                Byte[] SignedEncryptionPK = Convert.FromBase64String(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RegisteredUser.UserSignedPublicKeys.SubSignedPublicKeysB64[0]);
                                Byte[] EncryptionPK = new Byte[] { };
                                try
                                {
                                    if (UserShortTermEDGeneralDigitalSignaturePublicKey.Length == 32)
                                    {
                                        EncryptionPK = SodiumPublicKeyAuth.Verify(SignedEncryptionPK, UserShortTermEDGeneralDigitalSignaturePublicKey);
                                    }
                                    else
                                    {
                                        EncryptionPK = SecureED448.GetMessageFromSignatureMessage(UserShortTermEDGeneralDigitalSignaturePublicKey, SignedEncryptionPK, new Byte[] { });
                                    }
                                }
                                catch
                                {
                                    AbleToVerifyShortTermSignedPublicKeys = false;
                                }
                                if (AbleToVerifyShortTermSignedPublicKeys)
                                {
                                    EncryptedRSACredentialsModel MyEncryptedRSAKey = new EncryptedRSACredentialsModel();
                                    Byte[] RawPrivateKey = new Byte[] { };
                                    Byte[] EncryptedPrivateKey = new Byte[] { };
                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RegisteredUser.UserSignedPublicKeys.IsKEMorSealedBox[0])
                                    {
                                        EncapsulatedSharedSecretBox MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                        Byte[] Nonce = SodiumGenericHash.ComputeHash(64, EncryptionPK);
                                        if (MyPostModel.IsED25519OrED448OrRSA == 0)
                                        {
                                            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero
                                                || SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.CheckIsInvalid()==false)
                                            {
                                                if (MyPostModel.UseXSalsa20Poly1305)
                                                {
                                                    RawPrivateKey = new Byte[64];
                                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero)
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                                                        Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey, RawPrivateKey, 0, RawPrivateKey.Length);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                                                    }
                                                    else
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey());
                                                        Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey(), RawPrivateKey, 0, RawPrivateKey.Length);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey());
                                                    }
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBox.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                }
                                                else
                                                {
                                                    RawPrivateKey = new Byte[64];
                                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero)
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                                                        Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey, RawPrivateKey, 0, RawPrivateKey.Length);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                                                    }
                                                    else
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey());
                                                        Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey(), RawPrivateKey, 0, RawPrivateKey.Length);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey());
                                                    }
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBox.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                }
                                                SodiumSecureMemory.SecureClearBytes(RawPrivateKey);
                                                EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                            }
                                            else
                                            {
                                                MyModel.StatusString = "Error: ED25519 had not been initiated for this SHSM";
                                            }
                                        }
                                        else if (MyPostModel.IsED25519OrED448OrRSA == 1)
                                        {
                                            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey != null
                                                || SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP != null)
                                            {
                                                if (MyPostModel.UseXSalsa20Poly1305)
                                                {
                                                    RawPrivateKey = new Byte[56];
                                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey != null)
                                                    {
                                                        RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey;
                                                    }
                                                    else
                                                    {
                                                        RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP.PrivateKey;
                                                    }
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBox.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                }
                                                else
                                                {
                                                    RawPrivateKey = new Byte[56];
                                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey != null)
                                                    {
                                                        RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey;
                                                    }
                                                    else
                                                    {
                                                        RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP.PrivateKey;
                                                    }
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBox.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                }
                                                SodiumSecureMemory.SecureClearBytes(RawPrivateKey);
                                                EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                            }
                                            else
                                            {
                                                MyModel.StatusString = "Error: ED448 had not been initiated for your SHSM..";
                                            }
                                        }
                                        else
                                        {
                                            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts == null)
                                            {
                                                MyModel.StatusString = "Error: RSA had not been initiated..";
                                            }
                                            else
                                            {
                                                if (MyPostModel.UseXSalsa20Poly1305)
                                                {
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Exponent;
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBox.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedExponentB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Modulus;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBox.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedModulusB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.D;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBox.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedDB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.P;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBox.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedPB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Q;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBox.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedQB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DP;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBox.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedDPB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DQ;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBox.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedDQB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.InverseQ;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBox.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedInverseQB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                }
                                                else
                                                {
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Exponent;
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedExponentB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Modulus;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedModulusB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.D;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedDB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.P;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedPB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Q;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedQB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DP;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedDPB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DQ;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedDQB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.InverseQ;
                                                    MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                                    EncryptedPrivateKey = SodiumSecretBoxXChaCha20Poly1305.Create(RawPrivateKey, Nonce, MyBox.SharedSecretIntPtr);
                                                    EncryptedPrivateKey = MyBox.CipherTextBytes.Concat(EncryptedPrivateKey).ToArray();
                                                    MyModel.EncryptedRSAKey.EncryptedInverseQB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                                }
                                            }
                                        }
                                        if (MyModel.StatusString.Contains("Error") == false)
                                        {
                                            MyModel.StatusString = "Success: Able to export ED25519 or ED448 or RSA private key";
                                            if (MyPostModel.IsED25519OrED448OrRSA == 0 || MyPostModel.IsED25519OrED448OrRSA == 1)
                                            {
                                                MyModel.EncryptedPrivateKeyB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (MyPostModel.IsED25519OrED448OrRSA == 0)
                                        {
                                            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero ||
                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.CheckIsInvalid()==false)
                                            {
                                                RawPrivateKey = new Byte[64];
                                                if (MyPostModel.UseXSalsa20Poly1305)
                                                {
                                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero)
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                                                        Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey, RawPrivateKey, 0, RawPrivateKey.Length);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                                                    }
                                                    else
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey());
                                                        Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey(), RawPrivateKey, 0, RawPrivateKey.Length);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey());
                                                    }
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBox.Create(RawPrivateKey, EncryptionPK);
                                                    SodiumSecureMemory.SecureClearBytes(RawPrivateKey);
                                                }
                                                else
                                                {
                                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero)
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                                                        Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey, RawPrivateKey, 0, RawPrivateKey.Length);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                                                    }
                                                    else
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey());
                                                        Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey(), RawPrivateKey, 0, RawPrivateKey.Length);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey());
                                                    }
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(RawPrivateKey, EncryptionPK);
                                                    SodiumSecureMemory.SecureClearBytes(RawPrivateKey);
                                                }
                                            }
                                            else
                                            {
                                                MyModel.StatusString = "Error: ED25519 had not been initiated for the user..";
                                            }
                                        }
                                        else if (MyPostModel.IsED25519OrED448OrRSA == 1)
                                        {
                                            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey != null ||
                                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP != null)
                                            {
                                                RawPrivateKey = new Byte[56];
                                                if (MyPostModel.UseXSalsa20Poly1305)
                                                {
                                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero)
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                                                        Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey, RawPrivateKey, 0, RawPrivateKey.Length);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                                                    }
                                                    else
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey());
                                                        Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey(), RawPrivateKey, 0, RawPrivateKey.Length);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey());
                                                    }
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBox.Create(RawPrivateKey, EncryptionPK);
                                                    SodiumSecureMemory.SecureClearBytes(RawPrivateKey);
                                                }
                                                else
                                                {
                                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero)
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                                                        Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey, RawPrivateKey, 0, RawPrivateKey.Length);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                                                    }
                                                    else
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey());
                                                        Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey(), RawPrivateKey, 0, RawPrivateKey.Length);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.GetPrivateKey());
                                                    }
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(RawPrivateKey, EncryptionPK);
                                                    SodiumSecureMemory.SecureClearBytes(RawPrivateKey);
                                                }
                                            }
                                            else
                                            {
                                                MyModel.StatusString = "Error: ED448 had not been initiated for this SHSM..";
                                            }
                                        }
                                        else
                                        {
                                            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts != null)
                                            {
                                                if (MyPostModel.UseXSalsa20Poly1305)
                                                {
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Exponent;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBox.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedExponentB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Modulus;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBox.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedModulusB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.D;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBox.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedDB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.P;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBox.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedPB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Q;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBox.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedQB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DP;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBox.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedDPB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DQ;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBox.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedDQB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.InverseQ;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBox.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedInverseQB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                }
                                                else
                                                {
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Exponent;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedExponentB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Modulus;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedModulusB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.D;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedDB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.P;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedPB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Q;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedQB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DP;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedDPB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DQ;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedDQB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    RawPrivateKey = SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.InverseQ;
                                                    EncryptedPrivateKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(RawPrivateKey, EncryptionPK);
                                                    MyModel.EncryptedRSAKey.EncryptedInverseQB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                }
                                                if (MyModel.StatusString.Contains("Error") == false)
                                                {
                                                    MyModel.StatusString = "Success: Able to export ED25519 or ED448 or RSA private key";
                                                    if (MyPostModel.IsED25519OrED448OrRSA == 0 || MyPostModel.IsED25519OrED448OrRSA == 1)
                                                    {
                                                        MyModel.EncryptedPrivateKeyB64 = Convert.ToBase64String(EncryptedPrivateKey);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                MyModel.StatusString = "Error: RSA had not been initiated for this SHSM..";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MyModel.StatusString = "Error: Unable to verify submitted signed encryption public key..";
                                }
                            }
                            else
                            {
                                MySQLGeneralQuery = new MySqlCommand();
                                MySQLGeneralQuery.CommandText = "DELETE FROM `User_Challenge` WHERE `User_ID`=@User_ID";
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = MyPostModel.User_ID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                MySQLGeneralQuery.ExecuteNonQuery();
                                MyModel.StatusString = "Error: Deleting the generated challenge that expired..";
                            }
                        }
                        else
                        {
                            MyModel.StatusString = "Error: This verified challenge does not exist";
                        }
                        myMyOwnMySQLConnection.MyMySQLConnection.Close();
                        myMyOwnMySQLConnection.ClearConnectionString();
                    }
                    else
                    {
                        MyModel.StatusString = "Error: Unable to verify signed challenge";
                    }
                }
                else
                {
                    MyModel.StatusString = "Error: Unable to verify user signed short term general digital signature public key";
                }
            }
            else
            {
                MyModel.StatusString = "Error: This user does not exist..";
            }
            Console.WriteLine(JsonConvert.SerializeObject(MyModel));
            return MyModel;
        }
        */
    }
}
