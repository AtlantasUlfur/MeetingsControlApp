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
            var people = PersonUtilities.GetAllPeople(PEOPLEPATH);
            log.LogInformation("All registered people have been loaded");

            // Load all meetings into a list
            var meetings = MeetingUtilities.GetAllMeetings(MEETINGSPATH);
            log.LogInformation("All registered meetings have been loaded");

            //Login or register prompt
            CommandUtilities.LoginOrRegisterLoop(people, out Person LoginPerson, log, config);

            // logic loop
            while (true)
            {
                // ask for command input
                Console.Write(LoginPerson.Username+"> ");
                var input = Console.ReadLine();


                // check if the command matches any known commands
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
                    // split the input into tokens
                    var tokens = input.Split(" ");

                    // check for help command
                    if (tokens.Length == 1 &&
                            (tokens[0].ToLower() == "-h"
                            || tokens[0].ToLower() == "help"
                            || tokens[0].ToLower() == "-help")
                    )
                    {
                        CommandUtilities.RunHelpCommand();
                        continue;
                    }
                    
                    else if (tokens.Length == 2)
                    {
                        // check for incorrect delete meeting command
                        if (tokens[0].ToLower() == "delete"
                            && tokens[1].ToLower() == "meeting")
                        {
                            ConsoleUtilities.PrintWarningToConsole(new string[2]
                            {
                                "Add operand with meeting id: DELETE meeting {id}",
                                "Aborting command"
                            });
                            continue;
                        }
                        // check for filter meetings command
                        if (tokens[0].ToLower() == "filter"
                            && tokens[1].ToLower() == "meetings")
                        {

                            CommandUtilities.RunFilteringMeetingsCommand(meetings, people);
                            
                            continue;
                        }
                    }
                    else if (tokens.Length == 3)
                    {
                        // check for create new meeting command
                        if ((tokens[0].ToLower() == "add"
                            || tokens[0].ToLower() == "create")
                            && tokens[1].ToLower() == "new"
                            && tokens[2].ToLower() == "meeting")
                        {
                            if (MeetingUtilities.CreateMeeting(people, meetings, MEETINGSPATH))
                            {
                                log.LogInformation("A new meeting has been created");
                            }

                            continue;
                        }
                        // check for get * people command
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
                        // check for get * meetings command
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
                        // check for delete meeting {id} command
                        if (tokens[0].ToLower() == "delete"
                            && tokens[1].ToLower() == "meeting")
                        {
                            CommandUtilities.RunDeleteMeetingCommand(meetings, tokens[2], LoginPerson, log, config);
                            continue;
                        }
                    }
                    else if(tokens.Length == 6)
                    {   
                        // check for remove person {person_id} from meeting {meeting_id} command
                        if ((tokens[0].ToLower() == "delete" || tokens[0].ToLower() == "remove")
                          && tokens[1].ToLower() == "person" && tokens[3].ToLower() == "from"
                          && tokens[4].ToLower() == "meeting")
                        {
                            CommandUtilities.RunDeletePersonFromMeetingCommand(tokens[2], tokens[5], people,
                                meetings, log, config);
                            continue;
                        }
                        // check fo rthe add person {person_id} to meeting {meeting_id} command
                        if (tokens[0].ToLower() == "add" && tokens[1].ToLower() =="person"
                            && tokens[3].ToLower() =="to" && tokens[4].ToLower() == "meeting")
                        {
                            CommandUtilities.RunAddPersonToMeetingCommand(tokens[2], tokens[5], people,
                                meetings, log, config);
                            continue;
                        }
                    }

                    // If no command was found
                    ConsoleUtilities.PrintErrorToConsole("Command not found write -h or help for all available commands");
                }
            }
        }
    }
}