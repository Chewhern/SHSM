using ASodium;
using BCASodium;
using Newtonsoft.Json;
using SHSM_CLI.APIMethodHelper;
using SHSM_CLI.DirectoryHelper;
using SHSM_CLI.PostDataModel;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_CLI.ActualCommands
{
    public static class UploadDataToArweaveCommand
    {
        public static Command Create()
        {
            var command = new Command(
                "arweaveuploaddata",
                "Upload data to arweave in a memory safe manner..");

            var dataOption = new Option<string>("--data")
            {
                Description = "Input normal string data or JSON string data",
                DefaultValueFactory = _ => ""
            };

            var user_ID_Option = new Option<string>("--user_ID")
            {
                Description = "What will be the user ID?",
                DefaultValueFactory = _ => ""
            };

            command.Options.Add(dataOption);
            command.Options.Add(user_ID_Option);

            command.SetAction(parseResult =>
            {
                StandardizedDirectoriesFunction.InitializedDirectories();
                String User_ID = parseResult.GetValue(user_ID_Option)!;
                String DataPayload = parseResult.GetValue(dataOption)!;
                if (Directory.Exists(StandardizedDirectoriesFunction.UsersRootFolder + User_ID) == true)
                {
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
                    ArweaveRSAOpsDataModel MyModel = new ArweaveRSAOpsDataModel();
                    MyModel.User_ID = User_ID;
                    MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                    MyModel.JSONDataString = DataPayload;
                    String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                    String ResultString = ArweaveUploadDataHelper.ArweaveUploadData(JSONBodyString);
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
