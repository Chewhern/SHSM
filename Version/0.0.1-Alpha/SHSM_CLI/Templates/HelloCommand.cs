using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;

namespace SHSM_CLI.Templates
{
    public static class HelloCommand
    {
        public static Command Create()
        {
            var command = new Command(
                "hello",
                "Say hello to someone");

            var nameOption = new Option<string>("--name")
            {
                Description = "The name of the person to greet",
                DefaultValueFactory = _ => "World"
            };

            command.Options.Add(nameOption);

            command.SetAction(parseResult =>
            {
                string name = parseResult.GetValue(nameOption)!;

                Console.WriteLine($"Hello {name}!");
            });

            return command;
        }
    }
}
