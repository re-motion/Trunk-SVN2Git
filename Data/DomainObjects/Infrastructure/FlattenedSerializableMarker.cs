using System;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  [Serializable]
  internal class FlattenedSerializableMarker
  {
    public static readonly FlattenedSerializableMarker Instance = new FlattenedSerializableMarker();
  }
}