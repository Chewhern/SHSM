using ASodium;
using BCASodium;
using Newtonsoft.Json;
using SHSM_CLI.DirectoryHelper;
using SHSM_CLI.SHSMDataModel;
using SHSM_CLI.SPKIDataModel;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_CLI.ActualCommands
{
    public static class GenerateSubKeyMaterialCommand
    {
        //Similar in logic to generate au info
        //This's also mainly used for easier anchor data to Arweave in advance..

        public static Command Create()
        {
            var command = new Command(
                "gensubkey",
                "Generate sub key for a given user id and returning JSON data that must anchor to Arweave in advance..");

            var algorithmOption = new Option<string>("--algorithm")
            {
                Description = "The digital signature algorithm (ED25519 or ED448)",
                DefaultValueFactory = _ => "ED25519"
            };

            var user_ID_Option = new Option<string>("--user_ID")
            {
                Description = "What will be the user ID?",
                DefaultValueFactory = _ => ""
            };

            var duration_Option = new Option<int>("--duration")
            {
                Description = "What will be the valid duration for this sub general purpose digital signature key pair? (Month)",
                DefaultValueFactory = _ => 1
            };

            command.Options.Add(algorithmOption);
            command.Options.Add(user_ID_Option);
            command.Options.Add(duration_Option);

            command.SetAction(parseResult =>
            {
                StandardizedDirectoriesFunction.InitializedDirectories();
                String algorithmName = parseResult.GetValue(algorithmOption)!;
                bool IsValidDigitalSignatureAlgorithm = true;
                String User_ID = parseResult.GetValue(user_ID_Option)!;
                int DurationInt = parseResult.GetValue(duration_Option)!;
                String SignedSubGeneralPurposeSignaturePublicKeyB64 = "";
                if (Directory.Exists(StandardizedDirectoriesFunction.UsersRootFolder + User_ID) == true && DurationInt<=5 && DurationInt>0) 
                {
                    if (algorithmName.CompareTo("ED25519") == 0)
                    {
                        Byte[] SubGeneralPurposeSignaturePublicKey = new Byte[] { };
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                        {
                            RevampedKeyPair MyKeyPair = SodiumPublicKeyAuth.GenerateRevampedKeyPair();
                            SubGeneralPurposeSignaturePublicKey = MyKeyPair.PublicKey;
                            File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt", MyKeyPair.PrivateKey);
                            File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\SubDSAPublicKey.txt", MyKeyPair.PublicKey);
                            MyKeyPair.Clear();
                        }
                        else
                        {
                            RevampedKeyPair MyKeyPair = SodiumPublicKeyAuth.GenerateRevampedKeyPair();
                            File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt", MyKeyPair.PrivateKey);
                            File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/SubDSAPublicKey.txt", MyKeyPair.PublicKey);
                            MyKeyPair.Clear();
                        }
                        Byte[] RootSigningSignaturePrivateKey = File.ReadAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\SPrivateKey.txt");
                        Byte[] SignedSubGeneralPurposeSignaturePublicKey = new Byte[] { };
                        if (RootSigningSignaturePrivateKey.Length == 64) 
                        {
                            SignedSubGeneralPurposeSignaturePublicKey = SodiumPublicKeyAuth.Sign(SubGeneralPurposeSignaturePublicKey, RootSigningSignaturePrivateKey, true);
                        }
                        else 
                        {
                            SignedSubGeneralPurposeSignaturePublicKey = SecureED448.GenerateSignatureMessage(RootSigningSignaturePrivateKey, SubGeneralPurposeSignaturePublicKey, new Byte[] { }, true);
                        }
                        SignedSubGeneralPurposeSignaturePublicKeyB64 = Convert.ToBase64String(SignedSubGeneralPurposeSignaturePublicKey);
                        algorithmName = "ED25519";
                    }
                    else if (algorithmName.CompareTo("ED448") == 0)
                    {
                        Byte[] SubGeneralPurposeSignaturePublicKey = new Byte[] { };
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            ED448RevampedKeyPair MyKeyPair = SecureED448.GenerateED448RevampedKeyPair();
                            SubGeneralPurposeSignaturePublicKey = MyKeyPair.PublicKey;
                            File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt", MyKeyPair.PrivateKey);
                            File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\SubDSAPublicKey.txt", MyKeyPair.PublicKey);
                            MyKeyPair.Clear();
                        }
                        else
                        {
                            ED448RevampedKeyPair MyKeyPair = SecureED448.GenerateED448RevampedKeyPair();
                            SubGeneralPurposeSignaturePublicKey = MyKeyPair.PublicKey;
                            File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt", MyKeyPair.PrivateKey);
                            File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/SubDSAPublicKey.txt", MyKeyPair.PublicKey);
                            MyKeyPair.Clear();
                        }

                        Byte[] RootSigningSignaturePrivateKey = File.ReadAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\SPrivateKey.txt");
                        Byte[] SignedSubGeneralPurposeSignaturePublicKey = new Byte[] { };
                        if (RootSigningSignaturePrivateKey.Length == 64)
                        {
                            SignedSubGeneralPurposeSignaturePublicKey = SodiumPublicKeyAuth.Sign(SubGeneralPurposeSignaturePublicKey, RootSigningSignaturePrivateKey, true);
                        }
                        else
                        {
                            SignedSubGeneralPurposeSignaturePublicKey = SecureED448.GenerateSignatureMessage(RootSigningSignaturePrivateKey, SubGeneralPurposeSignaturePublicKey, new Byte[] { }, true);
                        }
                        SignedSubGeneralPurposeSignaturePublicKeyB64 = Convert.ToBase64String(SignedSubGeneralPurposeSignaturePublicKey);
                        algorithmName = "ED448";
                    }
                    else
                    {
                        IsValidDigitalSignatureAlgorithm = false;
                        Console.WriteLine("Error: Not supporting other digital signature algorithms for now..");
                    }
                    if (IsValidDigitalSignatureAlgorithm)
                    {
                        DateTime CurrentDateTime = DateTime.UtcNow.AddHours(8).AddDays(DurationInt*30);
                        SubSignedPKModel MyModel = new SubSignedPKModel();
                        MyModel.SignedDigitalSignaturePublicKeyB64 = SignedSubGeneralPurposeSignaturePublicKeyB64;
                        MyModel.DigitalSignatureAlgorithm = algorithmName;
                        MyModel.ValidDate_Day = CurrentDateTime.Day;
                        MyModel.ValidDate_Month = CurrentDateTime.Month;
                        MyModel.ValidDate_Year = CurrentDateTime.Year;
                        String JSONString = JsonConvert.SerializeObject(MyModel);
                        Console.WriteLine(JSONString);
                    }
                }
                else 
                {
                    Console.WriteLine("Error: This specified user id doesn't exist or the duration exceeded maximum 5 months threshold or the duration is lower than or not equal to 1");
                }
            });

            return command;
        }
    }
}
