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
    public static class PublicKeyCryptographySealedBoxDecryptDataCommand
    {
        public static Command Create()
        {
            var command = new Command(
                "pkcsealedboxdecryptdata",
                "Decrypt sealedbox encrypted data with imported sealedbox private key");

            var algorithmOption = new Option<int>("--algorithm")
            {
                Description = "The symmetric encryption algorithm" + Environment.NewLine +
                "0=XSalsa20Poly1305, 1=XChaCha20Poly1305",
                DefaultValueFactory = _ => 0
            };

            var dataOption = new Option<string>("--data")
            {
                Description = "Input a base64 String data",
                DefaultValueFactory = _ => ""
            };

            var dataEncodingTypeOption = new Option<int>("--encoding")
            {
                Description = "The type of encoding for the data String. (0=UTF8/Unicode,1=Base64)",
                DefaultValueFactory = _ => 0
            };

            //This part may need to be handled with care..
            //because there's no way to clear the potential sensitive data in the supported client languages
            //as they're in string or immutable data type..
            var dataOutputEncodingTypeOption = new Option<int>("--encoding_output")
            {
                Description = "The type of encoding for outputting the data String. (0=UTF8/Unicode,1=Base64)",
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
            command.Options.Add(dataOutputEncodingTypeOption);
            command.Options.Add(user_ID_Option);

            command.SetAction(parseResult =>
            {
                StandardizedDirectoriesFunction.InitializedDirectories();
                String User_ID = parseResult.GetValue(user_ID_Option)!;
                int AlgorithmChoice = parseResult.GetValue(algorithmOption)!;
                String DataToBeDecrypted = parseResult.GetValue(dataOption)!;
                int DataEncodingType = parseResult.GetValue(dataEncodingTypeOption)!;
                int OutputDataEncodingType = parseResult.GetValue(dataOutputEncodingTypeOption)!;
                if (Directory.Exists(StandardizedDirectoriesFunction.UsersRootFolder + User_ID) == true)
                {
                    Byte[] EncryptedDataStringBytes = new Byte[] { };
                    Boolean AbleToBeConvertFromB64String = true;
                    try
                    {
                        EncryptedDataStringBytes = Convert.FromBase64String(DataToBeDecrypted);
                    }
                    catch
                    {
                        AbleToBeConvertFromB64String = false;
                    }
                    if (AbleToBeConvertFromB64String)
                    {
                        PublicKeyCryptographyDecryptDataModel MyModel = new PublicKeyCryptographyDecryptDataModel();
                        MyModel.EncryptedDataB64 = DataToBeDecrypted;
                        MyModel.IsSealedBoxOrKEM = true;
                        MyModel.IsXSalsa20Poly1305OrXChaCha20Poly1305 = (AlgorithmChoice==0);
                        MyModel.KEMEncryptionPKB64 = "";
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
                        String ResultString = PublicKeyCryptoSealedBoxDecryptHelper.PublicKeyCryptoSealedBoxDecrypt(JSONBodyString);
                        Console.WriteLine(ResultString);
                    }
                    else
                    {
                        Console.WriteLine("Error: The data to be decrypted was not in base64 format..");
                    }
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
