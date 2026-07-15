using Newtonsoft.Json;
using SHSM_ClientApp.Helper;
using SHSM_ClientApp.SHSMDataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_ClientApp.APIMethodHelper
{
    public static class ETLSInitializationHelper
    {
        public static String ETLSInitialization(String User_ID,Boolean IsKEM=true) 
        {
            String StatusString = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(APIIPAddressHelper.IPAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync($"ETLS?User_ID={User_ID}&IsKEM={IsKEM}");
                response.Wait();
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var Result = readTask.Result;

                    ServerInitialETLSDataModel MyModel = JsonConvert.DeserializeObject<ServerInitialETLSDataModel>(Result);

                    if (IsKEM) 
                    {
                        if (MyModel.KEMPublicKeyB64.CompareTo("") != 0) 
                        {
                            String ETLSRootFolder = "";
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                            {
                                ETLSRootFolder = AppContext.BaseDirectory + "\\ETLS\\";
                                if (Directory.Exists(ETLSRootFolder + User_ID) == false) 
                                {
                                    Directory.CreateDirectory(ETLSRootFolder + User_ID);
                                }
                                File.WriteAllBytes(ETLSRootFolder + User_ID + "\\ETLSKEMPublicKey.txt", Convert.FromBase64String(MyModel.KEMPublicKeyB64));
                                File.WriteAllText(ETLSRootFolder + User_ID + "\\ETLSAlgorithmType.txt", "KEM");
                            }
                            else 
                            {
                                ETLSRootFolder = AppContext.BaseDirectory + "/ETLS/";
                                if (Directory.Exists(ETLSRootFolder + User_ID) == false)
                                {
                                    Directory.CreateDirectory(ETLSRootFolder + User_ID);
                                }
                                File.WriteAllBytes(ETLSRootFolder + User_ID + "/ETLSKEMPublicKey.txt", Convert.FromBase64String(MyModel.KEMPublicKeyB64));
                                File.WriteAllText(ETLSRootFolder + User_ID + "/ETLSAlgorithmType.txt", "KEM");
                            }
                            StatusString = "Success: Created ETLS public key (KEM)";
                        }
                        else 
                        {
                            StatusString = "Error: You had already initialized ETLS for import operations.. Kindly check back if you initialized using KEM or SealedBox";
                        }
                    }
                    else 
                    {
                        if (MyModel.EncryptionX25519PublicKeyB64.CompareTo("") != 0 && MyModel.MACX25519PublicKeyB64.CompareTo("")!=0)
                        {
                            String ETLSRootFolder = "";
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                ETLSRootFolder = AppContext.BaseDirectory + "\\ETLS\\";
                                if (Directory.Exists(ETLSRootFolder + User_ID) == false)
                                {
                                    Directory.CreateDirectory(ETLSRootFolder + User_ID);
                                }
                                File.WriteAllBytes(ETLSRootFolder + User_ID + "\\ETLSMACX25519PublicKey.txt", Convert.FromBase64String(MyModel.MACX25519PublicKeyB64));
                                File.WriteAllBytes(ETLSRootFolder + User_ID + "\\ETLSEncryptionX25519PublicKey.txt", Convert.FromBase64String(MyModel.EncryptionX25519PublicKeyB64));
                                File.WriteAllText(ETLSRootFolder + User_ID + "\\ETLSAlgorithmType.txt", "X25519");
                            }
                            else
                            {
                                ETLSRootFolder = AppContext.BaseDirectory + "/ETLS/";
                                if (Directory.Exists(ETLSRootFolder + User_ID) == false)
                                {
                                    Directory.CreateDirectory(ETLSRootFolder + User_ID);
                                }
                                File.WriteAllBytes(ETLSRootFolder + User_ID + "\\ETLSMACX25519PublicKey.txt", Convert.FromBase64String(MyModel.MACX25519PublicKeyB64));
                                File.WriteAllBytes(ETLSRootFolder + User_ID + "\\ETLSEncryptionX25519PublicKey.txt", Convert.FromBase64String(MyModel.EncryptionX25519PublicKeyB64));
                                File.WriteAllText(ETLSRootFolder + User_ID + "/ETLSAlgorithmType.txt", "X25519");
                            }
                            StatusString = "Success: Created ETLS public key (Server side - Sealedbox)";
                        }
                        else
                        {
                            StatusString = "Error: You had already initialized ETLS for import operations.. Kindly check back if you initialized using KEM or SealedBox";
                        }
                    }
                }
            }
            return StatusString;
        }
    }
}
