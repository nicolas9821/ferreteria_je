using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ferreteria_je.Repositories.Models;
using System.ComponentModel.DataAnnotations;

namespace ferreteria_je.Repositories.Models
{
    [Table("factura")]
    public class factura
    {
        [Dapper.Contrib.Extensions.KeyAttribute]
        public int id_factura { get; set; }

        [Required(ErrorMessage = "La fecha de la factura es obligatoria.")]

        public DateTime fecha { get; set; }

        public int? id_cliente { get; set; }  // Clave foránea con ON DELETE CASCADE

        // Propiedad para mostrar el nombre del cliente en la grilla (no mapeada directamente a la tabla factura)
        [Write(false)] // Indica a Dapper.Contrib que no intente escribir esta propiedad en la BD
        public string nombre_cliente { get; set; }

        [Required(ErrorMessage = "El total de la factura es obligatorio.")] // NOT NULL
        public decimal total { get; set; }
    }
}