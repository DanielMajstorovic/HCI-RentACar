using System;
using System.Collections.Generic;

namespace RentACar.Model
{
    public partial class Klijent
    {
        public Klijent()
        {
            Iznajmljivanjes = new HashSet<Iznajmljivanje>();
        }

        public int IdKlijent { get; set; }
        public string Ime { get; set; } = null!;
        public string Prezime { get; set; } = null!;
        public string BrojTelefona { get; set; } = null!;
        public string? Email { get; set; }

        public virtual ICollection<Iznajmljivanje> Iznajmljivanjes { get; set; }

        public override string? ToString()
        {
            return $"{Ime} {Prezime}";
        }
    }
}
