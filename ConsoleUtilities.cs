using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUI
{
    internal class ConsoleUtilities
    {
        /// <summary>
        /// Prints a colored text to console and resets console.ForegroundColor to the original value
        /// </summary>
        /// <param name="output"> The text to output to console </param>
        /// <param name="outputColor"> Color of the text </param>
        public static void printColoredText(string output, ConsoleColor outputColor)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = outputColor;
            Console.WriteLine(output);
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Prints a string array as a different line for each element to the console and resets console.ForegroundColor to the original value
        /// </summary>
        /// <param name="output"> string lines to output to console </param>
        /// <param name="outputColor"> Color of the text </param>
        public static void printColoredText(string[] output, ConsoleColor outputColor)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = outputColor;
            foreach(string line in output)
            {
                Console.WriteLine(line);
            }
            Console.ForegroundColor = originalColor;
        }
        /// <summary>
        /// Prints a yellow text to console
        /// </summary>
        /// <param name="output">The line to print</param>
        /// <seealso cref="System.String"> </seealso>
        public static void printWarningToConsole(string output)
        {
            printColoredText(output, ConsoleColor.Yellow);
        }
        /// <summary>
        /// Prints a yellow line for each element of the array to the console
        /// </summary>
        /// <param name="output">The line to print</param>
        /// <seealso cref="System.String"> </seealso>
        public static void printWarningToConsole(string[] output)
        {
            printColoredText(output, ConsoleColor.Yellow);
        }
        /// <summary>
        /// Prints a red text to console
        /// </summary>
        /// <param name="output">The line to print</param>
        /// <seealso cref="System.String"> </seealso>
        public static void printErrorToConsole(string output)
        {
            printColoredText(output, ConsoleColor.Red);
        }
        /// <summary>
        /// Prints a red line for each element of the array to the console
        /// </summary>
        /// <param name="output">The line to print</param>
        /// <seealso cref="System.String"> </seealso>
        public static void printErrorToConsole(string[] output)
        {
            printColoredText(output, ConsoleColor.Red);
        }
    }
}
