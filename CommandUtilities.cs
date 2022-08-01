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
                    ConsoleUtilities.PrintErrorToConsole("Password or username doesnt match");
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
                ConsoleUtilities.PrintErrorToConsole("No name entered");
                return;
            }
            Console.WriteLine("Enter your last name");
            string surname = Console.ReadLine();
            if (surname == "")
            {
                ConsoleUtilities.PrintErrorToConsole("No last name entered");
                return;
            }

            Console.WriteLine("Enter your desired username");
            string username = Console.ReadLine();
            if (username == "")
            {
                ConsoleUtilities.PrintErrorToConsole("No usernamename entered");
                return;
            }
            if (!PersonUtilities.UsernameIsUnique(people, username))
            {
                ConsoleUtilities.PrintErrorToConsole("Username must be unique");
                return;
            }
            Console.WriteLine("Enter your password");
            string password = Console.ReadLine();
            if (password == "")
            {
                ConsoleUtilities.PrintErrorToConsole("No password entered");
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
                    ConsoleUtilities.PrintErrorToConsole("Enter login or register");
                }
            }
        }

        /// <summary>
        /// Lists all the available commands to the console
        /// </summary>
        public static void RunHelpCommand()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("All available Commands: ");
            Console.WriteLine("\n\"GET * people\"" +
                "\n" +
                "Returns All people registered in database \n");

            Console.WriteLine("\"GET * meetings\"" +
                "\n" +
                "Returns All meetings fom the database \n");

            Console.WriteLine("\"[CREATE\\ADD] new meeting\"" +
               "\n" +
               "Will promt you to input information to create a new meeting \n");

            Console.WriteLine("\"DELETE meeting {id}\"" +
               "\n" +
               "Deletes a meeting with a given id from database\n" +
               "{id} - integer\n");


            Console.WriteLine("\"ADD person {person_id} TO meeting {meeting_id}\"" +
               "\n" +
               "Deletes a meeting with a given id from database\n" +
               "{person_id} - integer\n" +
               "{meeting_id} - integer\n");

            Console.WriteLine("\"[DELETE/REMOVE] person {person_id} FROM meeting {meeting_id}\"" +
              "\n" +
              "Deletes a meeting with a given id from database\n" +
              "{person_id} - integer\n" +
              "{meeting_id} - integer\n");

            Console.WriteLine("\"FILTER meetings\"" +
             "\n" +
             "Prompts to add a filter to show meetings\n");

            Console.ForegroundColor = ConsoleColor.White;
        }
        /// <summary>
        /// Deletes a meeting from the database
        /// </summary>
        /// <param name="meetings"> the list of all the meetings in the database</param>
        /// <param name="meeting_id_token"> the user input for meeting_id </param>
        /// <param name="LoginPerson"> the logged in user </param>
        /// <param name="log"> Ilogger for logging </param>
        /// <param name="config"> application configuration </param>
        public static void RunDeleteMeetingCommand(List<Meeting> meetings, 
            string meeting_id_token, Person LoginPerson,
            ILogger<MeetingsService> log, IConfiguration config)
        {
            // check if the token is an integer
            if (!Int32.TryParse(meeting_id_token, out int id))
            {
                ConsoleUtilities.PrintErrorToConsole(new string[2]
                {
                    "id should be an integer: DELETE meeting {id}",
                    "Aborting command"
                });
                return;
            }

            var index = MeetingUtilities.GetMeetingIndex(meetings, id);
            // check if the meeting exists
            if (index == -1)
            {
                ConsoleUtilities.PrintErrorToConsole(new string[2]
                {
                   "Meeting with given index does not exist",
                   "Aborting command"
                });
                return;
            }
            if (!MeetingUtilities.UserCanDeleteMeeting(meetings, index, LoginPerson))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("User {0} does not have permissions to delete meeting with index {1}", LoginPerson.Username, id);
                Console.WriteLine("Aborting command");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            if (MeetingUtilities.DeleteMeeting(meetings, config["Meetings_file"], id))
            {
                log.LogInformation("Meeting with id: {meeting_id} has been deleted sucessfully", id);
            }
            else
            {
                log.LogInformation("Meeting with id: {meeting_id} could not be deleted", id);
            }
        }

        /// <summary>
        /// Deletes a person from a meeting
        /// </summary>
        /// <param name="person_id_token">the token user has input for user id</param>
        /// <param name="meeting_id_token">the token user has input for meeting id</param>
        /// <param name="people"> lsit of all people in the database </param>
        /// <param name="meetings"> list of all meetings in the database </param>
        /// <param name="log"> Ilogger for logging </param>
        /// <param name="config"> application configuration </param>
        public static void RunDeletePersonFromMeetingCommand(string person_id_token, string meeting_id_token,
            List<Person> people, List<Meeting> meetings,
            ILogger<MeetingsService> log, IConfiguration config)
        {
            // check if person_id is an integer
            if (!Int32.TryParse(person_id_token, out int personId))
            {
                ConsoleUtilities.PrintErrorToConsole(new string[2]
                {
                    "{person_id} should be an integer",
                    "Aborting command"
                });
                return;
            }
            // check if meeting_id is an integer
            if (!Int32.TryParse(meeting_id_token, out int meetingId))
            {
                ConsoleUtilities.PrintErrorToConsole(new string[2]
                {
                    "{meeting_id} should be an integer",
                    "Aborting command"
                });
                return;
            }
            // check if person exists
            if (!PersonUtilities.PersonExists(people, personId))
            {
                ConsoleUtilities.PrintErrorToConsole(new string[2]
               {
                    String.Format("Person with id: {0} doesn't exist", personId),
                    "Aborting command"
               });
                return;
            }
            // check if meeting exists
            if (!MeetingUtilities.MeetingExists(meetings, meetingId))
            {
                ConsoleUtilities.PrintErrorToConsole(new string[2]
                {
                    String.Format("Meeting with id: {0} doesn't exist", meetingId),
                    "Aborting command"
                });
                return;
            }
            // get the meeting and the person
            var meeting = MeetingUtilities.GetMeeting(meetings, meetingId);
            var person = PersonUtilities.FindPerson(people, personId);

            // check if the person is actually registered in the meeting
            if (!MeetingUtilities.PersonIsAlreadyInAMeeting(meeting, person))
            {

                ConsoleUtilities.PrintErrorToConsole(new string[2]
                {
                    String.Format("Person with id: {0} is not in a Meeting  with id: {1}", personId, meetingId),
                    "Aborting command"
                });
                return;
            }
            // check if the person is not the person responsible for the meeting
            if (MeetingUtilities.PersonIsAResponsiblePerson(meeting, person))
            {
                ConsoleUtilities.PrintErrorToConsole(new string[2]
                {
                    String.Format("Person with id: {0} is a responsible person for meeting with id: {1}", personId, meetingId),
                    "Aborting command"
                });
                return;
            }

            // check if the person was succesfully deleted from the meeting
            if (MeetingUtilities.DeletePersonFromAMeeting(meeting, person))
            {
                log.LogInformation("Person {person_id} has been removed from {meeting_id}", personId, meetingId);
                try
                {
                    MeetingUtilities.SaveAllMeetings(meetings, config["Meetings_file"]);
                    log.LogInformation("Persons {person_id} deletion from meeting {meeting_id} has been saved to file",
                    personId, meetingId);
                }
                catch (Exception)
                {
                    log.LogWarning("Persons {person_id} deletion from {meeting_id} could not be saved to file",
                    personId, meetingId);
                    return;
                }
            }
            else
            {
                log.LogWarning("Person {person_id} could not be removed from {meeting_id}", personId, meetingId);
            }
        }
        /// <summary>
        /// Adds a person to a meeting
        /// </summary>
        /// <param name="person_id_token">the token user has input for user id</param>
        /// <param name="meeting_id_token">the token user has input for meeting id</param>
        /// <param name="people"> lsit of all people in the database </param>
        /// <param name="meetings"> list of all meetings in the database </param>
        /// <param name="log"> Ilogger for logging </param>
        /// <param name="config"> application configuration </param>
        public static void RunAddPersonToMeetingCommand(string person_id_token, string meeting_id_token,
            List<Person> people, List<Meeting> meetings,
            ILogger<MeetingsService> log, IConfiguration config)
        {
            // check if {person_id} is an integer
            if (!Int32.TryParse(person_id_token, out int personId))
            {
                ConsoleUtilities.PrintErrorToConsole(new string[2]
                {
                    "{person_id} should be an integer",
                    "Aborting command"
                });
                return;
            }
            // check if {meeting_id} is an integer
            if (!Int32.TryParse(meeting_id_token, out int meetingId))
            {
                ConsoleUtilities.PrintErrorToConsole(new string[2]
                {
                    "{meeting_id} should be an integer",
                    "Aborting command"
                });
                return;
            }
            // check if person with {person_id} exists
            if (!PersonUtilities.PersonExists(people, personId))
            {
                ConsoleUtilities.PrintErrorToConsole(new string[2]
                {
                    String.Format("Person with id: {0} doesn't exist", personId),
                    "Aborting command"
                });
                return;
            }
            // check if meeting with {meeting_id} exists
            if (!MeetingUtilities.MeetingExists(meetings, meetingId))
            {
                ConsoleUtilities.PrintErrorToConsole(new string[2]
                {
                    String.Format("Meeting with id: {0} doesn't exist", meetingId),
                    "Aborting command"
                });
                return;
            }
            // get meeting and person
            var meeting = MeetingUtilities.GetMeeting(meetings, meetingId);
            var person = PersonUtilities.FindPerson(people, personId);

            // check if the person is already in the meeting
            if (MeetingUtilities.PersonIsAlreadyInAMeeting(meeting, person))
            {

                ConsoleUtilities.PrintErrorToConsole(new string[2]
                {
                    String.Format("Person with id: {0} is already in a Meeting with id: {1} doesn't exist", personId, meetingId),
                    "Aborting command"
                });
                return;
            }
            // check if the person is registered for other meeting at that time
            if (MeetingUtilities.PersonIsBusy(person, meetings, meeting))
            {
                ConsoleUtilities.PrintWarningToConsole(new string[2]
               {
                   "Person is already in a meeting at given time",
                    String.Format("Are you sure you want to add person {0} to a meeting {1}? (Y/N)", personId, meetingId)
               });

                string answer = Console.ReadLine().ToLower();
                if (answer != "y" || answer != "yes")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Aborting command");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            // check if the person was sucessfully added to the meeting
            if (MeetingUtilities.AddPersonToAMeeting(meeting, person))
            {
                log.LogInformation("Person with id {person_id} has been added to a meeting with id {meeting_id}",
                    personId, meetingId);
                try
                {
                    MeetingUtilities.SaveAllMeetings(meetings, config["Meetings_file"]);
                    log.LogInformation("Person with id {person_id} addition to {meeting_id} has been saved to file",
                    personId, meetingId);
                }
                catch (Exception)
                {
                    log.LogWarning("Person with id {person_id} addition to {meeting_id} could not be saved to file",
                    personId, meetingId);
                    return;
                }
            }
            else
            {
                log.LogInformation("Person with id {person_id} could not be added to a meeting with id {meeting_id}",
                    personId, meetingId);
            }
        }
        /// <summary>
        /// Runs the filter meetings loop and periodically prints filtered meetings to the console
        /// </summary>
        /// <param name="meetings"> the list of all meetings in the database </param>
        /// <param name="people"> the list of all people in the database </param>
        public static void RunFilteringMeetingsCommand(List<Meeting> meetings, List<Person> people)
        {
            var tempMeetings = meetings;
            while (true)
            {
                Console.WriteLine("Select a filter: (Description," +
                    " resp.person, category, type, from," +
                    " to, beetween, peopleCount)");
                var filter = Console.ReadLine();
                if (filter.ToLower() == "description" || filter.ToLower() == "desc")
                {
                    Console.WriteLine("Add description filter:");
                    var description_filter = Console.ReadLine();
                    tempMeetings = tempMeetings.Where(x => x.Description.Contains(description_filter)).ToList();
                }
                else if (filter.ToLower() == "responsible person" || filter.ToLower() == "resp.person")
                {
                    Console.WriteLine("enter the id of responsible person");
                    if (!Int32.TryParse(Console.ReadLine(), out int responsible_person_id))
                    {
                        ConsoleUtilities.PrintWarningToConsole("id should be an integer!");
                        continue;
                    }
                    else
                    {
                        if (PersonUtilities.FindPerson(people, responsible_person_id) == null)
                        {
                            ConsoleUtilities.PrintWarningToConsole(
                                String.Format("person with id: {0} doesn't exist", responsible_person_id
                            ));
                            continue;
                        }
                        else
                        {
                            tempMeetings = tempMeetings.Where(x => x.ResponsiblePerson.Id == responsible_person_id).ToList();
                        }
                    }
                }
                else if (filter.ToLower() == "category")
                {
                    Console.WriteLine("enter a category (CodeMonkey / Hub / Short / TeamBuilding):");

                    if (!Enum.TryParse<Category>(Console.ReadLine(), true, out Category category))
                    {
                        ConsoleUtilities.PrintWarningToConsole("Enter a valid category");
                        continue;
                    }
                    else
                    {
                        tempMeetings = tempMeetings.Where(x => x.Category == category).ToList();
                    }

                }
                else if (filter.ToLower() == "type")
                {
                    Console.WriteLine("enter a type (Live / InPerson):");
                    if (!Enum.TryParse<VismaEntry.Type>(Console.ReadLine(), true, out VismaEntry.Type type))
                    {
                        ConsoleUtilities.PrintWarningToConsole("Enter a valid type");
                        continue;
                    }
                    else
                    {
                        tempMeetings = tempMeetings.Where(x => x.Type == type).ToList();
                    }
                }
                else if (filter.ToLower() == "from")
                {
                    Console.WriteLine("Enter a date:");
                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime startTime))
                    {
                        ConsoleUtilities.PrintWarningToConsole("Enter a valid starting time");
                        continue;
                    }
                    else
                    {
                        tempMeetings = tempMeetings.Where(x => x.StartDate > startTime).ToList();
                    }
                }
                else if (filter.ToLower() == "to")
                {
                    Console.WriteLine("Enter a date:");
                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime endTime))
                    {
                        ConsoleUtilities.PrintWarningToConsole("Enter a valid starting time");
                        continue;
                    }
                    else
                    {
                        tempMeetings = tempMeetings.Where(x => x.EndDate < endTime).ToList();
                    }
                }
                else if (filter.ToLower() == "between")
                {
                    Console.WriteLine("Enter a starting date:");
                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime startTime))
                    {
                        ConsoleUtilities.PrintWarningToConsole("Enter a valid starting time");
                        continue;
                    }
                    else
                    {
                        tempMeetings = tempMeetings.Where(x => x.StartDate > startTime).ToList();
                    }
                    Console.WriteLine("Enter an ending date:");
                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime endTime))
                    {
                        ConsoleUtilities.PrintWarningToConsole("Enter a valid starting time");
                        continue;
                    }
                    else
                    {
                        tempMeetings = tempMeetings.Where(x => x.EndDate < endTime).ToList();
                    }
                }
                else if (filter.ToLower() == "peoplecount" || filter.ToLower() == "count")
                {
                    Console.WriteLine("Enter a number from which to filter: ");
                    if (!Int32.TryParse(Console.ReadLine(), out int count))
                    {
                        ConsoleUtilities.PrintWarningToConsole("count should be an integer!");
                        continue;
                    }
                    else
                    {
                        tempMeetings = tempMeetings.Where(x => x.People.Count >= count).ToList();
                    }
                }
                else
                {
                    ConsoleUtilities.PrintWarningToConsole("Please enter a valid filter");
                    continue;
                }

                // Prints the filtered meetings
                Console.WriteLine("\nFiltered Meetings:\n");
                foreach (var meeting in tempMeetings)
                {
                    Console.WriteLine(meeting.ToString());
                    Console.WriteLine();
                }

                Console.WriteLine("Would you like to add a new filter? (Y/N)");


                var response = Console.ReadLine();
                if (response == "")
                {
                    ConsoleUtilities.PrintErrorToConsole("Aborting command");
                    break;
                }
                else if (response.ToLower() == "y" || response.ToLower() == "yes")
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
