using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Wisehat.Domain.Entities;

public class WebProject
{
  [BsonId]
  public Guid Id { get; set; }

  public required string ProfileId { get; set; }

  [MaxLength(50)]
  public required string Name { get; set; }

  public string? Description { get; set; }

  public List<Widget> Widgets { get; set; } = [];

  public DateTime CreatedOn { get; set; }

  public DateTime LastModified { get; set; }
}
