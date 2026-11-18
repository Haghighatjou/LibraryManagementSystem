
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Security.Policy;
using System.Diagnostics.Eventing.Reader;
using Library_Management_System.Services;
using Library_Management_System.Exceptions;

namespace Library_Management_System
{
    public class Library
    {
        
        private readonly LibraryService libraryService=new LibraryService();
        
                
        public void AddBook()
        {
            Console.WriteLine("Add book");
            Console.WriteLine(" ");
            string title;
            while(true)
            {

                Console.WriteLine("Title:");
                title = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(title))
                    break;
                Console.WriteLine("Title can not be empty.");

            }

            string author;
            while(true)
            {
                Console.WriteLine("Author:");
                author = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(author))
                    break;
                Console.WriteLine("Author can not be empty.");
            }

            int publishYear;
            while(true)
            {
                Console.WriteLine("Publish Year:");
                if (int.TryParse(Console.ReadLine(), out publishYear))
                {
                    break;
                }
                Console.WriteLine("Please enter a valid year.");                
            }

            Book book=new Book(title,author, publishYear);
            
            libraryService.AddBook(book);
            Console.WriteLine("Book added.");
        }
        public void RemoveBook()
        {
            Console.WriteLine("Remove book.");
            string title;
            while(true)
            {
                Console.WriteLine("Enter a title");
                title = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(title))
                {
                    Console.WriteLine("Title cannot be empty.");
                    continue;
                }
                

                try
                {
                    libraryService.DeleteBook(title);
                    Console.WriteLine("Book removed.");
                    break;
                }

                catch (BookNotFoundException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("Please choose another book.");
                    continue;
                }

                catch (BookNotAvailableException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("Please choose another book.");
                    continue;
                }

                catch (BookHasLoanHistoryException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("Please choose another book.");
                    continue;
                }
            }                                  
        }

        public void EditBook()
        {
            Console.WriteLine("Edit book.");
            string title;
            while(true)
            {
                Console.WriteLine("Enter a title.");
                title = Console.ReadLine();
                if(!string.IsNullOrWhiteSpace(title))
                {
                    break;
                }
                Console.WriteLine("Title cannot be empty.");
            }
            var book = FindBookByTitle(title);
            if (book != null)
            {
                
                
                while(true)
                {
                    Console.WriteLine("Title:");
                    title= Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        book.Title = title;
                        break;
                    }
                    Console.WriteLine("Title cannot be empty.");
                }

                string author;
                while (true)
                {
                    Console.WriteLine("Author:");
                    author = Console.ReadLine();
                    if(!string.IsNullOrWhiteSpace(author))
                    {
                        book.Author = author;
                        break;
                    }
                    Console.WriteLine("Author cannot be empty.");
                }

                int publishYear;
                while (true)
                {
                    Console.WriteLine("Publish Year:");
                    string year = Console.ReadLine();
                    if (int.TryParse(year, out publishYear) && publishYear>0 && publishYear<= DateTime.Now.Year)
                    {
                        break;
                    }
                    Console.WriteLine("Please enter a valid year.");
                }
                
                book.PublishYear= publishYear;
                libraryService.UpdateBook(book);
                Console.WriteLine("Book edited.");
            }
            else
                Console.WriteLine("Book not found.");
        }

        public void ShowBooks()
        {
            var allBooks = libraryService.GetBooks();
            if (allBooks.Any())
            {
                Console.WriteLine("Show Books:");
                allBooks.ForEach(b => b.ShowInfo());
            }
            else
            {
                Console.WriteLine("List is empty");
                return;
            }
        }

        public void SearchBook()
        {
            Console.WriteLine("Search a book:");
            
            string title;
            while(true)
            {
                Console.WriteLine("Enter a title:");
                title = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(title))
                {
                    Book book = FindBookByTitle(title);
                    if (book != null)
                    {
                        book.ShowInfo();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Book not found.");
                        continue;
                    }
                                           
                }
                else
                    Console.WriteLine("Title cannot be empty.");
            }                        
        }

        public void BorrowBook()
        {
            Console.WriteLine("Borrow book:");
            while(true)
            {
                string title;
                Book book;
                while (true)
                {
                    Console.WriteLine("Enter title");
                    title = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        Console.WriteLine("Title cannot be empty.");
                        continue;
                    }
                    book = FindBookByTitle(title);
                    if (book == null)
                    {
                        Console.WriteLine("Book not found.");
                        continue;
                    }
                    try
                    {
                        libraryService.CheckBookAvailability(book.ID);
                        break;
                    }
                    catch(BookNotAvailableException ex)
                    {
                        Console.WriteLine($"Error:{ex.Message}");
                        Console.WriteLine("Please choose another book.");
                    }
                                           
                }

                //Member member;
                bool borrowed = false;
                while (true)
                {
                    Console.WriteLine("Enter member name:");
                    string memberName = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(memberName))
                    {                        
                        Console.WriteLine("Member name cannot be empty.");
                        continue;
                    }

                    //member = FindMemberByName(memberName);

                    try
                    {
                        libraryService.BorrowBook(book.ID, memberName);
                        Console.WriteLine("Book borrowed seccussfully.");
                        borrowed = true;
                        break;
                    }
                    catch (MemberNotFoundException ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        Console.WriteLine("Please enter another member name.");
                        continue;
                    }
                    catch (BookNotAvailableException ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        Console.WriteLine("Please choose another book.");
                        continue;
                    }
                    
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        Console.WriteLine("Please choose another book.");
                        continue;
                    }
                    
                }
                                               
                if(borrowed)
                {
                    break;
                }
            }
        }

        public void ReturnBook()
        {
            Console.WriteLine("Return book");
            string title;
            
            while (true)
            {
                Console.WriteLine("Enter book title");
                title = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(title))
                {
                    Console.WriteLine("Book title cannot be empty.");
                    continue;
                }
                
                
                try
                {
                    libraryService.ReturnBook(title);
                    Console.WriteLine("Book returned successfully.");
                    break;
                }
                catch (BookNotFoundException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("Please choose another book.");
                    continue;
                }
                catch (BookNotBorrowedException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("Please choose another book.");
                    continue;
                }
                
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("Please choose another book.");
                    continue;
                }
            }
        }

        private Book FindBookByTitle(string title)
        {
            return libraryService.FindBookByTitle(title);
        }

        private Member FindMemberByName(string name)
        {
            return libraryService.FindMemberByName(name);
        }
        public void AddMember()
        {
            Console.WriteLine("Add member");
            string name;
            while(true)
            {
                Console.WriteLine("Enter name:");
                name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(name))
                    break;
                Console.WriteLine("Name cannot be empty.");
            }

            string phone;
            while(true)
            {
                Console.WriteLine("Enter phone number:");
                phone = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(phone) && phone.All(char.IsDigit))
                    break;
                Console.WriteLine("Please enter a valid phone number.");
            }
            Member member = new Member(name, phone);
            libraryService.AddMember(member);                      
            Console.WriteLine("Member added.");
        }

        public void ShowMembers()
        {
            var allMembers = libraryService.GetAllMembers();
            if(allMembers.Any())
            {
                Console.WriteLine("Total members:");
                allMembers.ForEach(m => m.ShowInfo());
            }
            else
            {
                Console.WriteLine("There is no member.");
                
            }
        }

        public void EditMember()
        {
            Console.WriteLine("Edit Member:");
            string name;
            while(true)
            {
                Console.WriteLine("Enter member name:");
                name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(name))
                    break;
                Console.WriteLine("Name cannot be empty.");
            }
            Member member = FindMemberByName(name);
            if(member != null)
            {
                member.ShowInfo();
                int id;
                while (true)
                {
                    Console.WriteLine("Enter Id for edit member:");

                    if (int.TryParse(Console.ReadLine(), out id))
                    {
                        if (id == member.ID)
                            break;

                        Console.WriteLine("The ID does not match the member.");
                    }
                    else
                        Console.WriteLine("Please enter a valid ID.");

                }

                while (true)
                {
                    Console.WriteLine("Enter member name:");
                    member.Name = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(member.Name))
                        break;
                    Console.WriteLine("Name cannot be empty.");
                }


                string phone;
                while (true)
                {
                    Console.WriteLine("Enter phone number:");
                    phone = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(phone) && phone.All(char.IsDigit))
                        break;
                    Console.WriteLine("Please enter a valid phone number.");
                }
                libraryService.UpdateMember(member);
                Console.WriteLine("Updated seccessfully.");
            }
            else
            {
                Console.WriteLine("Member not found.");
            }                       
        }

        public void DisableMember()
        {
            Console.WriteLine("Disable member...");
            Console.WriteLine("Enter member name:");
            string name = Console.ReadLine();
            Member member = FindMemberByName(name);
            if (member != null)
            {
                member.ShowInfo();
                int id;
                while (true)
                {
                    Console.WriteLine("Enter member id:");
                    if (int.TryParse(Console.ReadLine(), out id))
                    {
                        if (id == member.ID)
                            break;
                        Console.WriteLine("The ID does not match the member.");
                    }
                    else
                        Console.WriteLine("Enter a valid ID.");

                }
                libraryService.UpdateMemberStatus(member.ID, false);
                Console.WriteLine("Member disabled.");
            }


            else
                Console.WriteLine("Member not found.");
            
        }

        public void EnableMember()
        {
            Console.WriteLine("Enable Member...");
            Console.WriteLine("Enter member name:");
            string name = Console.ReadLine();
            Member member = FindMemberByName(name);
            if (member != null)
            {
                member.ShowInfo();
                int id;
                while(true)
                {
                    Console.WriteLine("Enter member id:");
                    if (int.TryParse(Console.ReadLine(), out id))
                    {
                        if (id == member.ID)
                            break;
                        Console.WriteLine("The ID does not match the member.");
                    }
                    else
                        Console.WriteLine("Enter a valid ID.");

                }
                libraryService.UpdateMemberStatus(member.ID, true);
                Console.WriteLine("Member enabled.");

            }
            else
                Console.WriteLine("Member not found.");
        }

        public void GetStatistics()
        {
            var report = libraryService.GetStatistics();

            Console.WriteLine("==== Library Report =====");
            Console.WriteLine($"Total books: {report.TotalBooks}");
            Console.WriteLine($"Available books: {report.AvailableBooks}");
            Console.WriteLine($"Borrowed books: {report.BorrowedBooks}");
            Console.WriteLine($"Total members: {report.TotalMembers}");
            Console.WriteLine($"Active members: {report.ActiveMembers}");
            Console.WriteLine($"Disable members: {report.DisabledMembers}");
        }
    }
}


