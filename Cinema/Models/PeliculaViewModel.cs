using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cinema.Models
{
    public class PeliculaViewModel
    {
        public PeliculaViewModel() { 
        }
        public int idPelicula { get; set; }
        public string titulo { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fechaEstreno { get; set; }
        public Nullable<int> idDirector { get; set; }
        public string duracion { get; set; }
        public string idioma { get; set; }
        public Nullable<int> presupuesto { get; set; }
        public Nullable<int> idEstudio { get; set; }

        public virtual director director { get; set; }
        public virtual estudio estudio { get; set; }
        public List<int> genero { get; set; }        
        public List<int> actor { get; set; }
    }
}