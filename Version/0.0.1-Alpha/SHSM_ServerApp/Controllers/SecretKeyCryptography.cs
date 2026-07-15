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
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;

namespace SHSM_ServerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretKeyCryptography : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection;

        //Initialize Encryption and MAC Key
        //AES..
        //Stream Cipher
        //MAC..
        //Export
        //Extend duration..
        //Delete..

        [HttpGet]
        public String InitializeSecretKeysPair(String User_ID, String SignedChallengeB64) 
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
                                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption == IntPtr.Zero) 
                                {
                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption = SodiumRNG.GetRandomBytesIntPtr(32);
                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC = SodiumRNG.GetRandomBytesIntPtr(32);
                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ValidDateTime = DateTime.UtcNow.AddHours(9);
                                    ResultString = "Success: Created secret keys in SHSM";
                                }
                                else 
                                {
                                    ResultString = "Error: You can consider to extend the secret keys duration in SHSM or delete and reinitialize it";
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
            }
            else
            {
                ResultString = "Error: This user does not exist..";
            }
            return ResultString;
        }

        [HttpPost("ImportKeys")]
        public String ImportKeys(SecretKeyCryptographyImportKeysDataModel PostDataModel)
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
                                Byte[] EncryptedMACSecretKey = Convert.FromBase64String(PostDataModel.EncryptedMACSecretKeyB64);
                                Byte[] EncryptedEncryptionSecretKey = Convert.FromBase64String(PostDataModel.EncryptedEncryptionSecretKeyB64);
                                if (PostDataModel.IsKEMOrSealedBox)
                                {
                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.CheckIsInvalid()==false)
                                    {
                                        Byte[] EncapsulatedMACSecretKey = new Byte[SodiumKEM.GetCipherTextBytesLength()];
                                        Byte[] ActualEncryptedMACSecretKey = new Byte[EncryptedMACSecretKey.LongLength - EncapsulatedMACSecretKey.LongLength];
                                        Byte[] EncapsulatedEncryptionSecretKey = new Byte[SodiumKEM.GetCipherTextBytesLength()];
                                        Byte[] ActualEncryptedEncryptionSecretKey = new Byte[EncryptedEncryptionSecretKey.LongLength - EncapsulatedEncryptionSecretKey.LongLength];
                                        Byte[] Nonce = new Byte[] { };
                                        Boolean AbleToDecrypt = true;
                                        IntPtr TemporaryMACSecretKey = IntPtr.Zero;
                                        IntPtr TemporaryEncryptionSecretKey = IntPtr.Zero;
                                        Byte[] ActualMACSecretKey = new Byte[] { };
                                        Byte[] ActualEncryptionSecretKey = new Byte[] { };
                                        IntPtr ActualMACSecretKeyIntPtr = IntPtr.Zero;
                                        IntPtr ActualEncryptionSecretKeyIntPtr = IntPtr.Zero;
                                        Boolean IsZero = true;
                                        Boolean IsZero2 = true;
                                        Array.Copy(EncryptedMACSecretKey, 0, EncapsulatedMACSecretKey, 0, EncapsulatedMACSecretKey.LongLength);
                                        Array.Copy(EncryptedMACSecretKey, EncapsulatedMACSecretKey.LongLength, ActualEncryptedMACSecretKey, 0, ActualEncryptedMACSecretKey.LongLength);
                                        Array.Copy(EncryptedEncryptionSecretKey, 0, EncapsulatedEncryptionSecretKey, 0, EncapsulatedEncryptionSecretKey.LongLength);
                                        Array.Copy(EncryptedEncryptionSecretKey, EncapsulatedEncryptionSecretKey.LongLength, ActualEncryptedEncryptionSecretKey, 0, ActualEncryptedEncryptionSecretKey.LongLength);
                                        try
                                        {
                                            TemporaryMACSecretKey = SodiumKEM.DecapsulateSharedSecret(EncapsulatedMACSecretKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPrivateKey());
                                            TemporaryEncryptionSecretKey = SodiumKEM.DecapsulateSharedSecret(EncapsulatedEncryptionSecretKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPrivateKey());
                                        }
                                        catch
                                        {
                                            AbleToDecrypt = false;
                                        }
                                        if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                        {
                                            Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPublicKey());
                                            if (AbleToDecrypt)
                                            {
                                                AbleToDecrypt = true;
                                                if (TemporaryMACSecretKey == IntPtr.Zero)
                                                {
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryMACSecretKey);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(TemporaryMACSecretKey);
                                                    ResultString = "Error: Unable to import due to system constraint. Try again later..";
                                                }
                                                if (TemporaryEncryptionSecretKey == IntPtr.Zero)
                                                {
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryEncryptionSecretKey);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(TemporaryEncryptionSecretKey);
                                                    ResultString = "Error: Unable to import due to system constraint. Try again later..";
                                                }
                                                try
                                                {
                                                    ActualMACSecretKey = SodiumSecretBox.Open(ActualEncryptedMACSecretKey, Nonce, TemporaryMACSecretKey);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                                    ActualEncryptionSecretKey = SodiumSecretBox.Open(ActualEncryptedEncryptionSecretKey, Nonce, TemporaryEncryptionSecretKey);
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    ActualMACSecretKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero, ActualMACSecretKey.Length);
                                                    ActualEncryptionSecretKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero2, ActualEncryptionSecretKey.Length);
                                                    if (IsZero)
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryMACSecretKey);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryEncryptionSecretKey);
                                                        SodiumGuardedHeapAllocation.Sodium_Free(TemporaryMACSecretKey);
                                                        SodiumGuardedHeapAllocation.Sodium_Free(TemporaryEncryptionSecretKey);
                                                        ResultString = "Error: Unable to import due to system constraint. Try again later..";
                                                    }
                                                    if (IsZero2)
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryMACSecretKey);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryEncryptionSecretKey);
                                                        SodiumGuardedHeapAllocation.Sodium_Free(TemporaryMACSecretKey);
                                                        SodiumGuardedHeapAllocation.Sodium_Free(TemporaryEncryptionSecretKey);
                                                        ResultString = "Error: Unable to import due to system constraint. Try again later..";
                                                    }
                                                    Marshal.Copy(ActualMACSecretKey, 0, ActualMACSecretKeyIntPtr, ActualMACSecretKey.Length);
                                                    Marshal.Copy(ActualEncryptionSecretKey, 0, ActualEncryptionSecretKeyIntPtr, ActualEncryptionSecretKey.Length);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(ActualMACSecretKeyIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(ActualEncryptionSecretKeyIntPtr);
                                                    SodiumSecureMemory.SecureClearBytes(ActualMACSecretKey);
                                                    SodiumSecureMemory.SecureClearBytes(ActualEncryptionSecretKey);
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC = ActualMACSecretKeyIntPtr;
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption = ActualEncryptionSecretKeyIntPtr;
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ValidDateTime = DateTime.UtcNow.AddHours(9);
                                                    ResultString = "Success: Able to import secret keys..";
                                                }
                                                else
                                                {
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryMACSecretKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryEncryptionSecretKey);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(TemporaryMACSecretKey);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(TemporaryEncryptionSecretKey);
                                                    ResultString = "Error: Unable to import because decryption failed..";
                                                }
                                            }
                                            else
                                            {
                                                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryMACSecretKey);
                                                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryEncryptionSecretKey);
                                                SodiumGuardedHeapAllocation.Sodium_Free(TemporaryMACSecretKey);
                                                SodiumGuardedHeapAllocation.Sodium_Free(TemporaryEncryptionSecretKey);
                                                ResultString = "Error: Unable to import because decryption failed..";
                                            }
                                        }
                                        else
                                        {
                                            Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.GetPublicKey());
                                            if (AbleToDecrypt)
                                            {
                                                AbleToDecrypt = true;
                                                if (TemporaryMACSecretKey == IntPtr.Zero)
                                                {
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryMACSecretKey);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(TemporaryMACSecretKey);
                                                    ResultString = "Error: Unable to import due to system constraint. Try again later..";
                                                }
                                                if (TemporaryEncryptionSecretKey == IntPtr.Zero)
                                                {
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryEncryptionSecretKey);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(TemporaryEncryptionSecretKey);
                                                    ResultString = "Error: Unable to import due to system constraint. Try again later..";
                                                }
                                                try
                                                {
                                                    ActualMACSecretKey = SodiumSecretBoxXChaCha20Poly1305.Open(ActualEncryptedMACSecretKey, Nonce, TemporaryMACSecretKey);
                                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                                    ActualEncryptionSecretKey = SodiumSecretBoxXChaCha20Poly1305.Open(ActualEncryptedEncryptionSecretKey, Nonce, TemporaryEncryptionSecretKey);
                                                }
                                                catch
                                                {
                                                    AbleToDecrypt = false;
                                                }
                                                if (AbleToDecrypt)
                                                {
                                                    ActualMACSecretKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero, ActualMACSecretKey.Length);
                                                    ActualEncryptionSecretKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero2, ActualEncryptionSecretKey.Length);
                                                    if (IsZero)
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryMACSecretKey);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryEncryptionSecretKey);
                                                        SodiumGuardedHeapAllocation.Sodium_Free(TemporaryMACSecretKey);
                                                        SodiumGuardedHeapAllocation.Sodium_Free(TemporaryEncryptionSecretKey);
                                                        ResultString = "Error: Unable to import due to system constraint. Try again later..";
                                                    }
                                                    if (IsZero2)
                                                    {
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryMACSecretKey);
                                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryEncryptionSecretKey);
                                                        SodiumGuardedHeapAllocation.Sodium_Free(TemporaryMACSecretKey);
                                                        SodiumGuardedHeapAllocation.Sodium_Free(TemporaryEncryptionSecretKey);
                                                        ResultString = "Error: Unable to import due to system constraint. Try again later..";
                                                    }
                                                    Marshal.Copy(ActualMACSecretKey, 0, ActualMACSecretKeyIntPtr, ActualMACSecretKey.Length);
                                                    Marshal.Copy(ActualEncryptionSecretKey, 0, ActualEncryptionSecretKeyIntPtr, ActualEncryptionSecretKey.Length);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(ActualMACSecretKeyIntPtr);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(ActualEncryptionSecretKeyIntPtr);
                                                    SodiumSecureMemory.SecureClearBytes(ActualMACSecretKey);
                                                    SodiumSecureMemory.SecureClearBytes(ActualEncryptionSecretKey);
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC = ActualMACSecretKeyIntPtr;
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption = ActualEncryptionSecretKeyIntPtr;
                                                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ValidDateTime = DateTime.UtcNow.AddHours(9);
                                                    ResultString = "Success: Able to import secret keys..";
                                                }
                                                else
                                                {
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryMACSecretKey);
                                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryEncryptionSecretKey);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(TemporaryMACSecretKey);
                                                    SodiumGuardedHeapAllocation.Sodium_Free(TemporaryEncryptionSecretKey);
                                                    ResultString = "Error: Unable to import because decryption failed..";
                                                }
                                            }
                                            else
                                            {
                                                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryMACSecretKey);
                                                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(TemporaryEncryptionSecretKey);
                                                SodiumGuardedHeapAllocation.Sodium_Free(TemporaryMACSecretKey);
                                                SodiumGuardedHeapAllocation.Sodium_Free(TemporaryEncryptionSecretKey);
                                                ResultString = "Error: Unable to import because decryption failed..";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ResultString = "Error: ETLS had not been initiated with KEM..";
                                    }
                                }
                                else
                                {
                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.CheckIsInvalid()==false &&
                                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MACX25519KeyPair.CheckIsInvalid()==false)
                                    {
                                        Byte[] MACSecretKey = new Byte[] { };
                                        Byte[] EncryptionSecretKey = new Byte[] { };
                                        if (PostDataModel.IsXSalsa20Poly1305OrXChaCha20Poly1305)
                                        {
                                            MACSecretKey = SodiumSealedPublicKeyBox.Open(EncryptedMACSecretKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MACX25519KeyPair.GetPublicKey(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MACX25519KeyPair.GetPrivateKey());
                                            EncryptionSecretKey = SodiumSealedPublicKeyBox.Open(EncryptedEncryptionSecretKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPublicKey(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey());
                                        }
                                        else
                                        {
                                            MACSecretKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedMACSecretKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MACX25519KeyPair.GetPublicKey(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MACX25519KeyPair.GetPrivateKey());
                                            EncryptionSecretKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedEncryptionSecretKey, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPublicKey(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.GetPrivateKey());
                                        }
                                        Boolean IsZero = true;
                                        Boolean IsZero2 = true;
                                        IntPtr MACSecretKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero, MACSecretKey.Length);
                                        IntPtr EncryptionSecretKeyIntPtr = SodiumGuardedHeapAllocation.Sodium_Malloc(ref IsZero2, EncryptionSecretKey.Length);
                                        if (IsZero || IsZero2)
                                        {
                                            SodiumSecureMemory.SecureClearBytes(MACSecretKey);
                                            SodiumSecureMemory.SecureClearBytes(EncryptionSecretKey);
                                            ResultString = "Error: Unable to import due to system constraint. Try again later..";
                                        }
                                        else
                                        {
                                            Marshal.Copy(MACSecretKey, 0, MACSecretKeyIntPtr, MACSecretKey.Length);
                                            Marshal.Copy(EncryptionSecretKey, 0, EncryptionSecretKeyIntPtr, EncryptionSecretKey.Length);
                                            SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(MACSecretKeyIntPtr);
                                            SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(EncryptionSecretKeyIntPtr);
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC = MACSecretKeyIntPtr;
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption = EncryptionSecretKeyIntPtr;
                                            SodiumSecureMemory.SecureClearBytes(MACSecretKey);
                                            SodiumSecureMemory.SecureClearBytes(EncryptionSecretKey);
                                            SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ValidDateTime = DateTime.UtcNow.AddHours(9);
                                            ResultString = "Success: Able to import secret keys..";
                                        }
                                    }
                                    else
                                    {
                                        ResultString = "Error: ETLS had not been initiated with SealedBox";
                                    }
                                }
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

        [HttpPost]
        public String EncryptData(SecretKeyCryptographyDataModel PostDataModel) 
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
                                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption != IntPtr.Zero) 
                                {
                                    Byte[] DataBytes = Convert.FromBase64String(PostDataModel.Base64DataOrCipherText);
                                    Byte[] AdditionalData = new Byte[] { };
                                    if (PostDataModel.AdditionalDataB64.CompareTo("") != 0)
                                    {
                                        AdditionalData = Convert.FromBase64String(PostDataModel.AdditionalDataB64);
                                    }
                                    Byte[] Nonce = new Byte[] { };
                                    Byte[] CipherText = new Byte[] { };
                                    Byte[] MACBytes = new Byte[] { };
                                    if (PostDataModel.AESAlgorithmIndex != -1)
                                    {
                                        if (SodiumSecretAeadAES256GCM.IsAES256GCMAvailable())
                                        {
                                            //AES256GCM
                                            if (PostDataModel.AESAlgorithmIndex == 0)
                                            {
                                                Nonce = SodiumSecretAeadAES256GCM.GeneratePublicNonce();
                                                CipherText = SodiumSecretAeadAES256GCM.Encrypt(DataBytes, Nonce, IntPtr.Zero, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption, AdditionalData);
                                            }
                                            //AEGIS256
                                            else if (PostDataModel.AESAlgorithmIndex == 1)
                                            {
                                                Nonce = SodiumSecretAeadAEGIS256.GeneratePublicNonce();
                                                CipherText = SodiumSecretAeadAEGIS256.Encrypt(DataBytes, Nonce, IntPtr.Zero, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption, AdditionalData);
                                            }
                                            //AEGIS128L
                                            else
                                            {
                                                Nonce = SodiumSecretAeadAEGIS128L.GeneratePublicNonce();
                                                CipherText = SodiumSecretAeadAEGIS128L.Encrypt(DataBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption, IntPtr.Zero, AdditionalData);
                                            }
                                            CipherText = Nonce.Concat(CipherText).ToArray();
                                            ResultString = Convert.ToBase64String(CipherText);
                                        }
                                        else
                                        {
                                            ResultString = "Error: This VPS or device does not support secure AES.. Aborting..";
                                        }
                                    }
                                    else
                                    {
                                        ResultString = "Error: This endpoint only supports secure AES";
                                    }
                                }
                                else 
                                {
                                    ResultString = "Error: There're no existing secret keys in SHSM";
                                }
                                SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                return ResultString;
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

        [HttpPost("AEADEncrypt")]
        public String AEADEncryptData(SecretKeyCryptographyAEADDataModel PostDataModel)
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
                                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption != IntPtr.Zero)
                                {
                                    Byte[] DataBytes = Convert.FromBase64String(PostDataModel.Base64DataOrCipherText);
                                    Byte[] AdditionalData = null;
                                    if (PostDataModel.AdditionalDataB64.CompareTo("") != 0)
                                    {
                                        AdditionalData = Convert.FromBase64String(PostDataModel.AdditionalDataB64);
                                    }
                                    Byte[] Nonce = new Byte[] { };
                                    Byte[] CipherText = new Byte[] { };
                                    Byte[] MACBytes = new Byte[] { };
                                    if (PostDataModel.AEADAlgorithmIndex != -1)
                                    {
                                        //XChaCha20Poly1305IETF
                                        //ChaCha20Poly1305IETF
                                        //ChaCha20Poly1305
                                        if (PostDataModel.AEADAlgorithmIndex == 0)
                                        {
                                            Nonce = SodiumSecretAeadXChaCha20Poly1305IETF.GeneratePublicNonce();
                                            CipherText = SodiumSecretAeadXChaCha20Poly1305IETF.Encrypt(DataBytes, Nonce, IntPtr.Zero, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption, AdditionalData);
                                        }
                                        else if (PostDataModel.AEADAlgorithmIndex == 1)
                                        {
                                            Nonce = SodiumSecretAeadChaCha20Poly1305IETF.GeneratePublicNonce();
                                            CipherText = SodiumSecretAeadChaCha20Poly1305IETF.Encrypt(DataBytes, Nonce, IntPtr.Zero, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption, AdditionalData);
                                        }
                                        else
                                        {
                                            Nonce = SodiumSecretAeadChaCha20Poly1305.GeneratePublicNonce();
                                            CipherText = SodiumSecretAeadChaCha20Poly1305.Encrypt(DataBytes, Nonce, IntPtr.Zero, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption, AdditionalData);
                                        }
                                        CipherText = Nonce.Concat(CipherText).ToArray();
                                        ResultString = Convert.ToBase64String(CipherText);
                                    }
                                    else 
                                    {
                                        ResultString = "Error: This endpoint only supports AEAD";
                                    }
                                }
                                else
                                {
                                    ResultString = "Error: There're no existing secret keys in SHSM";
                                }
                                SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                return ResultString;
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

        [HttpPost("StreamCipherEncrypt")]
        public String StreamCipherEncryptData(SecretKeyCryptographyStreamCipherDataModel PostDataModel)
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
                                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption != IntPtr.Zero)
                                {
                                    Byte[] DataBytes = Convert.FromBase64String(PostDataModel.Base64DataOrCipherText);
                                    Byte[] Nonce = new Byte[] { };
                                    Byte[] CipherText = new Byte[] { };
                                    Byte[] MACBytes = new Byte[] { };
                                    if (PostDataModel.StreamCipherAlgorithmIndex != -1) 
                                    {
                                        //XChaCha20, XSalsa20, ChaCha20, ChaCha20IETF, Salsa20, Salsa12, Salsa8
                                        if (PostDataModel.StreamCipherAlgorithmIndex == 0)
                                        {
                                            Nonce = SodiumStreamCipherXChaCha20.GenerateXChaCha20Nonce();
                                            CipherText = SodiumStreamCipherXChaCha20.XChaCha20Encrypt(DataBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                        }
                                        else if (PostDataModel.StreamCipherAlgorithmIndex == 1)
                                        {
                                            Nonce = SodiumStreamCipherXSalsa20.GenerateXSalsa20Nonce();
                                            CipherText = SodiumStreamCipherXSalsa20.XSalsa20Encrypt(DataBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                        }
                                        else if (PostDataModel.StreamCipherAlgorithmIndex == 2)
                                        {
                                            Nonce = SodiumStreamCipherChaCha20.GenerateChaCha20Nonce();
                                            CipherText = SodiumStreamCipherChaCha20.ChaCha20Encrypt(DataBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                        }
                                        else if (PostDataModel.StreamCipherAlgorithmIndex == 3)
                                        {
                                            Nonce = SodiumStreamCipherChaCha20.GenerateChaCha20IETFNonce();
                                            CipherText = SodiumStreamCipherChaCha20.ChaCha20IETFEncrypt(DataBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                        }
                                        else if (PostDataModel.StreamCipherAlgorithmIndex == 4)
                                        {
                                            Nonce = SodiumStreamCipherSalsa20.GenerateSalsa20Nonce();
                                            CipherText = SodiumStreamCipherSalsa20.Salsa20Encrypt(DataBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                        }
                                        else if (PostDataModel.StreamCipherAlgorithmIndex == 5)
                                        {
                                            Nonce = SodiumStreamCipherSalsa20128.GenerateSalsa20Nonce();
                                            CipherText = SodiumStreamCipherSalsa20128.Salsa2012RoundsEncrypt(DataBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                        }
                                        else
                                        {
                                            Nonce = SodiumStreamCipherSalsa20128.GenerateSalsa20Nonce();
                                            CipherText = SodiumStreamCipherSalsa20128.Salsa208RoundsEncrypt(DataBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                        }
                                        CipherText = Nonce.Concat(CipherText).ToArray();
                                        if (PostDataModel.MACAlgorithmIndex != -1)
                                        {
                                            //HMACSHA512256,HMACSHA512,HMACSHA256,Poly1305
                                            if (PostDataModel.MACAlgorithmIndex == 0)
                                            {
                                                MACBytes = SodiumHMACSHA512256.ComputeMAC(CipherText, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                            }
                                            else if (PostDataModel.MACAlgorithmIndex == 1)
                                            {
                                                MACBytes = SodiumHMACSHA512.ComputeMAC(CipherText, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                            }
                                            else if (PostDataModel.MACAlgorithmIndex == 2)
                                            {
                                                MACBytes = SodiumHMACSHA256.ComputeMAC(CipherText, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                            }
                                            else
                                            {
                                                MACBytes = SodiumOneTimeAuth.ComputePoly1305MAC(CipherText, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                            }
                                            CipherText = MACBytes.Concat(CipherText).ToArray();
                                            ResultString = Convert.ToBase64String(CipherText);
                                        }
                                        else
                                        {
                                            ResultString = "Error: Stream Cipher algorithms must pair with one of the supported MAC algorithms to prevent cipher text tampering..";
                                        }
                                    }
                                    else 
                                    {
                                        ResultString = "Error: This endpoint only supports Stream Cipher";
                                    }
                                }
                                else
                                {
                                    ResultString = "Error: There're no existing secret keys in SHSM";
                                }
                                SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                return ResultString;
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

        [HttpPost("Decrypt")]
        public String DecryptData(SecretKeyCryptographyDataModel PostDataModel)
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
                                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption != IntPtr.Zero)
                                {
                                    Byte[] CipherTextBytes = Convert.FromBase64String(PostDataModel.Base64DataOrCipherText);
                                    Byte[] AdditionalData = null;
                                    if (PostDataModel.AdditionalDataB64.CompareTo("") != 0)
                                    {
                                        AdditionalData = Convert.FromBase64String(PostDataModel.AdditionalDataB64);
                                    }
                                    Byte[] Nonce = new Byte[] { };
                                    Byte[] PlainTextBytes = new Byte[] { };
                                    Byte[] MACBytes = new Byte[] { };
                                    Byte[] TrimmedCipherTextBytes = new Byte[] { };
                                    Boolean AbleToVerifyMAC = true;
                                    if (PostDataModel.AESAlgorithmIndex != -1)
                                    {
                                        if (SodiumSecretAeadAES256GCM.IsAES256GCMAvailable())
                                        {
                                            //AES256GCM
                                            if (PostDataModel.AESAlgorithmIndex == 0)
                                            {
                                                Nonce = new Byte[SodiumSecretAeadAES256GCM.GetNoncePublicLength()];
                                                TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - Nonce.LongLength];
                                                Array.Copy(CipherTextBytes, 0, Nonce, 0, Nonce.LongLength);
                                                Array.Copy(CipherTextBytes, Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                try
                                                {
                                                    PlainTextBytes = SodiumSecretAeadAES256GCM.Decrypt(TrimmedCipherTextBytes, Nonce, IntPtr.Zero, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption, AdditionalData);
                                                }
                                                catch
                                                {
                                                    ResultString = "Error: Unable to decrypt..";
                                                }
                                            }
                                            //AEGIS256
                                            else if (PostDataModel.AESAlgorithmIndex == 1)
                                            {
                                                Nonce = new Byte[SodiumSecretAeadAEGIS256.GetNoncePublicLength()];
                                                TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                Array.Copy(CipherTextBytes, 0, Nonce, 0, Nonce.LongLength);
                                                Array.Copy(CipherTextBytes, Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                try
                                                {
                                                    PlainTextBytes = SodiumSecretAeadAEGIS256.Decrypt(TrimmedCipherTextBytes, Nonce, IntPtr.Zero, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption, AdditionalData);
                                                }
                                                catch
                                                {
                                                    ResultString = "Error: Unable to decrypt..";
                                                }
                                            }
                                            //AEGIS128L
                                            else
                                            {
                                                Nonce = new Byte[SodiumSecretAeadAEGIS128L.GetNoncePublicLength()];
                                                TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                Array.Copy(CipherTextBytes, 0, Nonce, 0, Nonce.LongLength);
                                                Array.Copy(CipherTextBytes, Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                try
                                                {
                                                    PlainTextBytes = SodiumSecretAeadAEGIS128L.Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption, IntPtr.Zero,AdditionalData);
                                                }
                                                catch
                                                {
                                                    ResultString = "Error: Unable to decrypt..";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ResultString = "Error: This VPS or device does not support secure AES.. Aborting..";
                                        }
                                        if (ResultString.Contains("Error") == false)
                                        {
                                            ResultString = Convert.ToBase64String(PlainTextBytes);
                                        }
                                    }
                                    else 
                                    {
                                        ResultString = "Error: This endpoint only support secure AES in decrypting";
                                    }
                                }
                                else
                                {
                                    ResultString = "Error: There're no existing secret keys in SHSM";
                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
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

        [HttpPost("AEADDecrypt")]
        public String AEADDecryptData(SecretKeyCryptographyAEADDataModel PostDataModel)
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
                                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption != IntPtr.Zero)
                                {
                                    Byte[] CipherTextBytes = Convert.FromBase64String(PostDataModel.Base64DataOrCipherText);
                                    Byte[] AdditionalData = null;
                                    if (PostDataModel.AdditionalDataB64.CompareTo("") != 0)
                                    {
                                        AdditionalData = Convert.FromBase64String(PostDataModel.AdditionalDataB64);
                                    }
                                    Byte[] Nonce = new Byte[] { };
                                    Byte[] PlainTextBytes = new Byte[] { };
                                    Byte[] MACBytes = new Byte[] { };
                                    Byte[] TrimmedCipherTextBytes = new Byte[] { };
                                    Boolean AbleToVerifyMAC = true;
                                    if (PostDataModel.AEADAlgorithmIndex != -1)
                                    {
                                        //XChaCha20Poly1305IETF
                                        //ChaCha20Poly1305IETF
                                        //ChaCha20Poly1305
                                        if (PostDataModel.AEADAlgorithmIndex == 0)
                                        {
                                            Nonce = new Byte[SodiumSecretAeadXChaCha20Poly1305IETF.GetNoncePublicLength()];
                                            TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - Nonce.LongLength];
                                            Array.Copy(CipherTextBytes, 0, Nonce, 0, Nonce.LongLength);
                                            Array.Copy(CipherTextBytes, Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                            try
                                            {
                                                PlainTextBytes = SodiumSecretAeadXChaCha20Poly1305IETF.Decrypt(TrimmedCipherTextBytes, Nonce, IntPtr.Zero, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption, AdditionalData);
                                            }
                                            catch
                                            {
                                                ResultString = "Error: Unable to decrypt..";
                                            }
                                        }
                                        else if (PostDataModel.AEADAlgorithmIndex == 1)
                                        {
                                            Nonce = new Byte[SodiumSecretAeadChaCha20Poly1305IETF.GetNoncePublicLength()];
                                            TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                            Array.Copy(CipherTextBytes, 0, Nonce, 0, Nonce.LongLength);
                                            Array.Copy(CipherTextBytes, Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                            try
                                            {
                                                PlainTextBytes = SodiumSecretAeadChaCha20Poly1305IETF.Decrypt(TrimmedCipherTextBytes, Nonce, IntPtr.Zero, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption, AdditionalData);
                                            }
                                            catch
                                            {
                                                ResultString = "Error: Unable to decrypt..";
                                            }
                                        }
                                        else
                                        {
                                            Nonce = new Byte[SodiumSecretAeadChaCha20Poly1305.GetNoncePublicLength()];
                                            TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                            Array.Copy(CipherTextBytes, 0, Nonce, 0, Nonce.LongLength);
                                            Array.Copy(CipherTextBytes, Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                            try
                                            {
                                                PlainTextBytes = SodiumSecretAeadChaCha20Poly1305.Decrypt(TrimmedCipherTextBytes, Nonce, IntPtr.Zero, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption, AdditionalData);
                                            }
                                            catch
                                            {
                                                ResultString = "Error: Unable to decrypt..";
                                            }
                                        }
                                        if (ResultString.Contains("Error") == false)
                                        {
                                            ResultString = Convert.ToBase64String(PlainTextBytes);
                                        }
                                    }
                                    else 
                                    {
                                        ResultString = "Error: Thi endpoint only support using AEAD to decrypt";
                                    }
                                }
                                else
                                {
                                    ResultString = "Error: There're no existing secret keys in SHSM";
                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
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

        [HttpPost("StreamCipherDecrypt")]
        public String StreamCipherDecryptData(SecretKeyCryptographyStreamCipherDataModel PostDataModel)
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
                                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption != IntPtr.Zero)
                                {
                                    Byte[] CipherTextBytes = Convert.FromBase64String(PostDataModel.Base64DataOrCipherText);
                                    if (PostDataModel.StreamCipherAlgorithmIndex != -1) 
                                    {
                                        Byte[] Nonce = new Byte[] { };
                                        Byte[] PlainTextBytes = new Byte[] { };
                                        Byte[] MACBytes = new Byte[] { };
                                        Byte[] TrimmedCipherTextBytes = new Byte[] { };
                                        Boolean AbleToVerifyMAC = true;
                                        PlainTextBytes = Nonce.Concat(PlainTextBytes).ToArray();
                                        if (PostDataModel.MACAlgorithmIndex != -1)
                                        {
                                            //XChaCha20, XSalsa20, ChaCha20, ChaCha20IETF, Salsa20, Salsa12, Salsa8
                                            //HMACSHA512256,HMACSHA512,HMACSHA256,Poly1305
                                            if (PostDataModel.MACAlgorithmIndex == 0)
                                            {
                                                MACBytes = new Byte[SodiumHMACSHA512256.GetComputedMACLength()];
                                                if (PostDataModel.StreamCipherAlgorithmIndex == 0)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.Length, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherXChaCha20.XChaCha20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 1)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherXSalsa20.XSalsa20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 2)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherChaCha20.GetChaCha20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherChaCha20.ChaCha20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 3)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherChaCha20.GetChaCha20IETFNonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherChaCha20.ChaCha20IETFDecrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 4)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherSalsa20.GetSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.Length, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherSalsa20.Salsa20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 5)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherSalsa20.GetSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherSalsa20128.Salsa2012RoundsEncrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherSalsa20.GetSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherSalsa20128.Salsa208RoundsEncrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                            }
                                            else if (PostDataModel.MACAlgorithmIndex == 1)
                                            {
                                                MACBytes = new Byte[SodiumHMACSHA512.GetComputedMACLength()];
                                                if (PostDataModel.StreamCipherAlgorithmIndex == 0)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherXChaCha20.XChaCha20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 1)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherXSalsa20.XSalsa20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 2)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherChaCha20.GetChaCha20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherChaCha20.ChaCha20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 3)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherChaCha20.GetChaCha20IETFNonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherChaCha20.ChaCha20IETFDecrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 4)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherSalsa20.GetSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherSalsa20.Salsa20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 5)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherSalsa20.GetSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherSalsa20128.Salsa2012RoundsEncrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherSalsa20.GetSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA512.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherSalsa20128.Salsa208RoundsEncrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                            }
                                            else if (PostDataModel.MACAlgorithmIndex == 2)
                                            {
                                                MACBytes = new Byte[SodiumHMACSHA256.GetComputedMACLength()];
                                                if (PostDataModel.StreamCipherAlgorithmIndex == 0)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherXChaCha20.XChaCha20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 1)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherXSalsa20.XSalsa20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 2)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherChaCha20.GetChaCha20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherChaCha20.ChaCha20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 3)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherChaCha20.GetChaCha20IETFNonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherChaCha20.ChaCha20IETFDecrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 4)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherSalsa20.GetSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherSalsa20.Salsa20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 5)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherSalsa20.GetSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherSalsa20128.Salsa2012RoundsEncrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherSalsa20.GetSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumHMACSHA256.VerifyMAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherSalsa20128.Salsa208RoundsEncrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                MACBytes = new Byte[SodiumOneTimeAuth.GetPoly1305MACLength()];
                                                if (PostDataModel.StreamCipherAlgorithmIndex == 0)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.Length, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumOneTimeAuth.VerifyPoly1305MAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherXChaCha20.XChaCha20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 1)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumOneTimeAuth.VerifyPoly1305MAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherXSalsa20.XSalsa20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 2)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherChaCha20.GetChaCha20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumOneTimeAuth.VerifyPoly1305MAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherChaCha20.ChaCha20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 3)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherChaCha20.GetChaCha20IETFNonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumOneTimeAuth.VerifyPoly1305MAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherChaCha20.ChaCha20IETFDecrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 4)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherSalsa20.GetSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumOneTimeAuth.VerifyPoly1305MAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherSalsa20.Salsa20Decrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else if (PostDataModel.StreamCipherAlgorithmIndex == 5)
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherSalsa20.GetSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumOneTimeAuth.VerifyPoly1305MAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherSalsa20128.Salsa2012RoundsEncrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                                else
                                                {
                                                    Nonce = new Byte[SodiumStreamCipherSalsa20.GetSalsa20NonceBytesLength()];
                                                    TrimmedCipherTextBytes = new Byte[CipherTextBytes.LongLength - MACBytes.LongLength - Nonce.LongLength];
                                                    Array.Copy(CipherTextBytes, 0, MACBytes, 0, MACBytes.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength, Nonce, 0, Nonce.LongLength);
                                                    Array.Copy(CipherTextBytes, MACBytes.LongLength + Nonce.LongLength, TrimmedCipherTextBytes, 0, TrimmedCipherTextBytes.LongLength);
                                                    AbleToVerifyMAC = SodiumOneTimeAuth.VerifyPoly1305MAC(MACBytes, Nonce.Concat(TrimmedCipherTextBytes).ToArray(), SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                                    if (AbleToVerifyMAC)
                                                    {
                                                        try
                                                        {
                                                            PlainTextBytes = SodiumStreamCipherSalsa20128.Salsa208RoundsEncrypt(TrimmedCipherTextBytes, Nonce, SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                                        }
                                                        catch
                                                        {
                                                            ResultString = "Error: Unable to decrypt..";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultString = "Error: Unable to verify MAC";
                                                    }
                                                }
                                            }
                                            if (ResultString.Contains("Error") == false)
                                            {
                                                ResultString = Convert.ToBase64String(PlainTextBytes);
                                            }
                                        }
                                        else
                                        {
                                            ResultString = "Error: Cipher text can only be decrypted once paired with MAC algorithms for stream cipher algorithms..";
                                        }
                                    }
                                    else 
                                    {
                                        ResultString = "Error: This endpoint only allow using stream cipher to decrypt";
                                    }
                                }
                                else
                                {
                                    ResultString = "Error: There're no existing secret keys in SHSM";
                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
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


        [HttpGet("ExportSecretKeys")]
        public SecretKeysExportModel ExportKeys(String User_ID, String SignedChallengeB64, Boolean UseXSalsa20Poly1305=true) 
        {
            SecretKeysExportModel MyModel = new SecretKeysExportModel();
            Boolean IsUserExist = RegisteredUsersHelper.users.ContainsKey(User_ID);
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
                String UserLongTermEDAuthPublicKeyB64 = UserInfo.Auth_PK;
                Byte[] UserLongTermEDAuthPublicKey = Convert.FromBase64String(UserLongTermEDAuthPublicKeyB64);
                String UserLongTermEDSigningPublicKeyB64 = UserInfo.Sign_PK;
                Byte[] UserLongTermEDSigningPublicKey = Convert.FromBase64String(UserLongTermEDSigningPublicKeyB64);
                String UserShortTermSignedEDGeneralDigitalSignaturePublicKeyB64 = UserSubSignedPKInfo.SignedDigitalSignaturePublicKeyB64;
                Byte[] UserShortTermSignedEDGeneralDigitalSignaturePublicKey = Convert.FromBase64String(UserShortTermSignedEDGeneralDigitalSignaturePublicKeyB64);
                Byte[] UserShortTermEDGeneralDigitalSignaturePublicKey = new Byte[] { };
                Byte[] SignedChallenge = Convert.FromBase64String(SignedChallengeB64);
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
                                    Byte[] RawMACSecretKey = new Byte[32];
                                    Byte[] RawEncryptionSecretKey = new Byte[32];
                                    Byte[] EncryptedMACSecretKey = new Byte[] { };
                                    Byte[] EncryptedEncryptionSecretKey = new Byte[] { };
                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                    Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC, RawMACSecretKey, 0, 32);
                                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                    Marshal.Copy(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption, RawEncryptionSecretKey, 0, 32);
                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForMAC);
                                    SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RegisteredUser.UserSignedPublicKeys.IsKEMorSealedBox[0])
                                    {
                                        EncapsulatedSharedSecretBox MyBox = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                        EncapsulatedSharedSecretBox MyBox2 = SodiumKEM.EncapsulateSecretKeyIntPtr(EncryptionPK);
                                        Byte[] Nonce = SodiumGenericHash.ComputeHash(64, EncryptionPK);
                                        if (UseXSalsa20Poly1305) 
                                        {
                                            Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                            EncryptedMACSecretKey = SodiumSecretBox.Create(RawMACSecretKey, Nonce, MyBox.SharedSecretIntPtr);
                                            Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                            EncryptedEncryptionSecretKey = SodiumSecretBox.Create(RawEncryptionSecretKey, Nonce, MyBox2.SharedSecretIntPtr);
                                        }
                                        else 
                                        {
                                            Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                            EncryptedMACSecretKey = SodiumSecretBoxXChaCha20Poly1305.Create(RawMACSecretKey, Nonce, MyBox.SharedSecretIntPtr);
                                            Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                            EncryptedEncryptionSecretKey = SodiumSecretBoxXChaCha20Poly1305.Create(RawEncryptionSecretKey, Nonce, MyBox2.SharedSecretIntPtr);
                                        }
                                        EncryptedMACSecretKey = MyBox.CipherTextBytes.Concat(EncryptedMACSecretKey).ToArray();
                                        EncryptedEncryptionSecretKey = MyBox2.CipherTextBytes.Concat(EncryptedEncryptionSecretKey).ToArray();
                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox.SharedSecretIntPtr);
                                        SodiumGuardedHeapAllocation.Sodium_Free(MyBox.SharedSecretIntPtr);
                                        SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(MyBox2.SharedSecretIntPtr);
                                        SodiumGuardedHeapAllocation.Sodium_Free(MyBox2.SharedSecretIntPtr);
                                    }
                                    else 
                                    {
                                        if (UseXSalsa20Poly1305) 
                                        {
                                            EncryptedMACSecretKey = SodiumSealedPublicKeyBox.Create(RawMACSecretKey, EncryptionPK);
                                            EncryptedEncryptionSecretKey = SodiumSealedPublicKeyBox.Create(RawEncryptionSecretKey, EncryptionPK);
                                        }
                                        else 
                                        {
                                            EncryptedMACSecretKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(RawMACSecretKey, EncryptionPK);
                                            EncryptedEncryptionSecretKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(RawEncryptionSecretKey, EncryptionPK);
                                        }
                                    }
                                    MyModel.EncryptedMACSecretKey = Convert.ToBase64String(EncryptedMACSecretKey);
                                    MyModel.EncryptedEncryptionSecretKey = Convert.ToBase64String(EncryptedEncryptionSecretKey);
                                    MyModel.StatusString = "Success: Able to export secret keys..";
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
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = User_ID;
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
            return MyModel;
        }

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
                            SodiumGuardedHeapAllocation.Sodium_MProtect_ReadOnly(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption != IntPtr.Zero) 
                            {
                                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ValidDateTime = DateTime.UtcNow.AddHours(12);
                                SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                ResultString = "Success: Extended the duration for secret keys..";
                            }
                            else 
                            {
                                SodiumGuardedHeapAllocation.Sodium_MProtect_NoAccess(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                                ResultString = "Error: You had not initialized secret keys..";
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
