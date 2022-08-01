using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VismaEntry;

namespace ConsoleUI
{
    internal static class MeetingUtilities
    {
        public static List<Meeting> GetAllMeetings(string path)
        {
            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<Meeting>>(json);
            }
            catch (Exception)
            {
                return new List<Meeting>();
            }
        }

        public static void SaveAllMeetings(List<Meeting> list, string path)
        {
            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        public static int GetMeetingIndex(List<Meeting> meetings, int id)
        {
            return meetings.FindIndex(x => x.ID == id);
        }
        public static bool UserCanDeleteMeeting(List<Meeting> meetings, int index, Person user)
        {
            return meetings[index].ResponsiblePerson.Id == user.Id;
        }
        public static bool DeleteMeeting(List<Meeting> meetings, string path, int index)
        {
            if(index < -1 || index >= meetings.Count)
            {
                return false;
            }else
            {
                meetings.RemoveAt(index);
                SaveAllMeetings(meetings,path);
                return true;
            }
        }
        public static bool CreateMeeting(List<Person> people, List<Meeting> meetings,string path)
        {
            // Get the name of the meeting
            Console.WriteLine("Enter name of the meeting:");
            var name = Console.ReadLine();

            // Get and check if the responsible person exists and is unambigious
            Console.WriteLine("Enter the name or id of the person responsible for the meeting:");
            var found_people = PersonUtilities.FindPerson(people, Console.ReadLine());
            if(found_people == null)
            {
                Console.WriteLine("Person not found");
                return false;
            }else if(found_people.Count == 0)
            {
                Console.WriteLine("Person not found");
                return false;
            }else if(found_people.Count > 1)
            {
                Console.WriteLine("Responsible person is ambigious, multiple poeple found, try using id instead.");
                Console.WriteLine("People found:");
                foreach(Person person in found_people)
                {
                    Console.WriteLine(person.ToString());
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Aborting command");
                Console.ForegroundColor = ConsoleColor.White;
                return false;
            }

            // Get the description of the meeting
            Console.WriteLine("Enter the description of the meeting:");
            var description = Console.ReadLine();

            //Get teh category of the meeting
            Console.WriteLine("Enter the Category of the meeting (CodeMonkey / Hub / Short / TeamBuilding):");

            if (!Enum.TryParse<Category>(Console.ReadLine(), true, out Category category))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Enter a valid category");
                Console.WriteLine("Aborting command");
                Console.ForegroundColor = ConsoleColor.White;
                return false;
            }

            //Get the Type of the meeting
            Console.WriteLine("Enter the type of the meeting (Live / InPerson):");

            if (!Enum.TryParse<VismaEntry.Type>(Console.ReadLine(), true, out VismaEntry.Type type))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Enter a valid type");
                Console.WriteLine("Aborting command");
                Console.ForegroundColor = ConsoleColor.White;
                return false;
            }

            Console.WriteLine("Enter the start time of the meeting:");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startTime))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Enter a valid starting time");
                Console.WriteLine("Aborting command");
                Console.ForegroundColor = ConsoleColor.White;
                return false;
            }

            Console.WriteLine("Enter the end time of the meeting:");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endTime))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Enter a valid ending time");
                Console.WriteLine("Aborting command");
                Console.ForegroundColor = ConsoleColor.White;
                return false;
            }
            if (startTime > endTime)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ending time is earlier than start time!");
                Console.WriteLine("Aborting command");
                Console.ForegroundColor = ConsoleColor.White;
                return false;
            }

            Meeting.ResetID(meetings);
            Meeting meeting = new(name, found_people[0],description,category,type,startTime,endTime);
            try
            {
                meetings.Add(meeting);
                SaveAllMeetings(meetings, path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool PersonIsBusy(Person person, List<Meeting> meetings, Meeting meeting)
        {
            var startTime = meeting.StartDate;
            var endTime = meeting.EndDate;
            foreach(var temp_meeting in meetings)
            {
                // if person is in a meeting check the times 
                if(temp_meeting.People.Where(x => x.Id == person.Id).Any())
                {
                    if (temp_meeting.StartDate <= startTime && temp_meeting.EndDate >= startTime)
                    {
                        return true;
                    }
                    if (temp_meeting.StartDate <= endTime && temp_meeting.EndDate >= endTime)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool PersonIsAResponsiblePerson(Meeting meeting, Person person)
        {
            return (meeting.ResponsiblePerson.Id == person.Id);
        }
        public static bool DeletePersonFromAMeeting(Meeting meeting, Person person)
        {
            try {
                meeting.People.RemoveAt(meeting.People.FindIndex(x => x.Id == person.Id));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool PersonIsAlreadyInAMeeting(Meeting meeting, Person person)
        {
            // Check if the persons id is already in the meetings people list
            if(meeting.People.Where(x => x.Id == person.Id).Any())
            {
                return true;
            }
            return false;
        }
        public static bool MeetingExists(List<Meeting> meetings, int id)
        {
            if(meetings.Where(x => x.ID == id).Any())
            {
                return true;
            }return false;
        }
        public static Meeting GetMeeting(List<Meeting> meetings, int id)
        {
            return meetings.Where(x => x.ID == id).First();
        }
        public static bool AddPersonToAMeeting(Meeting meeting, Person person)
        {
            try
            {
                meeting.People.Add(person);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
