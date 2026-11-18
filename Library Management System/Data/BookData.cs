using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Library_Management_System.Data
{
    internal class BookData
    {
        private readonly DbConnection db = new DbConnection();

        public List<Book> GetBooks()
        {
            
            List<Book> books = new List<Book>();
            using(SqlConnection connection=db.GetConnection())
            {
                connection.Open();
                
                string query = "SELECT Id, Title, Author, PublishYear, IsAvailable FROM Books";

                using(SqlCommand command=new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string title=reader.GetString(1);
                            string author=reader.GetString(2);
                            int publishYear=reader.GetInt32(3);
                            bool isAvailable=reader.GetBoolean(4);

                            Book book = new Book(id, title, author, publishYear);
                            book.SetStatus(isAvailable);

                            books.Add(book);
                            
                        }
                    }
                }
            }
            return books;
        }

        internal void AddBook(Book book)
        {
            using(SqlConnection connection=db.GetConnection())
            {
                connection.Open();
                string query = @"INSERT INTO Books
                                (Title,Author,PublishYear,IsAvailable)
                                VALUES
                                (@Title, @Author, @PublishYear, @IsAvailable)";
                using(SqlCommand command=new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@Title",book.Title);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@PublishYear", book.PublishYear);

                    bool isAvailable=book.Status== BookStatus.Available;
                    command.Parameters.AddWithValue("@IsAvailable", isAvailable);

                    command.ExecuteNonQuery();
                }
            }
        }

        internal void DeleteBook(int id)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Books WHERE Id = @Id";

                using(SqlCommand command = new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    command.ExecuteNonQuery();
                }
            }
        }

        internal bool HasLoan (int bookId)
        {
            using(SqlConnection connection=db.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Loans WHERE BookId= @BookId";
                using(SqlCommand command= new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@BookId", bookId);
                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        internal void UpdateBook(Book book)
        {
            using(SqlConnection connection=db.GetConnection())
            {
                connection.Open();

                string query = @"UPDATE Books
                                SET 
                                Title=@Title,
                                Author=@Author,
                                PublishYear=@PublishYear
                                WHERE
                                Id=@Id";

                using(SqlCommand command=new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@PublishYear", book.PublishYear);
                    command.Parameters.AddWithValue("@Id", book.ID);

                    command.ExecuteNonQuery();
                }
            }

        }

        internal void BorrowBook(int bookId, int memberId)
        {
            using(SqlConnection connection=db.GetConnection())
            {
                connection.Open();

                using(SqlCommand command=new SqlCommand("dbo.BorrowBook", connection))
                {
                    //نوع دستور داخل کوری معمولی نیست
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@BookId", bookId);
                    command.Parameters.AddWithValue("@MemberId", memberId);

                    command.ExecuteNonQuery();
                }
            }
        }

        internal void ReturnBook(int bookId)
        {
            using(SqlConnection connection=db.GetConnection())
            {
                connection.Open();
                using(SqlCommand command=new SqlCommand("dbo.ReturnBook",connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@BookId", bookId);
                    

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
