using System;
using System.Collections.Generic;

namespace Lab8Handout.Models
{
    public partial class Patron
    {
        public Patron()
        {
            CheckedOuts = new HashSet<CheckedOut>();
            Phones = new HashSet<Phone>();
        }

        public uint CardNum { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<CheckedOut> CheckedOuts { get; set; }
        public virtual ICollection<Phone> Phones { get; set; }
    }
}
