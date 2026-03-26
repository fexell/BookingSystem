namespace BookingSystem.Shared.DTOs;

public class ResourceDto {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}