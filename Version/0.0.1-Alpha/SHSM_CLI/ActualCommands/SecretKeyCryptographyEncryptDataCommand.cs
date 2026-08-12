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
    public static class SecretKeyCryptographyEncryptDataCommand
    {
        public static Command Create()
        {
            var command = new Command(
                "skcencryptdata",
                "Encrypt data with a pair of imported secret keys");

            var aesAlgorithmOption = new Option<int>("--aes_algorithm")
            {
                Description = "The AES algorithm to be used to encrypt data" + Environment.NewLine +
                "-1=No option choosen, 0=AES256GCM(Hardware Accelerated/Secure AES),"+Environment.NewLine+
                "1=AEGIS256(HA/SA), 2=AEGIS128L(HA/SA)",
                DefaultValueFactory = _ => -1
            };

            var aeadAlgorithmOption = new Option<int>("--aead_algorithm")
            {
                Description = "The AEAD algorithm to be used to encrypt data" + Environment.NewLine +
                "-1=No option choosen, 0=XChaCha20Poly1305IETF," + Environment.NewLine +
                "1=ChaCha20Poly1305IETF, 2=ChaCha20Poly1305" + Environment.NewLine,
                DefaultValueFactory = _ => -1
            };

            var streamCipherAlgorithmOption = new Option<int>("--streamcipher_algorithm")
            {
                Description = "The AEAD algorithm to be used to encrypt data" + Environment.NewLine +
                "-1=No option choosen, 0=XChaCha20," + Environment.NewLine +
                "1=XSalsa20, 2=ChaCha20" + Environment.NewLine +
                "3=ChaCha20IETF, 4=Salsa20" + Environment.NewLine +
                "5=Salsa12, 6=Salsa8",
                DefaultValueFactory = _ => -1
            };

            var macAlgorithmOption = new Option<int>("--mac_algorithm")
            {
                Description = "The MAC algorithm" + Environment.NewLine +
                "-1=No option choosen, 0=HMACSHA512256" + Environment.NewLine +
                "1=HMACSHA512, 2=HMACSHA256" + Environment.NewLine +
                "3=Poly1305",
                DefaultValueFactory = _ => -1
            };

            var dataOption = new Option<string>("--data")
            {
                Description = "Input String data to be encrypted",
                DefaultValueFactory = _ => ""
            };

            var additionalDataOption = new Option<string>("--additional_data")
            {
                Description = "Input String 'additional data' to be encrypted",
                DefaultValueFactory = _ => ""
            };

            var dataEncodingTypeOption = new Option<int>("--encoding")
            {
                Description = "The type of encoding for both inputted Strings. (0=UTF8/Unicode,1=Base64)",
                DefaultValueFactory = _ => 0
            };

            var user_ID_Option = new Option<string>("--user_ID")
            {
                Description = "What will be the user ID?",
                DefaultValueFactory = _ => ""
            };

            command.Options.Add(aesAlgorithmOption);
            command.Options.Add(aeadAlgorithmOption);
            command.Options.Add(streamCipherAlgorithmOption);
            command.Options.Add(macAlgorithmOption);
            command.Options.Add(dataOption);
            command.Options.Add(additionalDataOption);
            command.Options.Add(dataEncodingTypeOption);
            command.Options.Add(user_ID_Option);

            command.SetAction(parseResult =>
            {
                StandardizedDirectoriesFunction.InitializedDirectories();
                String User_ID = parseResult.GetValue(user_ID_Option)!;
                int AESAlgorithmChoice = parseResult.GetValue(aesAlgorithmOption)!;
                int AEADAlgorithmChoice = parseResult.GetValue(aeadAlgorithmOption)!;
                int StreamCipherAlgorithmChoice = parseResult.GetValue(streamCipherAlgorithmOption)!;
                int MACAlgorithmChoice = parseResult.GetValue(macAlgorithmOption)!;
                String DataToBeEncrypted = parseResult.GetValue(dataOption)!;
                String AdditionalDataToBeSubmitted = parseResult.GetValue(dataOption)!;
                int DataEncodingType = parseResult.GetValue(dataEncodingTypeOption)!;
                if (Directory.Exists(StandardizedDirectoriesFunction.UsersRootFolder + User_ID) == true)
                {
                    Byte[] DataStringBytes = new Byte[] { };
                    String ActualDataString = "";
                    Byte[] AdditionalDataStringBytes = new Byte[] { };
                    String ActualAdditionalDataString = "";
                    Boolean AbleToBeConvertFromB64String = true;
                    Boolean IsUnicode = (DataEncodingType==0);
                    if (IsUnicode)
                    {
                        DataStringBytes = Encoding.UTF8.GetBytes(DataToBeEncrypted);
                        ActualDataString = Convert.ToBase64String(DataStringBytes);
                        AdditionalDataStringBytes = Encoding.UTF8.GetBytes(AdditionalDataToBeSubmitted);
                        ActualAdditionalDataString = Convert.ToBase64String(AdditionalDataStringBytes);
                    }
                    else
                    {
                        try
                        {
                            DataStringBytes = Convert.FromBase64String(DataToBeEncrypted);
                            AdditionalDataStringBytes = Convert.FromBase64String(AdditionalDataToBeSubmitted);
                        }
                        catch
                        {
                            AbleToBeConvertFromB64String = false;
                        }
                        if (AbleToBeConvertFromB64String) 
                        {
                            ActualDataString = DataToBeEncrypted;
                            ActualAdditionalDataString = AdditionalDataToBeSubmitted;
                        }
                        else 
                        {
                            Console.WriteLine("Error: Either data to be encrypted is not encoded in base64 or additional data is not encoded in base64");
                        }
                    }
                    if (AbleToBeConvertFromB64String)
                    {
                        if (AESAlgorithmChoice != -1)
                        {
                            SecretKeyCryptographyDataModel MyModel = new SecretKeyCryptographyDataModel();
                            MyModel.AESAlgorithmIndex = AESAlgorithmChoice;
                            MyModel.AdditionalDataB64 = ActualAdditionalDataString;
                            MyModel.Base64DataOrCipherText = ActualDataString;
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
                            String ResultString = SecretKeyCryptoEncryptHelper.SecretKeyCryptoEncrypt(JSONBodyString);
                            Console.WriteLine(ResultString);
                        }
                        else if (AEADAlgorithmChoice != -1)
                        {
                            SecretKeyCryptographyAEADDataModel MyModel = new SecretKeyCryptographyAEADDataModel();
                            MyModel.AEADAlgorithmIndex = AESAlgorithmChoice;
                            MyModel.AdditionalDataB64 = ActualAdditionalDataString;
                            MyModel.Base64DataOrCipherText = ActualDataString;
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
                            String ResultString = SecretKeyCryptoAEADEncryptHelper.SecretKeyCryptoAEADEncrypt(JSONBodyString);
                            Console.WriteLine(ResultString);
                        }
                        else
                        {
                            SecretKeyCryptographyStreamCipherDataModel MyModel = new SecretKeyCryptographyStreamCipherDataModel();
                            MyModel.StreamCipherAlgorithmIndex = StreamCipherAlgorithmChoice;
                            MyModel.MACAlgorithmIndex = MACAlgorithmChoice;
                            MyModel.Base64DataOrCipherText = ActualDataString;
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
                            String ResultString = SecretKeyCryptoStreamCipherEncryptHelper.SecretKeyCryptoStreamCipherEncrypt(JSONBodyString);
                            Console.WriteLine(ResultString);
                        }
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
