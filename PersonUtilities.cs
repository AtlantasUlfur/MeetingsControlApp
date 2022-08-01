using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VismaEntry;

namespace ConsoleUI
{
    internal static class PersonUtilities
    {
        public static List<Person> GetAllPeople(string path)
        {
            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<Person>>(json);
            }
            catch (Exception)
            {
                return new List<Person>();
            }
        }
        public static bool UsernameIsUnique(List<Person> people, string username )
        {
            foreach(var person in people)
            {
                if(person.Username == username)
                {
                    return false;
                }
            }
            return true;
        }
        public static void SaveAllPeople(List<Person> list, string path)
        {
            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        public static bool AddPerson(List<Person> people, string name, string surname, string username, string password)
        {
            Person.ResetID(people);
            try
            {
                people.Add(new Person(name, surname, username, password));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
           
        }
        public static IDictionary<string, byte[]> EncryptPassword(string password)
        {
            Rfc2898DeriveBytes PBKDF2 = new(password, 8, 20);    //Hash the password with a 8 byte salt
            byte[] hashedPassword = PBKDF2.GetBytes(20);    //Returns a 20 byte hash
            byte[] salt = PBKDF2.Salt;

            int intValue = 20;
            byte[] intBytes = BitConverter.GetBytes(intValue);
            if (BitConverter.IsLittleEndian) { 
                Array.Reverse(intBytes);
             }
            byte[] result = intBytes;

            IDictionary<string, byte[]> returnDict = new Dictionary<string, byte[]>
            {
                ["password"] = hashedPassword,
                ["salt"] = salt,
                ["iterations"] = result
            };

            return returnDict;
        }
        public static byte[] EncryptPassword(string password, byte[] salt, byte[] iteration)
        {

            //int numberOfIterations = BitConverter.ToInt32(iteration, 0);

            Rfc2898DeriveBytes PBKDF2 = new(password, salt, 20);    //Hash the password with the users salt
            byte[] hashedPassword = PBKDF2.GetBytes(20);    //Returns a 20 byte hash            

            return hashedPassword;
        }
        public static Person? GetPersonByUsername(List<Person> people, string username)
        {
            var Person = people.Where<Person>(x => x.Username == username);
            if (!Person.Any())
            {
                return null;
            }
            else return Person.First();
        }

        public static bool CheckPassword(List<Person> people, string username, string password)
        {
            Person? person = GetPersonByUsername(people, username);
            if (person == null)
            {
                return false;
            }
            if (person.Salt != null && person.Iterations != null)
            {
                var hashedpw = EncryptPassword(password, person.Salt, person.Iterations);
                if (ByteArraysAreEqual(hashedpw, person.Password))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ByteArraysAreEqual(byte[] array1, byte[] array2)
        {
           if(array1.Length != array2.Length)
            {
                return false;
            }
            else
            {
                var equal = true;
                for(int i = 0; i < array1.Length; i++)
                {
                    if (array1[i] != array2[i])
                    {
                        equal = false;
                    }
                }
                return equal;
            }
        }

        public static Person? FindPerson(List<Person> people, int id)
        {
            var person = people.Where(x => x.Id == id);
            if (!person.Any())
            {
                return null;
            }else
            {
                return person.First();
            }
        }
        public static List<Person> FindPerson(List<Person> people, string nameOrId)
        {
            var returnPeople = new List<Person>();
            if (Int32.TryParse(nameOrId,out int id))
            {
               returnPeople.Add(FindPerson(people, id));
                return returnPeople;
            }
            else
            {
                return people.Where(x => x.Name == nameOrId).ToList();
            }
        }
        public static bool PersonExists(List<Person> people, int id)
        {
            if(people.Where(x => x.Id == id).Any())
            {
                return true;
            }
            return false;
        }
    }
}
