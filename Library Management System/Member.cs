using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System
{
    public class Member:IEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }

        public Member(int id, string name, string phone)
        {
            ID = id;
            Name = name;
            Phone = phone;
        }

        public Member(string name, string phone)
        {
            Name = name;
            Phone=phone;
        }

        public Member(int id, string name, string phone, bool isActive)
        {
            ID = id;
            Name = name;
            Phone = phone;
            IsActive = isActive;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"{ID} - {Name} - {Phone} - Is Active: {IsActive}");
        }
    }
}
