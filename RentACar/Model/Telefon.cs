using System;
using System.Collections.Generic;

namespace RentACar.Model
{
    public partial class Telefon
    {
        public string Telefon1 { get; set; } = null!;
        public int KorisnikIdKorisnik { get; set; }

        public virtual Korisnik KorisnikIdKorisnikNavigation { get; set; } = null!;
    }
}
