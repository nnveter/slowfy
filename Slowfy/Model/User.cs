using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.Model
{
    class User
    {
        public int id { get; set; }
        public string email { get; set; }
        public string password { get; set; } // Hashed password (bcrypt)
        public string name { get; set; } // Username
        public string avatarSrc { get; set; } // Link to avatar image on server
    }


}
