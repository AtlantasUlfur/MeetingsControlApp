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
        public static List<Person> getAllPeople(string path)
        {
            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<Person>>(json);
            }
            catch (Exception e)
            {
                return new List<Person>();
            }
        }

        public static void SaveAllPeople(List<Person> list, string path)
        {
            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static IDictionary<string, byte[]> encryptPassword(string password)
        {
            Rfc2898DeriveBytes PBKDF2 = new Rfc2898DeriveBytes(password, 8, 20);    //Hash the password with a 8 byte salt
            byte[] hashedPassword = PBKDF2.GetBytes(20);    //Returns a 20 byte hash
            byte[] salt = PBKDF2.Salt;

            int intValue = 20;
            byte[] intBytes = BitConverter.GetBytes(intValue);
            if (BitConverter.IsLittleEndian) { 
                Array.Reverse(intBytes);
             }
            byte[] result = intBytes;

            IDictionary<string, byte[]> returnDict = new Dictionary<string, byte[]>();
            returnDict["password"] = hashedPassword;
            returnDict["salt"] = salt;
            returnDict["iterations"] = result;

            return returnDict;
        }
        public static byte[] encryptPassword(string password, byte[] salt, byte[] iteration)
        {

            //int numberOfIterations = BitConverter.ToInt32(iteration, 0);

            Rfc2898DeriveBytes PBKDF2 = new Rfc2898DeriveBytes(password, salt, 20);    //Hash the password with the users salt
            byte[] hashedPassword = PBKDF2.GetBytes(20);    //Returns a 20 byte hash            

            return hashedPassword;
        }
        public static Person? getPersonByUsername(List<Person> people, string username)
        {
            var Person = people.Where<Person>(x => x.Username == username);
            if (Person.Count() == 0)
            {
                return null;
            }
            else return Person.First();
        }

        public static bool checkPassword(List<Person> people, string username, string password)
        {
            Person? person = getPersonByUsername(people, username);
            if (person == null)
            {
                return false;
            }
            if (person.Salt != null && person.Iterations != null)
            {
                var hashedpw = encryptPassword(password, person.Salt, person.Iterations);
                if (byteArraysAreEqual(hashedpw, person.Password))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool byteArraysAreEqual(byte[] array1, byte[] array2)
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

        public static Person? findPerson(List<Person> people, int id)
        {
            var person = people.Where(x => x.Id == id);
            if (person.Count() == 0)
            {
                return null;
            }else
            {
                return person.First();
            }
        }
        public static List<Person> findPerson(List<Person> people, string nameOrId)
        {
            bool isInt;
            int id;
            var returnPeople = new List<Person>();
            if(Int32.TryParse(nameOrId,out id))
            {
               returnPeople.Add(findPerson(people, id));
                return returnPeople;
            }
            else
            {
                return people.Where(x => x.Name == nameOrId).ToList();
            }
        }
        public static bool personExists(List<Person> people, int id)
        {
            if(people.Where(x=> x.Id == id).Count() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
