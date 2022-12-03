using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.Model
{
    public class Track
    {
        public int id { get; set; }
        public string author { get; set; }
        public string title { get; set; }
        public string duration { get; set; }
        public string source { get; set; }
        public bool e { get; set; }
        public int listid { get; set; } = 0;
        public string like { get; set; } = "ms-appx:///Views/hear1.png";
        public string image { get; set; }
        
    }
}
