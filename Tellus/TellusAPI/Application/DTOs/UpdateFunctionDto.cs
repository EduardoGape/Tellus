namespace TellusAPI.Application.DTOs
{
    public class UpdateFunctionDto
    {
        public string Name { get; set; } = string.Empty;
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool IsActive { get; set; }
    }
}
