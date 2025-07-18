using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Dapper.Contrib.Extensions; // Si estás usando Dapper.Contrib

namespace ferreteria_je.Repositories.Models
{
    [Table("producto")] // Opcional, útil con Dapper.Contrib
    public class producto
    {
        [Dapper.Contrib.Extensions.KeyAttribute]
        public int id_producto { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string nombre { get; set; }

        [StringLength(255)]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")] // NOT NULL
        // decimal(10,2) en MySQL se mapea bien a decimal en C#
        public decimal precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")] // NOT NULL
        // int en MySQL se mapea a int en C#
        public int stock { get; set; } = 0;
    }
}