using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_CLI.Templates
{
    //SHSM_CLI sum 10 20
    //example calling..
    public static class SumCommand
    {
        public static Command Create()
        {
            var command = new Command(
                "sum",
                "Add two numbers");

            var firstNumber = new Argument<int>("first")
            {
                Description = "The first number"
            };

            var secondNumber = new Argument<int>("second")
            {
                Description = "The second number"
            };

            command.Arguments.Add(firstNumber);
            command.Arguments.Add(secondNumber);

            command.SetAction(parseResult =>
            {
                int first = parseResult.GetValue(firstNumber);
                int second = parseResult.GetValue(secondNumber);

                Console.WriteLine(first + second);
            });

            return command;
        }
    }
}
