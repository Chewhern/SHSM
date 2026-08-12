using ASodium;
using BCASodium;
using SHSM_CLI.DirectoryHelper;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_CLI.ActualCommands
{
    public static class PublicKeyCryptographyVerifyDataCommand
    {
        public static Command Create()
        {
            var command = new Command(
                "pkcverifydata",
                "Verify Data with given public key exist within 'PublicKeyCryptography' folder..");

            var algorithmOption = new Option<int>("--algorithm")
            {
                Description = "The digital signature algorithm (ED25519 or ED448)" + Environment.NewLine +
                "0=ED25519, 1=ED448",
                DefaultValueFactory = _ => 0
            };

            var dataOption = new Option<string>("--data")
            {
                Description = "Input a Base64 signed data",
                DefaultValueFactory = _ => ""
            };

            var dataEncodingTypeOption = new Option<int>("--encoding")
            {
                Description = "The type of encoding for the verified data String. (0=UTF8/Unicode,1=Base64)",
                DefaultValueFactory = _ => 0
            };

            command.Options.Add(algorithmOption);
            command.Options.Add(dataOption);
            command.Options.Add(dataEncodingTypeOption);

            command.SetAction(parseResult =>
            {
                StandardizedDirectoriesFunction.InitializedDirectories();
                int AlgorithmChoice = parseResult.GetValue(algorithmOption)!;
                String SignedDataToBeVerified = parseResult.GetValue(dataOption)!;
                int DataEncodingType = parseResult.GetValue(dataEncodingTypeOption)!;
                if (Directory.GetFileSystemEntries(StandardizedDirectoriesFunction.PKCRootFolder).Length >0 && Directory.GetFileSystemEntries(StandardizedDirectoriesFunction.PKCRootFolder).Length<=3)
                {
                    Byte[] DigitalSignaturePublicKey = new Byte[] { };
                    Boolean IsProperPublicKeyExist = true;
                    Boolean IsBase64Data = true;
                    Boolean AbleToVerify = true;
                    String ActualVerifiedData = "";
                    if (AlgorithmChoice == 0)
                    {
                        try
                        {
                            DigitalSignaturePublicKey = File.ReadAllBytes(StandardizedDirectoriesFunction.PKCRootFolder + "ED25519PublicKey.txt");
                        }
                        catch
                        {
                            IsProperPublicKeyExist = false;
                        }
                        if (IsProperPublicKeyExist)
                        {
                            Byte[] SignedData = new Byte[] { };
                            try
                            {
                                SignedData = Convert.FromBase64String(SignedDataToBeVerified);
                            }
                            catch
                            {
                                IsBase64Data = false;
                            }
                            if (IsBase64Data)
                            {
                                try
                                {
                                    Byte[] VerifiedData = SodiumPublicKeyAuth.Verify(SignedData, DigitalSignaturePublicKey);
                                    if (DataEncodingType == 0)
                                    {
                                        ActualVerifiedData = Encoding.UTF8.GetString(VerifiedData);
                                    }
                                    else
                                    {
                                        ActualVerifiedData = Convert.ToBase64String(VerifiedData);
                                    }
                                }
                                catch
                                {
                                    AbleToVerify = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            DigitalSignaturePublicKey = File.ReadAllBytes(StandardizedDirectoriesFunction.PKCRootFolder + "ED448PublicKey.txt");
                        }
                        catch
                        {
                            IsProperPublicKeyExist = false;
                        }
                        if (IsProperPublicKeyExist)
                        {
                            Byte[] SignedData = new Byte[] { };
                            try
                            {
                                SignedData = Convert.FromBase64String(SignedDataToBeVerified);
                            }
                            catch
                            {
                                IsBase64Data = false;
                            }
                            if (IsBase64Data)
                            {
                                try
                                {
                                    Byte[] VerifiedData = SecureED448.GetMessageFromSignatureMessage(DigitalSignaturePublicKey, SignedData, new Byte[] { });
                                    if (DataEncodingType == 0)
                                    {
                                        ActualVerifiedData = Encoding.UTF8.GetString(VerifiedData);
                                    }
                                    else
                                    {
                                        ActualVerifiedData = Convert.ToBase64String(VerifiedData);
                                    }
                                }
                                catch
                                {
                                    AbleToVerify = false;
                                }
                            }
                        }
                    }
                    if (AbleToVerify) 
                    {
                        Console.WriteLine(ActualVerifiedData);
                    }
                    else 
                    {
                        Console.WriteLine("Error: Unable to verify given the digital signature algorithm and its public key");
                    }
                }
                else
                {
                    Console.WriteLine("Error: There's no public key material in 'PublicKeyCryptography' directory or the public key material count is greater than 3.");
                }
            });

            return command;
        }
    }
}
