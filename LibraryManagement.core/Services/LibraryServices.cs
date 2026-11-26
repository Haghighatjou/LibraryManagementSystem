using LibraryManagement.Core.Data;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



namespace LibraryManagement.Core.Services
{
    public class LibraryService
    {
        
        private readonly LibraryDbContext context;
        private readonly ILogger<LibraryService> logger;

        public LibraryService(LibraryDbContext context, ILogger<LibraryService> logger)
        {            
            this.context = context;
            this.logger = logger;
        }

        public async Task<PagedResult<Book>> GetBooksAsync(int pageNumber, int pageSize)
        {            
            if(pageNumber<=0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pageNumber), "Page number must be greater than zero");
            }

            if(pageSize<=0 || pageSize>100)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize),
                    "Page size must be between 1 and 100.");
            }

            var totalCount=await context.Books.CountAsync();
            var totalPages=(int)Math.Ceiling((double)totalCount/pageSize);

            return new PagedResult<Book>
            {
                Items = await context.Books
                    .OrderBy(b => b.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages= totalPages
            };
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await context.Books.FindAsync(id);
        }

        public async Task AddBookAsync(Book book)
        {
            logger.LogInformation(
                "Add Book request received. Title: {Title}, Author: {Author}",
                book.Title,book.Author);

            book.IsAvailable = true;

            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();
        }

        public async Task UpdateBookAsync(Book book)
        {
            var existingBook = await context.Books.FindAsync(book.Id);

            if(existingBook==null)
            {
                throw new BookNotFoundException($"Book with Id {book.Id} was not found.");
            }
            existingBook.Title=book.Title;
            existingBook.Author=book.Author;
            existingBook.PublishYear=book.PublishYear;            

            await context.SaveChangesAsync();

        }

        public async Task BorrowBookAsync(int bookId, int memberId)
        {
            logger.LogInformation("Borrow request received for BookId: {BookId}", bookId);
            logger.LogInformation("Borrow request received for MemberId: {MemberId}", memberId);

            await using var transaction =
                await context.Database.BeginTransactionAsync();

            var member = await context.Members.FindAsync(memberId);

            if(member==null)
            {
                throw new MemberNotFoundException($"Member with Id {memberId} was not found.");
            }

            if(!member.IsActive)
            {
                throw new MemberInactiveException("Inactive members cannot borrow book.");
            }

            var affectedRows = await context.Books.Where(b => b.Id == bookId && b.IsAvailable)
                .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.IsAvailable, false));

            if(affectedRows==0)
            {
                var bookExists = await context.Books.AnyAsync(b => b.Id == bookId);

                if(!bookExists)
                {
                    logger.LogWarning("Book not found. BookId:{BookId}", bookId);

                    throw new BookNotFoundException($"Book with Id {bookId} was not found.");
                }
                throw new BookNotAvailableException("Book is not available.");
            }

            
            var loan = new Loan
            {
                BookId = bookId,
                MemberId= memberId,
                BorrowDate=DateTime.Now,
                ReturnDate=null
            };

            await context.Loans.AddAsync(loan);
            
            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        public async Task ReturnBookAsync(int bookId)
        {
            logger.LogInformation("Return request received for BookId: {BookId}", bookId);

            await using var transaction
                =await context.Database.BeginTransactionAsync();

            var book=await context.Books.Where(b => b.Id==bookId).FirstOrDefaultAsync();

            if (book == null)
            {
                throw new BookNotFoundException("Book not found.");
            }

            if (book.IsAvailable)
            {
                throw new BookNotAvailableException("Book is not borrowed.");
            }

            book.IsAvailable = true;

            var loan = await context.Loans.Where(l => l.BookId == bookId && l.ReturnDate==null).FirstOrDefaultAsync();

            if(loan == null)
            {
                throw new LoanNotFoundException("Loan not found.");
            }

            loan.ReturnDate= DateTime.Now;

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task DeleteBookAsync(int bookId)
        {
            logger.LogInformation("Delete Book received for BookId: {BookId}", bookId);

            var book = await GetBookByIdAsync(bookId);

            if (book == null)
            {
                throw new BookNotFoundException("Book not found.");
            }

            if (!book.IsAvailable)
            {
                throw new BookNotAvailableException("Book is borrowed and cannot be deleted.");
            }

            if (await context.Loans.AnyAsync(l => l.BookId==bookId))
            {
                throw new BookHasLoanHistoryException("Book has loan history and cannot be deleted.");
            }

            context.Books.Remove(book);
            await context.SaveChangesAsync();
        }

        public async Task<Book?> FindBookByTitleAsync(string title)
        {
            return await context.Books.FirstOrDefaultAsync(b => b.Title == title);
        }

        public async Task<List<LoanReport>> GetActiveLoanReportAsync ()
        {
            return await context.LoanReport
                .Where(loan => loan.ReturnDate == null)
                .Select(loan => new LoanReport
                {
                    BookTitle = loan.BookTitle,
                    MemberName = loan.MemberName,
                    BorrowDate = loan.BorrowDate
                }).ToListAsync();
        }
        

        public async Task<List<Member>> GetMembersAsync()
        {            
            return await context.Members.ToListAsync();
        }

        public async Task UpdateMemberAsync(Member member)
        {
            logger.LogInformation("Update Member request received for MemberId: {MemberId}", member.Id);

            if (member.MemberName == null && member.PhoneNumber == null)
            {
                throw new ArgumentException("At least one field must be provided.");
            }

            var existingMember =await FindMemberByIdAsync(member.Id);
                       
            if (member.MemberName != null)
            {
                existingMember.MemberName = member.MemberName;
            }
            
            if(member.PhoneNumber != null)
            {
                existingMember.PhoneNumber = member.PhoneNumber;
            }
            
            await context.SaveChangesAsync();
        }

        public async Task EnableMemberAsync(int memberId)
        {
            logger.LogInformation("Enable Member request received for MemberId: {MemberId}", memberId);

            var existingMember=await FindMemberByIdAsync(memberId);


            if(existingMember.IsActive)
            {
                throw new MemberAlreadyActiveException("Member is already active.");
            }

            existingMember.IsActive = true;

            await context.SaveChangesAsync();
        }

        public async Task DisableMemberAsync(int memberId)
        {
            logger.LogInformation("Disable Member request received for MemberId: {MemberId}", memberId);

            var existingMember = await FindMemberByIdAsync(memberId);


            if (!existingMember.IsActive)
            {
                throw new MemberInactiveException("Member is already inactive.");
            }

            existingMember.IsActive = false;

            await context.SaveChangesAsync();
        }

        public async Task<Member> FindMemberByIdAsync(int id)
        {
            var member = await context.Members.FirstOrDefaultAsync(m => m.Id==id);

            if(member==null)
            {
                throw new MemberNotFoundException("Member not found.");
            }

            return member;
        }

        public async Task AddMemberAsync(string memberName,string phoneNumber)
        {
            logger.LogInformation("Add Member request received");

            var member = new Member
            {
                MemberName = memberName,
                PhoneNumber = phoneNumber,
                IsActive=true
            };

            context.Members.Add(member);
            await context.SaveChangesAsync();
        }
    }
}