namespace App2.Model
{
    public class User
    {
        public int id { get; set; }
        public string email { get; set; }
        public string password { get; set; } // Hashed password (bcrypt)
        public string name { get; set; } // Username
        public string avatarSrc { get; set; } // Link to avatar image on server
    }

}
