using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System
{
    public abstract class LibraryItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        

        public LibraryItem(string title)
        {
            
            Title = title;
        }
        public virtual void ShowInfo()
        {
            Console.WriteLine($"Title: {Title}");
        }
    }
}
