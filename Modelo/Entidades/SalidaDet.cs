using System;
using System.Collections.Generic;

namespace Modelo.Entidades;

public partial class SalidaDet
{
    public int SalidaDetID { get; set; }

    public int SalidaID { get; set; }

    public int LoteID { get; set; }

    public int Cantidad { get; set; }

    public virtual Lote Lote { get; set; } = null!;

    public virtual SalidaEnc Salida { get; set; } = null!;
}
