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
using ASodium;

namespace SHSM_ClientApp.APIMethodHelper
{
    public static class PublicKeyCryptoExportDSAPrivateKeysHelper
    {
        public static String PublicKeyCryptoExportDSAPrivateKeys(String JSONString,String User_ID,Boolean UseXSalsa20Poly1305)
        {
            String UsersRootFolder = "";
            String PKCRootFolder = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                UsersRootFolder = AppContext.BaseDirectory + "\\Users\\";
                PKCRootFolder = AppContext.BaseDirectory + "\\PublicKeyCryptography\\";
            }
            else
            {
                UsersRootFolder = AppContext.BaseDirectory + "/Users/";
                PKCRootFolder = AppContext.BaseDirectory + "/PublicKeyCryptography/";
            }
            String StatusString = "";
            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(JSONString, Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(APIIPAddressHelper.IPAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.PostAsync($"PublicKeyCryptography/ExportDSAKeys",content);
                response.Wait();
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var Result = readTask.Result;

                    Console.WriteLine(Result);

                    PublicKeysExportModel MyModel = JsonConvert.DeserializeObject<PublicKeysExportModel>(Result);
                    Byte[] ExportPrivateKeyBytes = new Byte[] { };
                    Byte[] ExportPublicKeyBytes = new Byte[] { };
                    Byte[] Nonce = new Byte[] { };
                    Byte[] EncryptedPrivateKeyBytes = new Byte[] { };
                    Byte[] EncryptedSharedSecretBytes = new Byte[SodiumKEM.GetCipherTextBytesLength()];
                    Byte[] SharedSecretBytes = new Byte[SodiumKEM.GetSharedSecretBytesLength()];
                    Byte[] TrimmedEncryptedPrivateKeyBytes = new Byte[] { };
                    Byte[] PrivateKeyBytes = new Byte[] { };
                    if (MyModel.StatusString.Contains("Error") == false) 
                    {
                        if (Directory.Exists(PKCRootFolder + User_ID) == false) 
                        {
                            Directory.CreateDirectory(PKCRootFolder + User_ID);
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
                        if (MyModel.EncryptedPrivateKeyB64.CompareTo("") != 0) 
                        {
                            EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedPrivateKeyB64);
                            if (ExportPrivateKeyBytes.Length == 32 && ExportPublicKeyBytes.Length==32) 
                            {
                                if (UseXSalsa20Poly1305) 
                                {
                                    PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes, true);
                                }
                                else 
                                {
                                    PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes, true);
                                }
                            }
                            else 
                            {
                                Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                SharedSecretBytes = SodiumKEM.DecapsulateSharedSecret(EncryptedSharedSecretBytes, ExportPrivateKeyBytes, true);
                                Nonce = SodiumGenericHash.ComputeHash(64, ExportPublicKeyBytes);
                                if (UseXSalsa20Poly1305) 
                                {
                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                    PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes, true);
                                }
                                else
                                {
                                    Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                    PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes, true);
                                }
                            }
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                File.WriteAllBytes(PKCRootFolder + User_ID+"\\DecryptedDSAPrivateKey.txt",PrivateKeyBytes);
                            }
                            else 
                            {
                                File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedDSAPrivateKey.txt", PrivateKeyBytes);
                            }
                            SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                        }
                        else
                        {
                            if (ExportPrivateKeyBytes.Length == 32 && ExportPublicKeyBytes.Length == 32)
                            {
                                if (UseXSalsa20Poly1305)
                                {
                                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                                    {
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAD.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDPB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSADP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDQB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSADQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedPB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedQB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedModulusB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAModulus.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedExponentB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAExponent.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedInverseQB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes, true);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAInverseQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                    }
                                    else 
                                    {

                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAD.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDPB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSADP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDQB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSADQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedPB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedQB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedModulusB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAModulus.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedExponentB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAExponent.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedInverseQB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBox.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes, true);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAInverseQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                    }
                                }
                                else
                                {
                                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                                    {
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAD.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDPB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSADP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDQB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSADQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedPB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedQB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedModulusB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAModulus.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedExponentB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAExponent.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedInverseQB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes, true);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAInverseQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                    }
                                    else 
                                    {
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAD.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDPB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSADP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDQB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSADQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedPB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedQB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedModulusB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAModulus.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedExponentB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAExponent.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedInverseQB64);
                                        PrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Open(EncryptedPrivateKeyBytes, ExportPublicKeyBytes, ExportPrivateKeyBytes, true);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAInverseQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                    }
                                }
                            }
                            else
                            {
                                SharedSecretBytes = SodiumKEM.DecapsulateSharedSecret(EncryptedSharedSecretBytes, ExportPrivateKeyBytes, true);
                                Nonce = SodiumGenericHash.ComputeHash(64, ExportPublicKeyBytes);
                                if (UseXSalsa20Poly1305)
                                {
                                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                                    {
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedExponentB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAExponent.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedModulusB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAModulus.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAD.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedPB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedQB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDPB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSADP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDQB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSADQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedInverseQB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes, true);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAInverseQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                    }
                                    else
                                    {
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedExponentB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAExponent.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedModulusB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAModulus.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAD.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedPB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedQB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDPB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSADP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDQB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSADQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedInverseQB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBox.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes, true);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAInverseQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                    }
                                }
                                else
                                {
                                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                    {
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedExponentB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAExponent.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedModulusB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAModulus.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAD.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedPB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedQB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDPB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSADP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDQB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSADQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedInverseQB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes, true);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "\\DecryptedRSAInverseQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                    }
                                    else
                                    {
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedExponentB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAExponent.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedModulusB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAModulus.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAD.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedPB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedQB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDPB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSADP.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedDQB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSADQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                        EncryptedPrivateKeyBytes = Convert.FromBase64String(MyModel.EncryptedRSAKey.EncryptedInverseQB64);
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, 0, EncryptedSharedSecretBytes, 0, EncryptedSharedSecretBytes.Length);
                                        TrimmedEncryptedPrivateKeyBytes = new Byte[EncryptedPrivateKeyBytes.Length - EncryptedSharedSecretBytes.Length];
                                        Buffer.BlockCopy(EncryptedPrivateKeyBytes, EncryptedSharedSecretBytes.Length, TrimmedEncryptedPrivateKeyBytes, 0, TrimmedEncryptedPrivateKeyBytes.Length);
                                        PrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Open(TrimmedEncryptedPrivateKeyBytes, Nonce, SharedSecretBytes, true);
                                        File.WriteAllBytes(PKCRootFolder + User_ID + "/DecryptedRSAInverseQ.txt", PrivateKeyBytes);
                                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                                    }
                                }
                            }
                        }
                        StatusString = "Success: Fetched and decrypted server side exported private keys";
                    }
                    else 
                    {
                        StatusString = MyModel.StatusString;
                    }
                }
                else 
                {
                    Console.WriteLine(result.StatusCode);
                }
            }
            return StatusString;
        }
    }
}
