using System;
using System.Collections.Generic;

namespace Lab8Handout.Models
{
    public partial class Inventory
    {
        public uint Serial { get; set; }
        public string Isbn { get; set; } = null!;

        public virtual Title IsbnNavigation { get; set; } = null!;
        public virtual CheckedOut CheckedOut { get; set; } = null!;
    }
}
