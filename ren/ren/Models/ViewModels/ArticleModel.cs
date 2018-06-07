using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ren.Models.ViewModels
{
    public class ArticleModel
    {
        [Required]
        [StringLength(40, ErrorMessage = "Title Max Length is 40")]
        public string Title { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Description Max Length is 100")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(2000, ErrorMessage = "Text Max Length is 2000")]
        public string Text { get; set; }
    }
}
