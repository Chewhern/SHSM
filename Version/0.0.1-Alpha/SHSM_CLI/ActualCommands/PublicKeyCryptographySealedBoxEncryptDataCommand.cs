using SHSM_CLI.DirectoryHelper;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCASodium;
using ASodium;
using System.Runtime.InteropServices;

namespace SHSM_CLI.ActualCommands
{
    public static class PublicKeyCryptographySealedBoxEncryptDataCommand
    {
        public static Command Create()
        {
            var command = new Command(
                "pkcsealedboxencryptdata",
                "Encrypt data with given sealedbox public key exist within 'PublicKeyCryptography' folder..");

            var algorithmOption = new Option<int>("--algorithm")
            {
                Description = "The symmetric encryption algorithm" + Environment.NewLine +
                "0=XSalsa20Poly1305, 1=XChaCha20Poly1305",
                DefaultValueFactory = _ => 0
            };

            var dataOption = new Option<string>("--data")
            {
                Description = "Input data to get sealedbox encrypted",
                DefaultValueFactory = _ => ""
            };

            var dataEncodingTypeOption = new Option<int>("--encoding")
            {
                Description = "The type of encoding for the data String. (0=UTF8/Unicode,1=Base64)",
                DefaultValueFactory = _ => 0
            };

            command.Options.Add(algorithmOption);
            command.Options.Add(dataOption);
            command.Options.Add(dataEncodingTypeOption);

            command.SetAction(parseResult =>
            {
                StandardizedDirectoriesFunction.InitializedDirectories();
                int AlgorithmChoice = parseResult.GetValue(algorithmOption)!;
                String DataToGetSealedBoxEncrypted = parseResult.GetValue(dataOption)!;
                int DataEncodingType = parseResult.GetValue(dataEncodingTypeOption)!;
                if (Directory.GetFileSystemEntries(StandardizedDirectoriesFunction.PKCRootFolder).Length > 0
                && Directory.GetFileSystemEntries(StandardizedDirectoriesFunction.PKCRootFolder).Length <= 4
                && File.Exists(StandardizedDirectoriesFunction.PKCRootFolder + "SealedBoxPublicKey.txt")==true)
                {
                    Byte[] DataToGetEncrypted = new Byte[] { };
                    Boolean AbleToConvertFromBase64 = true;
                    if (DataEncodingType == 0) 
                    {
                        DataToGetEncrypted = Encoding.UTF8.GetBytes(DataToGetSealedBoxEncrypted);
                    }
                    else 
                    {
                        try 
                        {
                            DataToGetEncrypted = Convert.FromBase64String(DataToGetSealedBoxEncrypted);
                        }
                        catch 
                        {
                            AbleToConvertFromBase64 = false;
                        }
                    }
                    Byte[] EncryptedData = new Byte[] { };
                    Byte[] SealedBoxPublicKey = File.ReadAllBytes(StandardizedDirectoriesFunction.PKCRootFolder + "SealedBoxPublicKey.txt");
                    if (AlgorithmChoice == 0) 
                    {
                        if(DataEncodingType == 1 && AbleToConvertFromBase64 == false) 
                        {
                            Console.WriteLine("Error: You have inputted the wrong base64 data to be encrypted..");
                        }
                        else
                        {
                            EncryptedData = SodiumSealedPublicKeyBox.Create(DataToGetEncrypted, SealedBoxPublicKey);
                        }
                        Console.WriteLine(Convert.ToBase64String(EncryptedData));
                    }
                    else 
                    {
                        if (DataEncodingType == 1 && AbleToConvertFromBase64 == false)
                        {
                            Console.WriteLine("Error: You have inputted the wrong base64 data to be encrypted..");
                        }
                        else
                        {
                            EncryptedData = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(DataToGetEncrypted, SealedBoxPublicKey);
                        }
                        Console.WriteLine(Convert.ToBase64String(EncryptedData));
                    }
                }
                else
                {
                    Console.WriteLine("Error: There's no public key material in 'PublicKeyCryptography' directory or the public key material count is greater than 4 or there's no sealedbox public key.");
                }
            });

            return command;
        }
    }
}
