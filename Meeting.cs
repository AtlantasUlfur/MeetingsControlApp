using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VismaEntry
{
    internal class Meeting
    {
        private static int lastId = -1;
        public int ID { get; set; }
        public string Name { get; set; }

        public Person ResponsiblePerson { get; set; }

        public string Description { get; set; }

        public Category Category { get; set; }

        public Type Type { get; set;}

        public List<Person> People { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Meeting() {
            People = new List<Person>();
        }
        public Meeting(string name, Person responsiblePesron,
            string description, Category category, Type type, DateTime startDate, DateTime endDate)
        {
            Name = name;
            ResponsiblePerson = responsiblePesron;
            Description = description; 
            Category = category;
            Type = type;
            People = new List<Person>();
            StartDate = startDate;
            EndDate = endDate;
            ID = generateID();
            addPerson(responsiblePesron);
        }
        private int generateID()
        {
            return Interlocked.Increment(ref lastId);
        }
        public void SetLastID(int id)
        {
            lastId = id;
        }
        public void addPerson(Person person)
        {
            People.Add(person);
            return;
        }
        public void removePerson(Person person)
        {
            var id = People.FindIndex(x => x.Id == person.Id);
            if (id != -1)
            {
                People.RemoveAt(id);
            }else
            {
                throw new ArgumentException("No such person at the meeting");
            }
        }
        public override string ToString()
        {
            string returnString = String.Format("ID: {0}" +
                "\nName: {1}" +
                "\nResponsible person: \'{2}\' " +
                "\nDescription: {3}" +
                "\nCategory: {4}" +
                "\nType: {5}" +
                "\nStart Time: {6}" +
                "\nEnd Time: {7}" +
                "\nPeople in a meeting:"
                , ID, Name, ResponsiblePerson, Description,
                Category, Type, StartDate, EndDate);
           foreach(Person person in People)
           {
                returnString += String.Format("\n{0}", person.ToString());
           }
            return returnString;
        }
        public static void ResetID(List<Meeting> meetings)
        {
            int highest_id = -1;
            foreach(var meeting in meetings)
            {
                if(highest_id < meeting.ID)
                {
                    highest_id = meeting.ID;
                }
            }
            lastId = highest_id;
        }
    }
}
