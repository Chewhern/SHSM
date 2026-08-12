using ASodium;
using BCASodium;
using Newtonsoft.Json;
using SHSM_CLI.APIMethodHelper;
using SHSM_CLI.DirectoryHelper;
using SHSM_CLI.PostDataModel;
using SHSM_CLI.SHSMDataModel;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_CLI.ActualCommands
{
    public static class PublicKeyCryptographySignDataCommand
    {
        public static Command Create()
        {
            var command = new Command(
                "pkcsigndata",
                "Sign data with imported digital signature private key");

            var algorithmOption = new Option<int>("--algorithm")
            {
                Description = "The digital signature algorithm (ED25519 or ED448)" +Environment.NewLine+
                "0=ED25519, 1=ED448",
                DefaultValueFactory = _ => 0
            };

            var dataOption = new Option<string>("--data")
            {
                Description = "Input a String data",
                DefaultValueFactory = _ => ""
            };

            var dataEncodingTypeOption = new Option<int>("--encoding")
            {
                Description = "The type of encoding for the data String. (0=UTF8/Unicode,1=Base64)",
                DefaultValueFactory = _ => 0
            };

            var user_ID_Option = new Option<string>("--user_ID")
            {
                Description = "What will be the user ID?",
                DefaultValueFactory = _ => ""
            };

            command.Options.Add(algorithmOption);
            command.Options.Add(dataOption);
            command.Options.Add(dataEncodingTypeOption);
            command.Options.Add(user_ID_Option);

            command.SetAction(parseResult =>
            {
                StandardizedDirectoriesFunction.InitializedDirectories();
                String User_ID = parseResult.GetValue(user_ID_Option)!;
                int AlgorithmChoice = parseResult.GetValue(algorithmOption)!;
                String DataToBeSigned = parseResult.GetValue(dataOption)!;
                int DataEncodingType = parseResult.GetValue(dataEncodingTypeOption)!;
                if (Directory.Exists(StandardizedDirectoriesFunction.UsersRootFolder + User_ID) == true)
                {
                    Byte[] DataToBeSignedBytes = new Byte[] { };
                    String ActualDataString = "";
                    if (DataEncodingType==0)
                    {
                        DataToBeSignedBytes = Encoding.UTF8.GetBytes(DataToBeSigned);
                        ActualDataString = Convert.ToBase64String(DataToBeSignedBytes);
                    }
                    else
                    {
                        ActualDataString = DataToBeSigned;
                    }
                    PublicKeyCryptographySignDataModel MyModel = new PublicKeyCryptographySignDataModel();
                    MyModel.DataB64 = ActualDataString;
                    MyModel.IsED25519OrED448OrRSA = AlgorithmChoice;
                    MyModel.SignedChallengeB64 = "";
                    MyModel.User_ID = User_ID;
                    Byte[] DSAPrivateKeyBytes = new Byte[] { };
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        DSAPrivateKeyBytes = File.ReadAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                    }
                    else
                    {
                        DSAPrivateKeyBytes = File.ReadAllBytes(StandardizedDirectoriesFunction.UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                    }
                    Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                    Byte[] SignedChallengeBytes = new Byte[] { };
                    if (DSAPrivateKeyBytes.Length == 64)
                    {
                        SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                    }
                    else
                    {
                        SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                    }
                    MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                    String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                    String ResultString = PublicKeyCryptoSignDataHelper.PublicKeyCryptoSignData(JSONBodyString);
                    Console.WriteLine(ResultString);
                }
                else 
                {
                    Console.WriteLine("Error: This specified user id doesn't exist");
                }
            });

            return command;
        }
    }
}
