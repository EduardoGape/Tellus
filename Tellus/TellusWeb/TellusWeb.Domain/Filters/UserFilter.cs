namespace TellusWeb.Domain.Filters
{
    public class UserFilter
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? ProfileId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}