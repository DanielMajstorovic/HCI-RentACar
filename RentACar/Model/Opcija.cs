using System;
using System.Collections.Generic;

namespace RentACar.Model
{
    public partial class Opcija
    {
        public Opcija()
        {
            OpcijaNaIznajmljivanjus = new HashSet<OpcijaNaIznajmljivanju>();
        }

        public int IdOpcija { get; set; }
        public string Naziv { get; set; } = null!;
        public string? Opis { get; set; }
        public decimal Cijena { get; set; }

        public virtual ICollection<OpcijaNaIznajmljivanju> OpcijaNaIznajmljivanjus { get; set; }
    }
}
