using System.Collections.Generic;

namespace TellusAPI.Domain.Entities
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; }
        public List<Function> Functions { get; set; } = new List<Function>();
    }
}
