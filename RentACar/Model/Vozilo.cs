using System;
using System.Collections.Generic;

namespace RentACar.Model
{
    public partial class Vozilo
    {
        public Vozilo()
        {
            Iznajmljivanjes = new HashSet<Iznajmljivanje>();
        }

        public int IdVozilo { get; set; }
        public int KategorijaVozilaIdKategorijaVozila { get; set; }
        public string Marka { get; set; } = null!;
        public string Model { get; set; } = null!;
        public short Godiste { get; set; }
        public int Snaga { get; set; }
        public decimal DnevnaTarifa { get; set; }
        public decimal SedmicnaTarifa { get; set; }

        public virtual KategorijaVozila KategorijaVozilaIdKategorijaVozilaNavigation { get; set; } = null!;
        public virtual ICollection<Iznajmljivanje> Iznajmljivanjes { get; set; }
    }
}
