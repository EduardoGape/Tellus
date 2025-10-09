namespace TellusWeb.Domain.DTOs
{
    public class CreateFunctionDto
    {
        public string Name { get; set; } = string.Empty;
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool IsActive { get; set; } = true;
    }
}