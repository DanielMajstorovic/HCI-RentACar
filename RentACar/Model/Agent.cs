using System;
using System.Collections.Generic;

namespace RentACar.Model
{
    public partial class Agent
    {
        public Agent()
        {
            Iznajmljivanjes = new HashSet<Iznajmljivanje>();
            Problems = new HashSet<Problem>();
        }

        public int KorisnikIdKorisnik { get; set; }
        public decimal? Plata { get; set; }

        public virtual Korisnik KorisnikIdKorisnikNavigation { get; set; } = null!;
        public virtual ICollection<Iznajmljivanje> Iznajmljivanjes { get; set; }
        public virtual ICollection<Problem> Problems { get; set; }
    }
}
