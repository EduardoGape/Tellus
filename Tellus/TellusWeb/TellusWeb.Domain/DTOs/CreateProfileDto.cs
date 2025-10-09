using System.Collections.Generic;
using TellusWeb.Domain.Entities;

namespace TellusWeb.Domain.DTOs
{
    public class CreateProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
        public List<Function> Functions { get; set; } = new List<Function>();
    }
}