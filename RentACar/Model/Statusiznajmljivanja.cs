using System;
using System.Collections.Generic;

namespace RentACar.Model
{
    public partial class Statusiznajmljivanja
    {
        public Statusiznajmljivanja()
        {
            Iznajmljivanjes = new HashSet<Iznajmljivanje>();
        }

        public int IdStatusIznajmljivanja { get; set; }
        public string NazivStatusa { get; set; } = null!;

        public virtual ICollection<Iznajmljivanje> Iznajmljivanjes { get; set; }
    }
}
