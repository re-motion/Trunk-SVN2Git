using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Remotion.Data.DomainObjects.Transport
{
  /// <summary>
  /// Represents an export strategy for <see cref="DomainObject"/> instances using binary serialization. This matches <see cref="BinaryImportStrategy"/>.
  /// </summary>
  public class BinaryExportStrategy : IExportStrategy
  {
    public static readonly BinaryExportStrategy Instance = new BinaryExportStrategy();

    public byte[] Export (TransportItem[] transportedItems)
    {
      using (MemoryStream dataStream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        KeyValuePair<string, Dictionary<string, object>>[] versionIndependentItems = GetVersionIndependentItems (transportedItems);
        PerformSerialization(versionIndependentItems, dataStream, formatter);
        return dataStream.ToArray ();
      }
    }

    protected virtual void PerformSerialization (KeyValuePair<string, Dictionary<string, object>>[] versionIndependentItems, MemoryStream dataStream,
        BinaryFormatter formatter)
    {
      formatter.Serialize (dataStream, versionIndependentItems);
    }

    private KeyValuePair<string, Dictionary<string, object>>[] GetVersionIndependentItems (TransportItem[] transportItems)
    {
      return Array.ConvertAll<TransportItem, KeyValuePair<string, Dictionary<string, object>>> (transportItems, 
          delegate (TransportItem item) { return new KeyValuePair<string, Dictionary<string, object>> (item.ID.ToString (), item.Properties); });
    }
  }
}