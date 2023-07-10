using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoI_Libreria.Models
{
    public partial class Personas
    {
        public Personas()
        {
            this.Prestamos = new HashSet<Prestamos>();
        }

        public int Id { get; set; }

        [Display(Name = "Cédula de identidad")]
        [Required(ErrorMessage = "El campo CedulaIdentidad es requerido.")]
        public string CedulaIdentidad { get; set; }

        [Required(ErrorMessage = "El campo Nombre es requerido.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo Apellidos es requerido.")]
        public string Apellidos { get; set; }

        [Display(Name = "Fecha de Registro")]
        //[DataType(DataType.Date)]
        public Nullable<System.DateTime> FechaRegistro { get; set; }

        public Nullable<bool> Estado { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Prestamos> Prestamos { get; set; }
    }
}
