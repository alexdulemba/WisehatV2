// Author: Alexander Dulemba
// Copyright 2023

using System.Numerics;

namespace Wisehat.Domain.Entities;

public static class Extensions
{
  public static Vector2 ToVector2(this object obj)
  {
    ArgumentNullException.ThrowIfNull(obj);

    if (obj is not Vector2) 
      throw new ArgumentException("Failed to convert object to Vector2 since underlying type is not Vector2");

    return (Vector2)obj;
  }
}
