using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library_Management_System.Data;
using Microsoft.Data.SqlClient;

namespace Library_Management_System
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Library library = new Library();
            
            while (true)
            {
                ShowMenu();
                
                if(!int.TryParse(Console.ReadLine(),out int input))
                {
                    Console.WriteLine("Please enter a valid number.");
                    continue;
                }

                if(!Enum.IsDefined(typeof(Menu),input))
                {
                    Console.WriteLine("Invalid menu option.");
                }

                Menu choice = (Menu)input;

                try
                {
                    switch (choice)
                    {
                        case Menu.AddBook:
                            {
                                library.AddBook();
                                break;
                            }
                        case Menu.DeleteBook:
                            {
                                library.RemoveBook();
                                break;
                            }
                        case Menu.EditBook:
                            {
                                library.EditBook();
                                break;
                            }
                        case Menu.ShowBooks:
                            {
                                library.ShowBooks();
                                break;
                            }
                        case Menu.BorrowBook:
                            {
                                library.BorrowBook();
                                break;
                            }
                        case Menu.ReturnBook:
                            {
                                library.ReturnBook();
                                break;
                            }
                        case Menu.SearchBook:
                            {
                                library.SearchBook();
                                break;
                            }
                        case Menu.AddMember:
                            {
                                library.AddMember();
                                break;
                            }
                        case Menu.ShowMembers:
                            {
                                library.ShowMembers();
                                break;
                            }
                        case Menu.EditMember:
                            {
                                library.EditMember();
                                break;
                            }
                        case Menu.EnableMember:
                            {
                                library.EnableMember();
                                break;
                            }
                        case Menu.DisableMember:
                            {
                                library.DisableMember();
                                break;
                            }
                        case Menu.Reports:
                            {
                                library.GetStatistics();
                                break;
                            }
                        case Menu.Exit:
                            {
                                return;
                            }
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine(" ");
            }                             
                       
            void ShowMenu()
            {
                Console.WriteLine("====Library====");
                Console.WriteLine(" ");
                foreach(Menu menu in Enum.GetValues(typeof(Menu)))
                {
                    Console.WriteLine($"{(int)menu}-{menu}");
                }
            }                        
    
        }
    }       
}
