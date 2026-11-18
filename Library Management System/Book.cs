using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System
{
    public class Book : LibraryItem, IBorrowable,IEntity
    {

        public string Author { get; set; }
        public int PublishYear { get; set; }
        public BookStatus Status { get; private set; } = BookStatus.Available;

        public Book(string title, string author, int publishYear) : base(title)
        {
            Author = author;
            PublishYear = publishYear;
        }

        public Book(int id, string title, string author, int publishYear) : base(title)
        {
            ID= id;
            Author = author;
            PublishYear = publishYear;
        }
        public void Borrow(List<BorrowRecord> br)
        {
            if (Status == BookStatus.Borrowed)
            {
                Console.WriteLine("Book is borrowed.");
            }
            else
            {
                Console.WriteLine("Enter member Id");
                int MemberID = int.Parse(Console.ReadLine());
                BorrowRecord record = new BorrowRecord(ID, MemberID);
                br.Add(record);
                Status = BookStatus.Borrowed;
                Console.WriteLine("Book borrowed seccussfully.");
            }
        }

        public void Return(List<BorrowRecord> borrowRecords)
        {

            Console.WriteLine("Enter Member ID");
            int memberID = int.Parse(Console.ReadLine());
            BorrowRecord br = borrowRecords.Find(b => b.MemberId == memberID);
            if (br != null)
            {
                Status = BookStatus.Available;
                br.ReturnDate = DateTime.Now;
                Console.WriteLine("Book returned seccussfully.");
            }
            else
            {
                Console.WriteLine("Borrow record not found.");
            }
        }

        public void ShowInfo()
        {
            string Isborrow;
            if (Status == BookStatus.Borrowed)
                Isborrow = "yes";
            else
                Isborrow = "no";


            Console.WriteLine($"Id: {ID}, Title: {Title}, Author: {Author}, Publish Year: {PublishYear}, Borrowed:{Isborrow}");
        }

        public void SetStatus(bool isAvailable)
        {
            Status=isAvailable
                ?BookStatus.Available
                :BookStatus.Borrowed;
        }
    }

}
