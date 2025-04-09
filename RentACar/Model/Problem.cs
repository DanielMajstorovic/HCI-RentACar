using System;
using System.Collections.Generic;

namespace RentACar.Model
{
    public partial class Problem
    {
        public int IdProblem { get; set; }
        public string OpisProblema { get; set; } = null!;
        public DateTime DatumKreiranja { get; set; }
        public int? AdministratorKorisnikIdKorisnik { get; set; }
        public int AgentKorisnikIdKorisnik { get; set; }
        public string? PovratneInformacije { get; set; }
        public DateTime? DatumObrade { get; set; }

        public virtual Administrator? AdministratorKorisnikIdKorisnikNavigation { get; set; }
        public virtual Agent AgentKorisnikIdKorisnikNavigation { get; set; } = null!;
    }
}
