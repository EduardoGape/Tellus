using System.Collections.Generic;
using TellusAPI.Domain.Entities;

namespace TellusAPI.Application.DTOs
{
    public class CreateProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; }
        public List<Function> Functions { get; set; } = new List<Function>();
    }
}
