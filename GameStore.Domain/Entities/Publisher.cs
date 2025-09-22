using System.ComponentModel.DataAnnotations;

namespace GameStore.Domain.Entities;
public class Publisher : BaseEntity
{
    [Required]
    public string CompanyName { get; set; }

    public int? SupplierID { get; set; }

    public string? ContactName { get; set; }

    public string? ContactTitle { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Region { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public string? Phone { get; set; }

    public string? Fax { get; set; }

    public string? HomePage { get; set; }

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<Game> Games { get; set; }
}
