using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Services;
using LibraryManagement.Core.Data;
using LibraryManagementAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;


namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        public readonly LibraryService LibraryService;
        public readonly LibraryDbContext context;
        public BooksController(LibraryService libraryService, LibraryDbContext context)
        {
            this.LibraryService = libraryService;
            this.context= context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetBooksAsync()
        {
            
            var books = await LibraryService.GetBooksAsync();

            var bookDtos = books.Select(book => new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublishYear = book.PublishYear
            }).ToList();

            return Ok(bookDtos);
        }

        [Authorize]
        [HttpGet("{id}", Name ="GetBookById")]
        public async Task<IActionResult> GetBookByIdAsync(int id)
        {
            var book = await LibraryService.GetBookByIdAsync(id);

            if (book==null)
            {
                return NotFound();
            }
            var bookDto = new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublishYear = book.PublishYear
            };

            return Ok(bookDto);
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchBookAsync([FromQuery] string title)
        {
            var book = await LibraryService.FindBookByTitleAsync(title);

            if (book == null)
            {
                return NotFound();
            }

            var bookDto = new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublishYear = book.PublishYear
            };

            return Ok(bookDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddBook(CreateBookDTO bookDto)
        {
            Book book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                PublishYear = bookDto.PublishYear,
                IsAvailable = true
            };
            await LibraryService.AddBookAsync(book);

            return CreatedAtRoute("GetBookById", new { id = book.Id }, book);
            //return Ok(book);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookAsync(int id, UpdateBookDTO bookDto)
        {
            var book = new Book
            {
                Id = id,
                Title = bookDto.Title,
                Author = bookDto.Author,
                PublishYear = bookDto.PublishYear,
                IsAvailable = bookDto.IsAvailable
            };

            
            await LibraryService.UpdateBookAsync(book);

            return Ok(book);
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            await LibraryService.DeleteBookAsync(id);
            
            return Ok("Book deleted succussfully.");
        }

        [Authorize]
        [HttpPost("{bookId}/borrow")]
        public async Task<IActionResult> BorrowBookAsync(int bookId,int memberId)
        {
            try
            {
                await LibraryService.BorrowBookAsync(bookId, memberId);

                return Ok("Book borrowed seccussfully.");
            }

            catch(MemberInactiveException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBookAsync(int id)
        {
            await LibraryService.ReturnBookAsync(id);

            return Ok("Book returned successfully.");
        }

        [Authorize]
        [HttpGet ("active-loans")]
        public async Task<IActionResult> GetActiveLoanReportAsync()
        {
            var reports= await LibraryService.GetActiveLoanReportAsync();
            var reportsDto = reports.Select(report => new LoanReportDTO
            {
                bookTitle=report.BookTitle,
                memberName=report.MemberName,
                borrowDate=report.BorrowDate
            }).ToList();

            return Ok(reportsDto);
        }
    }
}
