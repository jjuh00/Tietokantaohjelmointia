﻿namespace Lentolippujärjestelmä.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Tallennetaan vain salattu salasana
        public bool IsAdmin { get; set; } = false;
    }
}
