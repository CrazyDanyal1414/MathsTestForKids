using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MathsTest
{
    public class UserLoginSignUp
    {
        [Serializable]
        public class User
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public User(string userName, string password)
            {
                UserName = userName;
                Password = password;
            }
        }
        [Serializable]
        public class UserManager
        {
            public List<User> Accounts;
            public UserManager() => Accounts = new List<User>();

            public void SerializeAccountDetails(string filePath)
            {
                IFormatter bf = new BinaryFormatter();
                using (Stream fs = new FileStream(filePath, FileMode.Create))
                    bf.Serialize(fs, this);
            }

            public static UserManager DeserializeAccountDetails(string filePath)
            {
                IFormatter bf = new BinaryFormatter();
                using (Stream sr = new FileStream(filePath, FileMode.Open))
                    return bf.Deserialize(sr) as UserManager;
            }

            public bool ContainsUserName(string userName) =>
                Accounts.Any(x => x.UserName == userName);

            public bool ContainsAccount(string userName, string password) =>
                Accounts.Any(x => x.UserName == userName && x.Password == password);

            public bool AddAccountDetails(string userName, string password)
            {
                Accounts.Add(new User(userName, password));
                return true;
            }

            public static (string, int) LogInProcess(string filePath)
            {
				int LogInOrSignUp;
				do
				{
					Console.WriteLine("To Login Type 1, To Create a new account Type 2, and to play as a guest Type 3");
					int.TryParse(Console.ReadLine(), out LogInOrSignUp);
				} while (LogInOrSignUp != 1 && LogInOrSignUp != 2);

				var userName = "";
				var password = "";
				var logInSuccessfull = false;
				var userDetails = new UserManager();
				if (File.Exists(filePath))
				{
					userDetails = DeserializeAccountDetails(filePath);
				}

				if (userDetails is null)
				{
					userDetails = new UserManager();
				}

				while (!logInSuccessfull)
				{
					if (LogInOrSignUp == 1)
					{
						Console.WriteLine("Write your username:");
						userName = Console.ReadLine();
						Console.WriteLine("Enter your password:");
						password = Console.ReadLine();
						if (userDetails.ContainsAccount(userName, password))
						{
							Console.WriteLine("You have logged in successfully!");
							logInSuccessfull = true;
							break;
						}
						else
						{
							Console.WriteLine("Your username or password is incorect, try again!");
						}
					}
					else if (LogInOrSignUp == 2)
					{
						Console.WriteLine("Enter a username:");
						userName = Console.ReadLine();

						if (userDetails.ContainsUserName(userName))
						{
							Console.WriteLine("The username is taken. Try another one.");
						}
						else
						{
							Console.WriteLine("Enter a password:");
							password = Console.ReadLine();

							logInSuccessfull = true;
							userDetails.AddAccountDetails(userName, password);
							userDetails.SerializeAccountDetails(filePath);
							Console.WriteLine($"A new account for {userName} has been created!");
						}
					}
                    else
                    {
						Console.WriteLine("How shall we adress you?");
						userName = Console.ReadLine();
                    }
					Console.WriteLine("\n");
				}
				return (userName, LogInOrSignUp);
			}
        }
    }
}
