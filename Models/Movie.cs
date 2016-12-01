using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RoseBud.Models
{
    public class Movie
    {
        [Key]
        public int ID { get; set; }
        public string ImdbID { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Poster { get; set; }
    }
}