using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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
                using Stream fs = new FileStream(filePath, FileMode.Create);
                bf.Serialize(fs, this);
            }

            public static UserManager DeserializeAccountDetails(string filePath)
            {
                IFormatter bf = new BinaryFormatter();
                using Stream sr = new FileStream(filePath, FileMode.Open);
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
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("To Login Type 1, To Create a new account Type 2, and to play as a guest Type 3");
                    Console.ResetColor();
					int.TryParse(Console.ReadLine(), out LogInOrSignUp);
				} while (LogInOrSignUp != 1 && LogInOrSignUp != 2 && LogInOrSignUp != 3);

				var userName = "";
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
                    string password = "";
                    if (LogInOrSignUp == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Write your username:");
                        Console.ResetColor();
                        userName = Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("Enter your password:");
                        Console.ResetColor();
                        do
                        {
                            ConsoleKeyInfo key = Console.ReadKey(true);
                            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                            {
                                password += key.KeyChar;
                                Console.Write("*");
                            }
                            else
                            {
                                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                                {
                                    password = password.Substring(0, password.Length - 1);
                                    Console.Write("\b \b");
                                }
                                else if (key.Key == ConsoleKey.Enter)
                                {
                                    Console.WriteLine("\n");
                                    break;
                                }
                            }
                        } while (true);
                        if (userDetails.ContainsAccount(userName, password))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("You have logged in successfully!");
                            Console.ResetColor();
                            logInSuccessfull = true;
                            break;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Your username or password is incorect, try again!");
                            Console.ResetColor();
                        }
                    }
                    else if (LogInOrSignUp == 2)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Enter a username:");
                        Console.ResetColor();
                        userName = Console.ReadLine();

                        if (userDetails.ContainsUserName(userName))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("The username is taken. Try another one.");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("Enter a password:");
                            Console.ResetColor();
                            do
                            {
                                ConsoleKeyInfo key = Console.ReadKey(true);
                                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                                {
                                    password += key.KeyChar;
                                    Console.Write("*");
                                }
                                else
                                {
                                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                                    {
                                        password = password.Substring(0, password.Length - 1);
                                        Console.Write("\b \b");
                                    }
                                    else if (key.Key == ConsoleKey.Enter)
                                    {
                                        Console.WriteLine("\n");
                                        break;
                                    }
                                }
                            } while (true);

                            logInSuccessfull = true;
                            userDetails.AddAccountDetails(userName, password);
                            userDetails.SerializeAccountDetails(filePath);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"A new account for {userName} has been created!");
                            Console.WriteLine("\n");
                            Console.ResetColor();
                        }
                    }
                    else if (LogInOrSignUp == 3)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("How shall we adress you?");
                        Console.ResetColor();
                        userName = Console.ReadLine();
                        Console.WriteLine("\n");
                        logInSuccessfull = true;
                    }
				}
				return (userName, LogInOrSignUp);
			}
        }
    }
}
