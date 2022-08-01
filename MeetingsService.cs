using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VismaEntry;

namespace ConsoleUI
{
    internal class MeetingsService : IMeetingsService
    {
        private readonly ILogger<MeetingsService> log;
        private readonly IConfiguration config;

        public MeetingsService(ILogger<MeetingsService> log, IConfiguration config)
        {
            this.log = log;
            this.config = config;
        }

        public async void Run()
        {
            string PEOPLEPATH = config["People_file"];
            string MEETINGSPATH = config["Meetings_file"];

            // Load all people into a list
            Person LoginPerson;
            var people = PersonUtilities.getAllPeople(PEOPLEPATH);
            log.LogInformation("All registered people have been loaded");
            // Load all meetings into a list
            var meetings = MeetingUtilities.getAllMeetings(MEETINGSPATH);
            log.LogInformation("All registered meetings have been loaded");
            //people.Add(new Person("name100", "surname100", "username100", "password100"));
            //PersonUtilities.SaveAllPeople(people, PEOPLEPATH);
            // Login loop
            while (true)
            {
                Console.WriteLine("Enter your username");
                var username = Console.ReadLine();
                Console.WriteLine("Enter your password");
                var password = Console.ReadLine();

                var passwordMatches = PersonUtilities.checkPassword(people, username, password);

                if (passwordMatches)
                {
                    LoginPerson = PersonUtilities.getPersonByUsername(people, username);
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Password or username doesnt match");
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }

            while (true)
            {
                Console.Write(LoginPerson.Username+"> ");
                var input = Console.ReadLine();

                if (input == null)
                {
                    continue;
                }
                if(input.ToLower() == "exit")
                {
                    Console.Clear();
                    break;
                }
                else
                {
                    var tokens = input.Split(" ");

                    /*foreach(var token in tokens)
                    {
                        Console.WriteLine(token);
                    }*/

                    // Listing all available commands
                    if (tokens.Length == 1 &&
                            (tokens[0].ToLower() == "-h"
                            || tokens[0].ToLower() == "help"
                            || tokens[0].ToLower() == "-help")
                    )
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("All available Commands: ");
                        Console.WriteLine("\n\"GET * people\"" +
                            "\n" +
                            "Returns All people registered in database \n");

                        Console.WriteLine("\"GET * meetings\""+
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
                        continue;
                    }
                    else if (tokens.Length == 2)
                    {
                        if (tokens[0].ToLower() == "delete"
                            && tokens[1].ToLower() == "meeting")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Add operand with meeting id: DELETE meeting {id}");
                            Console.WriteLine("Aborting command");
                            Console.ForegroundColor = ConsoleColor.White;
                            continue;
                        }
                        if (tokens[0].ToLower() == "filter"
                            && tokens[1].ToLower() == "meetings")
                        {
                            var tempMeetings = meetings;
                            // Filtering loop
                            while (true)
                            {
                                Console.WriteLine("Select a filter: (Description," +
                                    " resp.person, category, type, from," +
                                    " to, beetween, peopleCount)");
                                var filter = Console.ReadLine();
                                if(filter.ToLower() == "description" || filter.ToLower() == "desc")
                                {
                                    Console.WriteLine("Add description filter:");
                                }
                                else if (filter.ToLower() == "responsible person")
                                {
                                    Console.WriteLine("enter the id of responsible person");
                                }
                                else if (filter.ToLower() == "category")
                                {
                                    Console.WriteLine("enter a category (CodeMonkey / Hub / Short / TeamBuilding):");
                                }
                                else if (filter.ToLower() == "type")
                                {
                                    Console.WriteLine("enter a type (Live / InPerson)");
                                }
                                else if(filter.ToLower() == "from")
                                {
                                    Console.WriteLine("Enter a date:");
                                }
                                else if (filter.ToLower() == "to")
                                {
                                    Console.WriteLine("Enter a date:");
                                }
                                else if (filter.ToLower() == "between") { 
                                    Console.WriteLine("Enter a starting date:");
                                    Console.WriteLine("Enter an ending date:");
                                }
                                else if (filter.ToLower() == "peoplecount" || filter.ToLower() == "count")
                                {
                                    Console.WriteLine("Enter a number from which to filter: ");
                                }else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Please enter a valid filter");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }

                                Console.WriteLine();
                                Console.WriteLine("data");
                                Console.WriteLine();

                                Console.WriteLine("Would you like to add a new filter? (Y/N)");
                                var response = Console.ReadLine();
                                if(response == null)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Aborting command");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    continue;
                                }
                                else if(response.ToLower() is not "y" or not "yes")
                                {
                                    break;
                                }
                            }
                            continue;
                        }
                    }
                    else if (tokens.Length == 3)
                    {
                        if ((tokens[0].ToLower() == "add"
                            || tokens[0].ToLower() == "create")
                            && tokens[1].ToLower() == "new"
                            && tokens[2].ToLower() == "meeting")
                        {
                            if (MeetingUtilities.createMeeting(people, meetings, MEETINGSPATH))
                            {
                                log.LogInformation("A new meeting has been created");
                            }

                            continue;
                        }
                        if (tokens[0].ToLower() == "get"
                            && tokens[1] == "*"
                            && tokens[2].ToLower() == "people"
                        )
                        {
                            foreach (var person in people)
                            {
                                Console.WriteLine(person.ToString());
                            }
                            continue;
                        }
                        if (tokens[0].ToLower() == "get"
                            && tokens[1] == "*"
                            && tokens[2].ToLower() == "meetings"
                        )
                        {
                            foreach (var meeting in meetings)
                            {
                                Console.WriteLine();
                                Console.WriteLine(meeting.ToString());
                            }
                            continue;
                        }
                        if (tokens[0].ToLower() == "delete"
                            && tokens[1].ToLower() == "meeting")
                        {
                            if (!Int32.TryParse(tokens[2], out int id))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("id should be an integer: DELETE meeting {id}");
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }

                            var index = MeetingUtilities.GetMeetingIndex(meetings, id);
                            if (index == -1)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Meeting with given index does not exist");
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            if (!MeetingUtilities.userCanDeleteMeeting(meetings, index, LoginPerson))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("User {0} does not have permissions to delete meeting with index {1}",LoginPerson.Username,id);
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            if(MeetingUtilities.deleteMeeting(meetings, MEETINGSPATH, id))
                            {
                                log.LogInformation("Meeting with id: {meeting_id} has been deleted sucessfully", id);
                            }
                            else
                            {
                                log.LogInformation("Meeting with id: {meeting_id} could not be deleted", id);
                            }
                            continue;
                        }
                    }else if(tokens.Length == 6)
                    {
                        if ((tokens[0].ToLower() == "delete"
                            || tokens[0].ToLower() == "remove")
                          && tokens[1].ToLower() == "person"
                          && tokens[3].ToLower() == "from"
                          && tokens[4].ToLower() == "meeting")
                        {
                            if (!Int32.TryParse(tokens[2], out int personId))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("{person_id} should be an integer");
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            if (!Int32.TryParse(tokens[5], out int meetingId))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("{meeting_id} should be an integer");
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            if (!PersonUtilities.personExists(people, personId))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Person with id: {0} doesn't exist", personId);
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            if (!MeetingUtilities.MeetingExists(meetings, meetingId))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Meeting with id: {0} doesn't exist", meetingId);
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            var meeting = MeetingUtilities.GetMeeting(meetings, meetingId);
                            var person = PersonUtilities.findPerson(people, personId);
                            if (!MeetingUtilities.PersonIsAlreadyInAMeeting(meeting, person))
                            {

                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Person with id: {0} is not in a Meeting" +
                                    " with id: {1}", personId, meetingId);
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            if(MeetingUtilities.PersonIsAResponsiblePerson(meeting, person))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Person with id: {0} is a responsible person for meeting" +
                                    " with id: {1}", personId, meetingId);
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }

                            if (MeetingUtilities.DeletePersonFromAMeeting(meeting, person))
                            {
                                log.LogInformation("Person {person_id} has been removed from {meeting_id}", personId, meetingId);
                                try
                                {
                                    MeetingUtilities.SaveAllMeetings(meetings, MEETINGSPATH);
                                    log.LogInformation("Persons {person_id} deletion from meeting {meeting_id} has been saved to file",
                                    personId, meetingId);
                                }
                                catch (Exception)
                                {
                                    log.LogWarning("Persons {person_id} deletion from {meeting_id} could not be saved to file",
                                    personId, meetingId);
                                    continue;
                                }
                            }
                            else
                            {
                                log.LogWarning("Person {person_id} could not be removed from {meeting_id}", personId, meetingId);
                            }
                            continue;
                        }
                        ////////////////////////////////////////////////////////
                            if (tokens[0].ToLower() == "add" 
                            && tokens[1].ToLower() =="person"
                            && tokens[3].ToLower() =="to"
                            && tokens[4].ToLower() == "meeting")
                        {
                            if (!Int32.TryParse(tokens[2], out int personId))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("{person_id} should be an integer");
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            if (!Int32.TryParse(tokens[5], out int meetingId))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("{meeting_id} should be an integer");
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            if (!PersonUtilities.personExists(people, personId))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Person with id: {0} doesn't exist", personId);
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            if (!MeetingUtilities.MeetingExists(meetings, meetingId))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Meeting with id: {0} doesn't exist", meetingId);
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            var meeting = MeetingUtilities.GetMeeting(meetings, meetingId);
                            var person = PersonUtilities.findPerson(people, personId);
                            if (MeetingUtilities.PersonIsAlreadyInAMeeting(meeting, person))
                            {

                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Person with id: {0} is already in a Meeting" +
                                    " with id: {1} doesn't exist", personId,meetingId);
                                Console.WriteLine("Aborting command");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            if (MeetingUtilities.PersonIsBusy(person, meetings, meeting))
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Person is already in a meeting at given time");
                                Console.WriteLine("Are you sure you want to add person {0} to a meeting {1}? (Y/N)", personId, meetingId);
                                Console.ForegroundColor = ConsoleColor.White;
                                string answer = Console.ReadLine().ToLower();
                                if (answer != "y" || answer != "yes")
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Aborting command");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    continue;
                                }
                            }

                            if (MeetingUtilities.AddPersonToAMeeting(meeting, person,MEETINGSPATH))
                            {
                                log.LogInformation("Person with id {person_id} has been added to a meeting with id {meeting_id}",
                                    personId, meetingId);
                                try
                                {
                                    MeetingUtilities.SaveAllMeetings(meetings, MEETINGSPATH);
                                    log.LogInformation("Person with id {person_id} addition to {meeting_id} has been saved to file",
                                    personId, meetingId);
                                }
                                catch (Exception)
                                {
                                    log.LogWarning("Person with id {person_id} addition to {meeting_id} could not be saved to file",
                                    personId, meetingId);
                                    continue;
                                }
                            }else
                            {
                                log.LogInformation("Person with id {person_id} could not be added to a meeting with id {meeting_id}",
                                    personId, meetingId);
                            }
                            continue;
                        }
                    }

                    // If no command was found
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Command not found write -h or help for all available commands");
                    Console.ForegroundColor = ConsoleColor.White;
                    
                }
            }
        }
    }
}