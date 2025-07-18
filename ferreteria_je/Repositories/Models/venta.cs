using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Dapper.Contrib.Extensions;
using ferreteria_je.Repositories.Models;

namespace ferreteria_je.Repositories.Models
{
    [Table("venta")]
    public class venta
    {
        [Dapper.Contrib.Extensions.KeyAttribute]
        public int id_venta { get; set; }

        [Required(ErrorMessage = "La fecha de la compra es obligatoria.")]

        public DateTime fecha { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio.")] // NOT NULL
        // decimal(10,2) en MySQL se mapea bien a decimal en C#
        public decimal precio_unitario { get; set; }

        [Required]
        public int cantidad { get; set; }

        public int? id_cliente { get; set; }  // Clave foránea con ON DELETE CASCADE

        public int? id_usuario { get; set; }  // Clave foránea con ON DELETE CASCADE
        public int? id_producto { get; set; }
        
        [Required(ErrorMessage = "El total de la venta es obligatorio.")] // NOT NULL
        public decimal total { get; set; }
    }
}
