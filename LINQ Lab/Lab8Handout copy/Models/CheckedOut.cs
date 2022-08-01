using System;
using System.Collections.Generic;

namespace Lab8Handout.Models
{
    public partial class CheckedOut
    {
        public uint CardNum { get; set; }
        public uint Serial { get; set; }

        public virtual Patron CardNumNavigation { get; set; } = null!;
        public virtual Inventory SerialNavigation { get; set; } = null!;
    }
}
