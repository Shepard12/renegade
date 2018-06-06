using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ren.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
    }
}
