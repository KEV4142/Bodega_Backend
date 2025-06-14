﻿using System.ComponentModel.DataAnnotations;

namespace Modelo.Entidades;

public partial class SalidaEnc
{
    [Key]
    public int SalidaID { get; set; }
    [Required]
    public int SucursalID { get; set; }
    [Required]
    public DateTime Fecha { get; set; }

    public DateTime? FechaRecibido { get; set; }
    [Required]
    public string UsuarioID { get; set; } = null!;

    public string? UsuarioRecibe { get; set; }
    [Required]
    [StringLength(1)]
    [RegularExpression("^[BER]$", ErrorMessage = "El estado solo puede ser 'B' o 'E' o 'R'.")]
    public string Estado { get; set; } = null!;

    public virtual ICollection<SalidaDet> SalidaDets { get; set; } = new List<SalidaDet>();

    public virtual Sucursal Sucursales { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;

    public virtual Usuario? UsuarioRecibeRelacion { get; set; }
}