using System;
using System.Collections.Generic;

namespace MiPlata.Models;

public partial class Transaccione
{
    public int IdTransaccion { get; set; }

    public int IdUsuario { get; set; }

    public string Tipo { get; set; } = null!;

    public decimal Monto { get; set; }

    public DateTime Fecha { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
