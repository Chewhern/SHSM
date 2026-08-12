using SHSM_CLI.ActualCommands;
using System.CommandLine;

var rootCommand = new RootCommand("SHSM interoperability CLI client");

rootCommand.Subcommands.Add(CheckCommand.Create());
rootCommand.Subcommands.Add(GenerateAuthorizedUserInfoCommand.Create());
rootCommand.Subcommands.Add(GenerateSubKeyMaterialCommand.Create());
rootCommand.Subcommands.Add(PublicKeyCryptographySealedBoxEncryptDataCommand.Create());
rootCommand.Subcommands.Add(PublicKeyCryptographySealedBoxDecryptDataCommand.Create());
rootCommand.Subcommands.Add(PublicKeyCryptographySignDataCommand.Create());
rootCommand.Subcommands.Add(PublicKeyCryptographyVerifyDataCommand.Create());
rootCommand.Subcommands.Add(SecretKeyCryptographyInitializeCommand.Create());
rootCommand.Subcommands.Add(SecretKeyCryptographyEncryptDataCommand.Create());
rootCommand.Subcommands.Add(SecretKeyCryptographyDecryptDataCommand.Create());
rootCommand.Subcommands.Add(UploadDataToArweaveCommand.Create());
rootCommand.Subcommands.Add(VersionCommand.Create());

return rootCommand.Parse(args).Invoke();
