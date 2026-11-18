using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System
{
    public class Repository<T> where T : IEntity
    {
        private readonly List<T> items = new List<T>();

        public void Add(T item)
        {
            items.Add(item);
        }

        public T FindById(int id)
        {
            return items.FirstOrDefault(x => x.ID == id);
        }
        public void Remove(int id)
        {
            var item = FindById(id);
            if (item != null)
            {
                items.Remove(item);

            }
        }
        public List<T> GetAll()
        {
            return items;
        }
    }

}
