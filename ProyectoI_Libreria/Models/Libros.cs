using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoI_Libreria.Models
{
    public partial class Libros
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo ISBN es requerido.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "El campo T�tulo es requerido.")]
        [Display(Name = "T�tulo")]
        public string Titulo { get; set; }


        [Display(Name = "Casa Editorial")]
        [Required(ErrorMessage = "La casa editorial es requerido.")]
        public string CasaEditorial { get; set; }

        [Display(Name = "N�mero de Edici�n")]
        [Required(ErrorMessage = "El numero de edici�n es requerido.")]
        public Nullable<int> NumeroEdicion { get; set; }

        [Required(ErrorMessage = "El autor es requerida.")]
        public string Autor { get; set; }

        [Display(Name = "Cantidad Disponible")]
        [Required(ErrorMessage = "La cantidad de libros es requerida.")]
        public Nullable<int> CantidadDisponible { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Prestamos> Prestamos { get; set; }
    }
}
