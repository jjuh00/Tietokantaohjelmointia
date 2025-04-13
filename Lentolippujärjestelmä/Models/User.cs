namespace Lentolippujärjestelmä.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Tallennetaan vain salattu salasana
        public int Role { get; set; } = 1; // 0 = admin, 1 = normaali käyttäjä
    }
}