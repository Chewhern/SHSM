using ASodium;
using BCASodium;
using SHSM_CLI.APIMethodHelper;
using SHSM_CLI.DirectoryHelper;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SHSM_CLI.ActualCommands
{
    public static class SecretKeyCryptographyInitializeCommand
    {
        public static Command Create()
        {
            var command = new Command(
                "skcinitialize",
                "Initialize a pair of SHSM secret keys");

            var user_ID_Option = new Option<string>("--user_ID")
            {
                Description = "What will be the user ID?",
                DefaultValueFactory = _ => ""
            };

            command.Options.Add(user_ID_Option);

            command.SetAction(parseResult =>
            {
                StandardizedDirectoriesFunction.InitializedDirectories();
                String User_ID = parseResult.GetValue(user_ID_Option)!;
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
                    String ResultString = SecretKeyCryptoInitializeHelper.SecretKeyCryptoInitialize(User_ID, Convert.ToBase64String(SignedChallengeBytes));
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
