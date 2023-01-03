using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Models
{
    public class Crypto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Value { get; set; }
    }
}
