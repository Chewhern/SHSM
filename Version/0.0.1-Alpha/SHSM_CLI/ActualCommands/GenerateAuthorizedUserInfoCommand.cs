using ASodium;
using BCASodium;
using Newtonsoft.Json;
using SHSM_CLI.DirectoryHelper;
using SHSM_CLI.Helper;
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
    public static class GenerateAuthorizedUserInfoCommand
    {
        //Refer back to SPKI AU and ON application..
        //Avoid using this in your CLI call if applicable
        //This is only for easier generating necessary information and let you use the JSON data to anchor to Arweave in advance..
        public static Command Create()
        {
            var command = new Command(
                "genauinfo",
                "Generate Authorized User Information and returning JSON data that must anchor to Arweave in advance..");

            var algorithmOption = new Option<string>("--algorithm")
            {
                Description = "The digital signature algorithm (ED25519 or ED448)",
                DefaultValueFactory = _ => "ED25519"
            };

            var public_Contact_Option = new Option<string>("--public_contact")
            {
                Description = "What will be the public contact?",
                DefaultValueFactory = _ => ""
            };

            var duration_Option = new Option<int>("--duration")
            {
                Description = "What will be the valid duration for this authorized user information? (Month)",
                DefaultValueFactory = _ => 1
            };

            command.Options.Add(algorithmOption);
            command.Options.Add(public_Contact_Option);
            command.Options.Add(duration_Option);

            command.SetAction(parseResult =>
            {
                StandardizedDirectoriesFunction.InitializedDirectories();
                String algorithmName = parseResult.GetValue(algorithmOption)!;
                bool IsValidDigitalSignatureAlgorithm = true;
                String Public_Contact = parseResult.GetValue(public_Contact_Option)!;
                int DurationInt = parseResult.GetValue(duration_Option)!;
                String Private_Contact = "";
                String AuthenticationPublicKeyB64String = "";
                String SigningPublicKeyB64String = "";
                String OOBPublicKeyB64String = "";
                DateTime CurrentDateTime = DateTime.UtcNow.AddHours(8);
                String User_ID = CryptographicSecureIDGenerator.GenerateMinimumAmountOfUniqueString(32);
                if (User_ID.Length > 32)
                {
                    User_ID = User_ID.Substring(0, 32);
                }
                while (Directory.Exists(StandardizedDirectoriesFunction.UsersRootFolder + User_ID) == true)
                {
                    User_ID = CryptographicSecureIDGenerator.GenerateMinimumAmountOfUniqueString(32);
                    if (User_ID.Length > 32)
                    {
                        User_ID = User_ID.Substring(0, 32);
                    }
                }
                Directory.CreateDirectory(StandardizedDirectoriesFunction.UsersRootFolder + User_ID);
                if (algorithmName.CompareTo("ED25519") == 0) 
                {
                    RevampedKeyPair MyAuthenticationKeyPair = SodiumPublicKeyAuth.GenerateRevampedKeyPair();
                    RevampedKeyPair MySigningKeyPair = SodiumPublicKeyAuth.GenerateRevampedKeyPair();
                    RevampedKeyPair MyOOBKeyPair = SodiumPublicKeyAuth.GenerateRevampedKeyPair();
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\APrivateKey.txt", MyAuthenticationKeyPair.PrivateKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\APublicKey.txt", MyAuthenticationKeyPair.PublicKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\SPrivateKey.txt", MySigningKeyPair.PrivateKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\SPublicKey.txt", MySigningKeyPair.PublicKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\OOBPrivateKey.txt", MyOOBKeyPair.PrivateKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\OOBPublicKey.txt", MyOOBKeyPair.PublicKey);
                        File.WriteAllText(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\AlgorithmType.txt", "ED25519");
                    }
                    else
                    {
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/APrivateKey.txt", MyAuthenticationKeyPair.PrivateKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/APublicKey.txt", MyAuthenticationKeyPair.PublicKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/SPrivateKey.txt", MySigningKeyPair.PrivateKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/SPublicKey.txt", MySigningKeyPair.PublicKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/OOBPrivateKey.txt", MyOOBKeyPair.PrivateKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/OOBPublicKey.txt", MyOOBKeyPair.PublicKey);
                        File.WriteAllText(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/AlgorithmType.txt", "ED25519");
                    }
                    AuthenticationPublicKeyB64String = Convert.ToBase64String(MyAuthenticationKeyPair.PublicKey);
                    SigningPublicKeyB64String = Convert.ToBase64String(MySigningKeyPair.PublicKey);
                    OOBPublicKeyB64String = Convert.ToBase64String(MyOOBKeyPair.PublicKey);
                    MyAuthenticationKeyPair.Clear();
                    MySigningKeyPair.Clear();
                    MyOOBKeyPair.Clear();
                }
                else if (algorithmName.CompareTo("ED448") == 0) 
                {
                    ED448RevampedKeyPair MyAuthenticationKeyPair = SecureED448.GenerateED448RevampedKeyPair();
                    ED448RevampedKeyPair MySigningKeyPair = SecureED448.GenerateED448RevampedKeyPair();
                    ED448RevampedKeyPair MyOOBKeyPair = SecureED448.GenerateED448RevampedKeyPair();
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\APrivateKey.txt", MyAuthenticationKeyPair.PrivateKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\APublicKey.txt", MyAuthenticationKeyPair.PublicKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\SPrivateKey.txt", MySigningKeyPair.PrivateKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\SPublicKey.txt", MySigningKeyPair.PublicKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\OOBPrivateKey.txt", MyOOBKeyPair.PrivateKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\OOBPublicKey.txt", MyOOBKeyPair.PublicKey);
                        File.WriteAllText(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\AlgorithmType.txt", "ED25519");
                    }
                    else
                    {
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/APrivateKey.txt", MyAuthenticationKeyPair.PrivateKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/APublicKey.txt", MyAuthenticationKeyPair.PublicKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/SPrivateKey.txt", MySigningKeyPair.PrivateKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/SPublicKey.txt", MySigningKeyPair.PublicKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/OOBPrivateKey.txt", MyOOBKeyPair.PrivateKey);
                        File.WriteAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/OOBPublicKey.txt", MyOOBKeyPair.PublicKey);
                        File.WriteAllText(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/AlgorithmType.txt", "ED25519");
                    }
                    AuthenticationPublicKeyB64String = Convert.ToBase64String(MyAuthenticationKeyPair.PublicKey);
                    SigningPublicKeyB64String = Convert.ToBase64String(MySigningKeyPair.PublicKey);
                    OOBPublicKeyB64String = Convert.ToBase64String(MyOOBKeyPair.PublicKey);
                    MyAuthenticationKeyPair.Clear();
                    MySigningKeyPair.Clear();
                    MyOOBKeyPair.Clear();
                }
                else 
                {
                    IsValidDigitalSignatureAlgorithm = false;
                    Console.WriteLine("Error: Not supporting other digital signature algorithms for now..");
                }
                if (IsValidDigitalSignatureAlgorithm == true && DurationInt <= 6 && DurationInt > 0) 
                {
                    CurrentDateTime = CurrentDateTime.AddDays((DurationInt * 30) + 2);
                    AUInfoModel MyModel = new AUInfoModel();
                    MyModel.User_ID = User_ID;
                    MyModel.Public_Contact = Public_Contact;
                    MyModel.Private_Contact = Private_Contact;
                    MyModel.Sign_PK = SigningPublicKeyB64String;
                    MyModel.Auth_PK = AuthenticationPublicKeyB64String;
                    MyModel.OOB_PK = OOBPublicKeyB64String;
                    MyModel.ValidDate_Day = CurrentDateTime.Day;
                    MyModel.ValidDate_Month = CurrentDateTime.Month;
                    MyModel.ValidDate_Year = CurrentDateTime.Year;
                    MyModel.DSA_Type = algorithmName;
                    String JSONString = JsonConvert.SerializeObject(MyModel);
                    Console.WriteLine(JSONString);
                }
                else 
                {
                    Console.WriteLine("Error: Valid duration should only be minimum 1 month and maximum 6 months");
                }
            });

            return command;
        }
    }
}
