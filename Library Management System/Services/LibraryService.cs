using Library_Management_System.Data;
using System.Collections.Generic;
using System.Linq;
using System;
using Library_Management_System.Exceptions;

namespace Library_Management_System.Services
{
    public class LibraryService
    {
        private readonly BookData bookData;
        private readonly MemberData memberData;

        public LibraryService()
        {
            bookData = new BookData();
            memberData = new MemberData();
        }

        public List<Book> GetBooks()
        {
            return bookData.GetBooks();
        }

        public void AddBook(Book book)
        {
            bookData.AddBook(book);
        }

        public void UpdateBook(Book book)
        {
            bookData.UpdateBook(book);
        }

        public void BorrowBook(int bookId, int memberId)
        {

            var books = GetBooks();
            var book = books.FirstOrDefault(b => b.ID == bookId);

            if (book == null)
            {
                throw new Exception("Book not found.");
            }

            if (book.Status == BookStatus.Borrowed)
            {
                throw new BookNotAvailableException("Book is not available.");
            }
            var members = GetAllMembers();
            var member = members.FirstOrDefault(m => m.ID == memberId);

            if(member==null)
            {
                throw new MemberNotFoundException("Member not found.");
            }
            if(!member.IsActive)
            {
                throw new MemberDisabledException("Member is disabled.");
            }
                       
            bookData.BorrowBook(bookId, memberId);
        }

        public void BorrowBook(int bookId, string memberName)
        {
            var member = FindMemberByName(memberName);

            if(member == null)
            {
                throw new MemberNotFoundException("Member not found.");
            }

            BorrowBook(bookId, member.ID);
        }
        public void ReturnBook(int bookId)
        {
            var books = GetBooks();
            var book=books.FirstOrDefault(b => b.ID==bookId);

            if(book==null)
            {
                throw new BookNotFoundException("Book not found.");
            }
            if(book.Status==BookStatus.Available)
            {
                throw new BookNotBorrowedException("Book is not borrowed.");
            }

            bookData.ReturnBook(bookId);
        }

        public void ReturnBook(string bookTitle)
        {
            var book = FindBookByTitle(bookTitle);

            if(book==null)
            {
                throw new BookNotFoundException("Book not found.");
            }

            ReturnBook(book.ID);
        }

        public void DeleteBook(int bookId)
        {
            var books = GetBooks();
            var book=books.FirstOrDefault(b => b.ID== bookId);
            if(book==null)
            {
                throw new BookNotFoundException("Book not found.");
            }

            if(book.Status==BookStatus.Borrowed)
            {
                throw new BookNotAvailableException("Book is borrowed and cannot be deleted.");
            }

            if(bookData.HasLoan(bookId))
            {
                throw new BookHasLoanHistoryException("Book has loan history and cannot be deleted.");
            }

            bookData.DeleteBook(bookId);
        }

        public void DeleteBook(string title)
        {
            var book = FindBookByTitle(title);

            if(book==null)
            {
                throw new BookNotFoundException("Book not found.");
            }

            DeleteBook(book.ID);
        }

        public Book FindBookByTitle(string title)
        {
            return bookData.GetBooks().FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }

        public List<Member> GetAllMembers()
        {
            return memberData.GetAllMembers();
        }

        public void AddMember(Member member)
        {
            memberData.AddMember(member);
        }

        public void UpdateMember(Member member)
        {
            memberData.UpdateMember(member);
        }

        public Member FindMemberByName(string name)
        {
            return memberData.GetAllMembers().FirstOrDefault(n => n.Name.Equals(name, StringComparison.OrdinalIgnoreCase)); ;
        }

        public void CheckBookAvailability(int bookId)
        {
            var books = GetBooks();
            var book = books.FirstOrDefault(b => b.ID == bookId);

            if(book==null)
            {
                throw new Exception("Book not found.");
            }

            if(book.Status==BookStatus.Borrowed)
            {
                throw new BookNotAvailableException("Book is not available.");
            }
        }
        public bool IsMemberActive(string name)
        {
            var member = FindMemberByName(name);
            if(member == null)
            {
                return false;
            }
            return member.IsActive;
        }

        public void UpdateMemberStatus(int memberId,bool status)
        {
            memberData.UpdateMemberStatus(memberId, status);
        }

        public LibraryReport GetStatistics()
        {
            var allBooks = GetBooks();
            var allMembers = GetAllMembers();

            return new LibraryReport
            {
                TotalBooks= allBooks.Count,
                AvailableBooks=allBooks.Count(b => b.Status==BookStatus.Available),
                BorrowedBooks=allBooks.Count(b => b.Status==BookStatus.Borrowed),
                TotalMembers=allMembers.Count,
                ActiveMembers=allMembers.Count(m => m.IsActive),
                DisabledMembers=allMembers.Count(m => !m.IsActive)
            };
        }
    }
}