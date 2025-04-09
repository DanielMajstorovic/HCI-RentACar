using System;
using System.Collections.Generic;

namespace RentACar.Model
{
    public partial class Iznajmljivanje
    {
        public Iznajmljivanje()
        {
            OpcijaNaIznajmljivanjus = new HashSet<OpcijaNaIznajmljivanju>();
        }

        public int IdIznajmljivanje { get; set; }
        public int AgentKorisnikIdKorisnik { get; set; }
        public int VoziloKategorijaVozilaIdKategorijaVozila { get; set; }
        public int VoziloIdVozilo { get; set; }
        public decimal Cijena { get; set; }
        public DateOnly DatumIznajmljivanja { get; set; }
        public DateOnly? DatumVracanja { get; set; }
        public int KlijentIdKlijent { get; set; }
        public string? DodatniOpis { get; set; }
        public int StatusIznajmljivanjaIdStatusIznajmljivanja { get; set; }

        public virtual Agent AgentKorisnikIdKorisnikNavigation { get; set; } = null!;
        public virtual Klijent KlijentIdKlijentNavigation { get; set; } = null!;
        public virtual Statusiznajmljivanja StatusIznajmljivanjaIdStatusIznajmljivanjaNavigation { get; set; } = null!;
        public virtual Vozilo VoziloIdVoziloNavigation { get; set; } = null!;
        public virtual ICollection<OpcijaNaIznajmljivanju> OpcijaNaIznajmljivanjus { get; set; }
    }
}
