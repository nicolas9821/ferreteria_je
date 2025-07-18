using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Dapper.Contrib.Extensions; // Si estás usando Dapper.Contrib

namespace ferreteria_je.Repositories.Models
{
    [Table("proveedor")] // Opcional, útil con Dapper.Contrib
    public class proveedor
    {
        [Dapper.Contrib.Extensions.KeyAttribute]
        public int id_proveedor { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string nombre { get; set; }

        [StringLength(15)]
        public string telefono { get; set; }

        [StringLength(100)]
        public string direccion { get; set; }

        [StringLength(100)]
        public string email { get; set; }
    }
}