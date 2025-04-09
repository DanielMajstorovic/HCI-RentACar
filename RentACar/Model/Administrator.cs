using System;
using System.Collections.Generic;

namespace RentACar.Model
{
    public partial class Administrator
    {
        public Administrator()
        {
            Problems = new HashSet<Problem>();
        }

        public int KorisnikIdKorisnik { get; set; }

        public virtual Korisnik KorisnikIdKorisnikNavigation { get; set; } = null!;
        public virtual ICollection<Problem> Problems { get; set; }
    }
}
