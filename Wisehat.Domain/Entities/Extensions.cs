using System.Numerics;

namespace Wisehat.Domain.Entities;

public static class Extensions
{
  public static Vector2 ToVector2(this object obj)
  {
    ArgumentNullException.ThrowIfNull(obj);

    if (obj is not Vector2) throw new ArgumentException(nameof(obj));

    return (Vector2)obj;
  }
}
