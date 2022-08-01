using System;
using System.Collections.Generic;

namespace Lab8Handout.Models
{
    public partial class Title
    {
        public Title()
        {
            Inventories = new HashSet<Inventory>();
        }

        public string Isbn { get; set; } = null!;
        public string Title1 { get; set; } = null!;
        public string Author { get; set; } = null!;

        public virtual ICollection<Inventory> Inventories { get; set; }
    }
}
