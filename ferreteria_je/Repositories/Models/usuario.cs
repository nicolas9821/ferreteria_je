// ferreteria_je\Repositories\Models\usuario.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Dapper.Contrib.Extensions; // Importante para [Table] y [Key]

namespace ferreteria_je.Repositories.Models
{
    [Table("usuarios")] // La tabla en la base de datos sigue siendo 'usuarios' (plural). Esto es correcto.
    public class usuario // <-- ¡CLASE RENOMBRADA A 'usuario' (singular)!
    {
        [Dapper.Contrib.Extensions.KeyAttribute] // Asegúrate de usar [Key] de Dapper.Contrib.Extensions
        public int id_usuario { get; set; } // Propiedad id_usuario permanece igual

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string nombre { get; set; }

        [Required]
        [StringLength(15)]
        public string telefono { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [StringLength(100)]
        public string email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(255)]
        public string password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [StringLength(50)]
        public string rol { get; set; }
    }
}



