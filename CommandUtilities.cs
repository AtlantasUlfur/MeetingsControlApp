using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VismaEntry;

namespace ConsoleUI
{
    internal class CommandUtilities
    {
        /// <summary>
        /// The login loop to get credentials from console and confirm them
        /// </summary>
        /// <param name="people"> the list of all the people in the database </param>
        /// <param name="LoginPerson"> returns a Person whose credentials match the inputs as a referenced argument </param>
        public static void LoginLoop(List<Person> people, out Person LoginPerson)
        {
            while (true)
            {
                Console.WriteLine("Enter your username");
                var username = Console.ReadLine();
                Console.WriteLine("Enter your password");
                var password = Console.ReadLine();

                var passwordMatches = PersonUtilities.CheckPassword(people, username, password);

                if (passwordMatches)
                {
                    LoginPerson = PersonUtilities.GetPersonByUsername(people, username);
                    break;
                }
                else
                {
                    ConsoleUtilities.printErrorToConsole("Password or username doesnt match");
                }
            }
        }
        /// <summary>
        /// Registers a new user to the database
        /// </summary>
        /// <param name="people"> the list of all the people in the database </param>
        /// <param name="log"> the Ilogger reference for logging </param>
        /// <param name="config"> the application configuration </param>
        public static void RegisterPerson(List<Person>people, ILogger<MeetingsService> log, IConfiguration config)
        {
            Console.WriteLine("Enter your first name");
            string name = Console.ReadLine();
            if (name == "")
            {
                ConsoleUtilities.printErrorToConsole("No name entered");
                return;
            }
            Console.WriteLine("Enter your last name");
            string surname = Console.ReadLine();
            if (surname == "")
            {
                ConsoleUtilities.printErrorToConsole("No last name entered");
                return;
            }

            Console.WriteLine("Enter your desired username");
            string username = Console.ReadLine();
            if (username == "")
            {
                ConsoleUtilities.printErrorToConsole("No usernamename entered");
                return;
            }
            if (!PersonUtilities.UsernameIsUnique(people, username))
            {
                ConsoleUtilities.printErrorToConsole("Username must be unique");
                return;
            }
            Console.WriteLine("Enter your password");
            string password = Console.ReadLine();
            if (password == "")
            {
                ConsoleUtilities.printErrorToConsole("No password entered");
                return;
            }
            if (PersonUtilities.AddPerson(people, name, surname, username, password))
            {
                log.LogInformation("A new person has been created");
            }
            else
            {
                log.LogWarning("Creation of a new person has failed");
            }

            try
            {
                PersonUtilities.SaveAllPeople(people, config["People_file"]);
                log.LogInformation("Newly created person has been saved to file");
            }
            catch (Exception)
            {
                log.LogWarning("Creation of a new person was not saved to file");
            }
        }
        /// <summary>
        /// The initial application loop to promt login or register loops
        /// </summary>
        /// <param name="people"> the list of all the people in the database </param>
        /// <param name="loginPerson"> returns the person whose credentials matched the inputs after the login has been completed </param>
        /// <param name="log"> the ILogger for logging actions </param>
        /// <param name="config"> application configuration </param>
        public static void LoginOrRegisterLoop(List<Person> people, out Person loginPerson, ILogger<MeetingsService> log, IConfiguration config)
        {
            while (true)
            {
                Console.WriteLine("Write Login to login or Register to register");

                var prompt = Console.ReadLine();
                if (prompt.ToLower() == "login")
                {
                    LoginLoop(people, out loginPerson);
                    break;
                }
                else if (prompt.ToLower() == "register")
                {
                    RegisterPerson(people, log, config);
                   
                }else
                {
                    ConsoleUtilities.printErrorToConsole("Enter login or register");
                }
            }
        }

    }
}
