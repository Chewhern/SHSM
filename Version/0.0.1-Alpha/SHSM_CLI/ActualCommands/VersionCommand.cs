using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_CLI.ActualCommands
{
    public static class VersionCommand
    {
        public static Command Create()
        {
            var command = new Command(
                "version",
                "Display the application version");

            command.SetAction(_ =>
            {
                Console.WriteLine("SHSM_Client CLI v0.0.1-alpha");
            });

            return command;
        }
    }
}
