using System;
using System.Collections.Generic;

namespace RentACar.Model
{
    public partial class KategorijaVozila
    {
        public KategorijaVozila()
        {
            Vozilos = new HashSet<Vozilo>();
        }

        public int IdKategorijaVozila { get; set; }
        public string? Naziv { get; set; }

        public virtual ICollection<Vozilo> Vozilos { get; set; }
    }
}
