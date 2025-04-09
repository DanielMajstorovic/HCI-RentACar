using System;
using System.Collections.Generic;

namespace RentACar.Model
{
    public partial class Korisnik
    {
        public Korisnik()
        {
            Telefons = new HashSet<Telefon>();
        }

        public int IdKorisnik { get; set; }
        public string KorisnickoIme { get; set; } = null!;
        public string Lozinka { get; set; } = null!;
        public string Eposta { get; set; } = null!;
        public string TipNaloga { get; set; } = null!;
        public int MjestoIdMjesto { get; set; }
        public int? Tema { get; set; }
        public int? Jezik { get; set; }

        public virtual Mjesto MjestoIdMjestoNavigation { get; set; } = null!;
        public virtual Administrator? Administrator { get; set; }
        public virtual Agent? Agent { get; set; }
        public virtual ICollection<Telefon> Telefons { get; set; }
    }
}
