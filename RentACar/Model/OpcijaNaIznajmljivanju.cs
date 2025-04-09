using System;
using System.Collections.Generic;

namespace RentACar.Model
{
    public partial class OpcijaNaIznajmljivanju
    {
        public int OpcijaIdOpcija { get; set; }
        public int IznajmljivanjeIdIznajmljivanje { get; set; }
        public int IdOpcijaNaIznajmljivanju { get; set; }

        public virtual Iznajmljivanje IznajmljivanjeIdIznajmljivanjeNavigation { get; set; } = null!;
        public virtual Opcija OpcijaIdOpcijaNavigation { get; set; } = null!;
    }
}
