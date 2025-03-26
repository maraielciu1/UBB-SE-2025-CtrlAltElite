namespace MarketPlace924.Domain;

public class Address
{
    public int Id { get; set; }
    public string? StreetLine { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? PostalCode { get; set; }
}