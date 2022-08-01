using ConsoleUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VismaEntry
{
    public class Person
    {
        private static int lastId = -1;
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public string? Username { get; set; }

        public byte[]? Password { get; set; }

        public byte[]? Salt { get; set; }
        public byte[]? Iterations { get; set; }
        
        public Person()
        {
            Name = "";
            Id = 0;
            Surname = "";
        }
        public Person(string name, string surname)
        {

            this.Name = name;
            this.Surname = surname;
            Id = GenerateID();
        }
        public Person(int id, string name, string surname){
            Id = id;
            Name = name;
            Surname = surname;
        }

        public Person(string name, string surname, string username, string password)
        {

            this.Name = name;
            this.Surname = surname;
            Id = GenerateID();
            this.Username = username;

            var dict = PersonUtilities.encryptPassword(password);
            this.Password = dict["password"];
            this.Salt = dict["salt"];
            this.Iterations = dict["iterations"];
        }

        private static int GenerateID()
        {
            return Interlocked.Increment(ref lastId);
        }
        public static void SetLastID(int id)
        {
            lastId = id;
        }
        public override string ToString()
        {
            return String.Format("ID: {0} Full Name: {1} {2}",this.Id, this.Name, this.Surname);
        }
        public static void ResetID(List<Person> people)
        {
            int highest_id = -1;
            foreach (var person in people)
            {
                if (highest_id < person.Id)
                {
                    highest_id = person.Id;
                }
            }
            lastId = highest_id;
        }
    }
}
