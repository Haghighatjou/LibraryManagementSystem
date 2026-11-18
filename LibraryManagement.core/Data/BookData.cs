
using LibraryManagement.Core.Models;
using Microsoft.Data.SqlClient;

namespace LibraryManagement.Core.Data
{
    public class BookData
    {
        private readonly DbConnection dbConnection;
        public BookData(DbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            List<Book> books = new List<Book>();
            using (SqlConnection connection = dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                string query = "SELECT Id, Title, Author, PublishYear, IsAvailable FROM Books";

                using(SqlCommand command=new SqlCommand(query, connection))
                {
                    using(SqlDataReader reader=await command.ExecuteReaderAsync())
                    {
                        while(reader.Read())
                        {
                            Book book = new Book
                            {
                                Id = reader.GetInt32(0),
                                Title=reader.GetString(1),
                                Author=reader.GetString(2),
                                PublishYear=reader.GetInt32(3),
                                IsAvailable=reader.GetBoolean(4)
                            };

                            books.Add(book);
                        }
                    }
                }
            }

            return books; 
        }

        public async Task AddBookAsync(Book book)
        {
            using(SqlConnection connection=dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                string query = @"INSERT INTO Books 
                                (Title, Author, PublishYear, IsAvailable)
                                OUTPUT INSERTED.Id
                                VALUES 
                                (@Title, @Author, @PublishYear, @IsAvailable)";

                using(SqlCommand command=new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@PublishYear", book.PublishYear);
                    command.Parameters.AddWithValue("@IsAvailable", book.IsAvailable);

                    book.Id =(int)await command.ExecuteScalarAsync();
                    //command.ExecuteNonQuery();
                }
            }
        }

        public async Task DeleteBookAsync(int bookId)
        {
            using(SqlConnection connection=dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                string query = "DELETE FROM Books WHERE Id=@Id";

                using(SqlCommand command=new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", bookId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> HasLoanAsync(int bookId)
        {
            using(SqlConnection connection=dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                string query = "SELECT COUNT(*) FROM Loans WHERE BookId= @BookId";

                using(SqlCommand command=new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookId", bookId);

                    int count = (int)await command.ExecuteScalarAsync();

                    return count > 0;
                }
            }
        }

        public async Task UpdateBookAsync(Book book)
        {
            using(SqlConnection connection=dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                string query = @"UPDATE Books
                                SET
                                Title= @Title,
                                Author= @Author,
                                PublishYear=@PublishYear,
                                IsAvailable= @IsAvailable
                                WHERE
                                Id= @Id";

                using(SqlCommand command=new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@PublishYear", book.PublishYear);
                    command.Parameters.AddWithValue("@IsAvailable", book.IsAvailable);
                    command.Parameters.AddWithValue("@Id", book.Id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task BorrowBookAsync(int bookId, int memberId)
        {
            using(SqlConnection connection= dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                using(SqlCommand command=new SqlCommand("dbo.BorrowBook",connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@BookId", bookId);
                    command.Parameters.AddWithValue("@MemberId", memberId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task ReturnBookAsync(int bookId)
        {
            using(SqlConnection connection=dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                using(SqlCommand command=new SqlCommand("dbo.ReturnBook", connection))
                {
                    command.CommandType=System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@BookId", bookId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<LoanReport>> GetActiveLoanReportAsync ()
        {
            List<LoanReport> loanReports = new List<LoanReport>();
            using(SqlConnection connection=dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                string query = "SELECT BookTitle,MemberName,BorrowDate FROM dbo.LoanReport WHERE ReturnDate IS NULL";

                using(SqlCommand command=new SqlCommand(query,connection))
                {
                    using(SqlDataReader reader= await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            LoanReport report = new LoanReport
                            {
                                BookTitle= reader.GetString(0),
                                MemberName= reader.GetString(1),
                                BorrowDate= reader.GetDateTime(2)
                            };

                            loanReports.Add(report);
                        }
                    }
                }
            }

            return loanReports;
        }

    }
}
