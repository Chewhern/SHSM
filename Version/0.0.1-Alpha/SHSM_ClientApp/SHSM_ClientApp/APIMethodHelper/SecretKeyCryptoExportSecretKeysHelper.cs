using ASodium;
using Newtonsoft.Json;
using SHSM_ClientApp.Helper;
using SHSM_ClientApp.SHSMDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace SHSM_ClientApp.APIMethodHelper
{
    public static class SecretKeyCryptoExportSecretKeysHelper
    {
        public static String SecretKeyCryptoExportSecretKeys(String User_ID, String SignedChallengeB64, Boolean UseXSalsa20Poly1305 = true)
        {
            String UsersRootFolder = "";
            String SecretKeyRootFolder = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                UsersRootFolder = AppContext.BaseDirectory + "\\Users\\";
                SecretKeyRootFolder = AppContext.BaseDirectory + "\\SecretKeyCryptography\\";
            }
            else
            {
                UsersRootFolder = AppContext.BaseDirectory + "/Users/";
                SecretKeyRootFolder = AppContext.BaseDirectory + "\\SecretKeyCryptography\\";
            }
            String StatusString = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(APIIPAddressHelper.IPAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync($"SecretKeyCryptography/ExportSecretKeys?User_ID={User_ID}&SignedChallengeB64={HttpUtility.UrlEncode(SignedChallengeB64)}&UseXSalsa20Poly1305={UseXSalsa20Poly1305}");
                response.Wait();
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var Result = readTask.Result;

                    SecretKeysExportModel MyModel = new SecretKeysExportModel();
                    MyModel = JsonConvert.DeserializeObject<SecretKeysExportModel>(Result);
                    Byte[] ExportPrivateKeyBytes = new Byte[] { };
                    Byte[] ExportPublicKeyBytes = new Byte[] { };
                    Byte[] Nonce = new Byte[] { };
                    Byte[] EncryptedSecretKeyBytes = new Byte[] { };
                    Byte[] EncryptedSharedSecretBytes = new Byte[SodiumKEM.GetCipherTextBytesLength()];
                    Byte[] SharedSecretBytes = new Byte[SodiumKEM.GetSharedSecretBytesLength()];
                    Byte[] TrimmedEncryptedSecretKeyBytes = new Byte[] { };
                    Byte[] SecretKeyBytes = new Byte[] { };
                    if (MyModel.StatusString.Contains("Error") == false)
                    {
                        if (Directory.Exists(SecretKeyRootFolder + User_ID) == false) 
                        {
                            Directory.CreateDirectory(SecretKeyRootFolder + User_ID);
                        }
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            ExportPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\ExportPrivateKey.txt");
                            ExportPublicKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\ExportPublicKey.txt");
                        }
                        else
                        {
                            ExportPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/ExportPrivateKey.txt");
                            ExportPublicKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/ExportPrivateKey.txt");
                        }
                        if(ExportPrivateKeyBytes.Length == 32 && ExportPublicKeyBytes.Length==32) 
                        {
                            if (UseXSalsa20Poly1305)
                            {
                                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                                {
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedMACSecretKey);
                                    SecretKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedSecretKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "\\DecryptedMACSecretKey.txt", SecretKeyBytes);
                                    SodiumSecureMemory.SecureClearBytes(SecretKeyBytes);
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedEncryptionSecretKey);
                                    SecretKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedSecretKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "\\DecryptedEncryptionSecretKey.txt", SecretKeyBytes);
                                }
                                else
                                {
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedMACSecretKey);
                                    SecretKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedSecretKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "/DecryptedMACSecretKey.txt", SecretKeyBytes);
                                    SodiumSecureMemory.SecureClearBytes(SecretKeyBytes);
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedEncryptionSecretKey);
                                    SecretKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedSecretKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "/DecryptedEncryptionSecretKey.txt", SecretKeyBytes);
                                }
                            }
                            else
                            {
                                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                {
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedMACSecretKey);
                                    SecretKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedSecretKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "\\DecryptedMACSecretKey.txt", SecretKeyBytes);
                                    SodiumSecureMemory.SecureClearBytes(SecretKeyBytes);
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedEncryptionSecretKey);
                                    SecretKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedSecretKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "\\DecryptedEncryptionSecretKey.txt", SecretKeyBytes);
                                }
                                else
                                {
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedMACSecretKey);
                                    SecretKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedSecretKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "/DecryptedMACSecretKey.txt", SecretKeyBytes);
                                    SodiumSecureMemory.SecureClearBytes(SecretKeyBytes);
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedEncryptionSecretKey);
                                    SecretKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedSecretKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "/DecryptedEncryptionSecretKey.txt", SecretKeyBytes);
                                }
                            }
                        }
                        else 
                        {
                            if (UseXSalsa20Poly1305)
                            {
                                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                                {
                                    Nonce = SodiumGenericHash.ComputeHash(64, ExportPublicKeyBytes);
                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedMACSecretKey);
                                    TrimmedEncryptedSecretKeyBytes = new Byte[EncryptedSecretKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                    SharedSecretBytes = SodiumKEM.DecapsulateSharedSecret(EncryptedSharedSecretBytes, ExportPrivateKeyBytes);
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedSecretKeyBytes, 0, TrimmedEncryptedSecretKeyBytes.Length);
                                    SecretKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedSecretKeyBytes, Nonce, SharedSecretBytes, true);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "\\DecryptedMACSecretKey.txt", SecretKeyBytes);
                                    SodiumSecureMemory.SecureClearBytes(SecretKeyBytes);
                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedEncryptionSecretKey);
                                    TrimmedEncryptedSecretKeyBytes = new Byte[EncryptedSecretKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                    SharedSecretBytes = SodiumKEM.DecapsulateSharedSecret(EncryptedSharedSecretBytes, ExportPrivateKeyBytes,true);
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedSecretKeyBytes, 0, TrimmedEncryptedSecretKeyBytes.Length);
                                    SecretKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedSecretKeyBytes, Nonce, SharedSecretBytes, true);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "\\DecryptedEncryptionSecretKey.txt", SecretKeyBytes);
                                    SodiumSecureMemory.SecureClearBytes(SecretKeyBytes);
                                }
                                else 
                                {
                                    Nonce = SodiumGenericHash.ComputeHash(64, ExportPublicKeyBytes);
                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedMACSecretKey);
                                    TrimmedEncryptedSecretKeyBytes = new Byte[EncryptedSecretKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                    SharedSecretBytes = SodiumKEM.DecapsulateSharedSecret(EncryptedSharedSecretBytes, ExportPrivateKeyBytes);
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedSecretKeyBytes, 0, TrimmedEncryptedSecretKeyBytes.Length);
                                    SecretKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedSecretKeyBytes, Nonce, SharedSecretBytes, true);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "/DecryptedMACSecretKey.txt", SecretKeyBytes);
                                    SodiumSecureMemory.SecureClearBytes(SecretKeyBytes);
                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedEncryptionSecretKey);
                                    TrimmedEncryptedSecretKeyBytes = new Byte[EncryptedSecretKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                    SharedSecretBytes = SodiumKEM.DecapsulateSharedSecret(EncryptedSharedSecretBytes, ExportPrivateKeyBytes,true);
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedSecretKeyBytes, 0, TrimmedEncryptedSecretKeyBytes.Length);
                                    SecretKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedSecretKeyBytes, Nonce, SharedSecretBytes, true);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "/DecryptedEncryptionSecretKey.txt", SecretKeyBytes);
                                    SodiumSecureMemory.SecureClearBytes(SecretKeyBytes);
                                }
                            }
                            else 
                            {
                                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                {
                                    Nonce = SodiumGenericHash.ComputeHash(64, ExportPublicKeyBytes);
                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedMACSecretKey);
                                    TrimmedEncryptedSecretKeyBytes = new Byte[EncryptedSecretKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                    SharedSecretBytes = SodiumKEM.DecapsulateSharedSecret(EncryptedSharedSecretBytes, ExportPrivateKeyBytes);
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedSecretKeyBytes, 0, TrimmedEncryptedSecretKeyBytes.Length);
                                    SecretKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedSecretKeyBytes, Nonce, SharedSecretBytes, true);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "\\DecryptedMACSecretKey.txt", SecretKeyBytes);
                                    SodiumSecureMemory.SecureClearBytes(SecretKeyBytes);
                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedEncryptionSecretKey);
                                    TrimmedEncryptedSecretKeyBytes = new Byte[EncryptedSecretKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                    SharedSecretBytes = SodiumKEM.DecapsulateSharedSecret(EncryptedSharedSecretBytes, ExportPrivateKeyBytes, true);
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedSecretKeyBytes, 0, TrimmedEncryptedSecretKeyBytes.Length);
                                    SecretKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedSecretKeyBytes, Nonce, SharedSecretBytes, true);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "\\DecryptedEncryptionSecretKey.txt", SecretKeyBytes);
                                    SodiumSecureMemory.SecureClearBytes(SecretKeyBytes);
                                }
                                else
                                {
                                    Nonce = SodiumGenericHash.ComputeHash(64, ExportPublicKeyBytes);
                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedMACSecretKey);
                                    TrimmedEncryptedSecretKeyBytes = new Byte[EncryptedSecretKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                    SharedSecretBytes = SodiumKEM.DecapsulateSharedSecret(EncryptedSharedSecretBytes, ExportPrivateKeyBytes);
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedSecretKeyBytes, 0, TrimmedEncryptedSecretKeyBytes.Length);
                                    SecretKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedSecretKeyBytes, Nonce, SharedSecretBytes, true);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "/DecryptedMACSecretKey.txt", SecretKeyBytes);
                                    SodiumSecureMemory.SecureClearBytes(SecretKeyBytes);
                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                    EncryptedSecretKeyBytes = Convert.FromBase64String(MyModel.EncryptedEncryptionSecretKey);
                                    TrimmedEncryptedSecretKeyBytes = new Byte[EncryptedSecretKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                    SharedSecretBytes = SodiumKEM.DecapsulateSharedSecret(EncryptedSharedSecretBytes, ExportPrivateKeyBytes, true);
                                    Buffer.BlockCopy(EncryptedSecretKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedSecretKeyBytes, 0, TrimmedEncryptedSecretKeyBytes.Length);
                                    SecretKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedSecretKeyBytes, Nonce, SharedSecretBytes, true);
                                    File.WriteAllBytes(SecretKeyRootFolder + User_ID + "/DecryptedEncryptionSecretKey.txt", SecretKeyBytes);
                                    SodiumSecureMemory.SecureClearBytes(SecretKeyBytes);
                                }
                            }
                        }
                        StatusString = "Success: Fetched and decrypted server side exported secret keys";
                    }
                    else
                    {
                        StatusString = MyModel.StatusString;
                    }
                }
            }
            return StatusString;
        }
    }
}
