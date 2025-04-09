using System;
using System.Collections.Generic;

namespace RentACar.Model
{
    public partial class Mjesto
    {
        public Mjesto()
        {
            Korisniks = new HashSet<Korisnik>();
        }

        public int IdMjesto { get; set; }
        public string Naziv { get; set; } = null!;

        public virtual ICollection<Korisnik> Korisniks { get; set; }
    }
}
