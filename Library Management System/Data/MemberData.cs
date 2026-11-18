using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;


namespace Library_Management_System.Data
{
    internal class MemberData
    {
        private DbConnection db = new DbConnection();

        internal List<Member> GetAllMembers()
        {
            List<Member> members = new List<Member>();

            using(SqlConnection connection=db.GetConnection())
            {
                connection.Open();

                string query = "SELECT Id, MemberName, PhoneNumber, IsActive FROM Members";

                using(SqlCommand command=new SqlCommand(query, connection))
                {
                    using(SqlDataReader reader=command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            Member member=new Member(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetBoolean(3));

                            members.Add(member);
                        }
                    }
                }
            }

            return members;
        }

        internal void AddMember(Member member)
        {
            using(SqlConnection connection=db.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Members (MemberName, PhoneNumber) VALUES (@MemberName, @PhoneNumber)";

                using(SqlCommand command=new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@MemberName", member.Name);
                    command.Parameters.AddWithValue("@PhoneNumber", member.Phone);
                    
                    command.ExecuteNonQuery();
                }
            }
        }

        internal void UpdateMember(Member member)
        {
            using(SqlConnection connection=db.GetConnection())
            {
                connection.Open();

                string query = "UPDATE Members SET MemberName=@MemberName, PhoneNumber=@PhoneNumber WHERE Id=@Id";

                using(SqlCommand command=new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@MemberName", member.Name);
                    command.Parameters.AddWithValue("@PhoneNumber", member.Phone);
                    command.Parameters.AddWithValue("@Id", member.ID);

                    command.ExecuteNonQuery();
                }
            }
        }

        internal void UpdateMemberStatus(int id, bool isActive)
        {
            using(SqlConnection connection=db.GetConnection())
            {
                connection.Open();
                string query = "UPDATE Members SET IsActive= @IsActive WHERE Id=@Id";

                using(SqlCommand command=new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IsActive", isActive);
                    command.Parameters.AddWithValue("@Id", id);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
