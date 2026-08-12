using SHSM_CLI.DirectoryHelper;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_CLI.ActualCommands
{
    //Check whether the necessary files and documents exist..
    public static class CheckCommand
    {
        public static Command Create()
        {
            var command = new Command(
                "check",
                "Check if necessary information exist..");

            command.SetAction(_ =>
            {
                StandardizedDirectoriesFunction.CreateDirectoriesIfNotExist();
                bool ServerAPIIPAddressExist = File.Exists(StandardizedDirectoriesFunction.ServerRootFolder + "IP.txt") == true;
                bool AuthorizedUserExist = Directory.GetDirectories(StandardizedDirectoriesFunction.UsersRootFolder).Length > 0;
                //Kindly use the ":" as separator when calling this on CLI..
                //so that you can know whether your application can proceed..
                //This isn't a full check yet as Arweave ID checks have not been applied here due to some sort of infeasibility..
                Console.WriteLine("Passed the checks?:"+(ServerAPIIPAddressExist==true && AuthorizedUserExist==true).ToString());
            });

            return command;
        }
    }
}
