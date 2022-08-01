using System;
using System.Collections.Generic;

namespace Lab8Handout.Models
{
    public partial class Phone
    {
        public uint CardNum { get; set; }
        public string Phone1 { get; set; } = null!;

        public virtual Patron CardNumNavigation { get; set; } = null!;
    }
}
