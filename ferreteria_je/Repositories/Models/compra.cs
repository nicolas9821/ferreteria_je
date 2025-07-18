    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using Dapper.Contrib.Extensions;
    using ferreteria_je.Repositories.Models;

    namespace ferreteria_je.Repositories.Models
    {
        [Table("compra")]
        public class compra
        {
            [Dapper.Contrib.Extensions.KeyAttribute]
            public int id_compra { get; set; }

            [Required(ErrorMessage = "La fecha de la compra es obligatoria.")]

            public DateTime fecha { get; set; }

            public int? id_proveedor { get; set; } 

            public int? id_usuario { get; set; }  

            [Required(ErrorMessage = "El total de la compra es obligatorio.")] 
            public decimal total { get; set; }
        // NUEVAS PROPIEDADES AGREGADAS AQUÍ
        [Dapper.Contrib.Extensions.Write(false)] // Indica a Dapper que no intente escribir esta columna en la DB
        public string nombre_proveedor { get; set; }

        [Dapper.Contrib.Extensions.Write(false)] // Indica a Dapper que no intente escribir esta columna en la DB
        public string nombre_usuario { get; set; }
        // FIN DE NUEVAS PROPIEDADES
    }
}