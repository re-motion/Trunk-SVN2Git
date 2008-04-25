using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Remotion.Data.DomainObjects.Transport
{
  /// <summary>
  /// Represents an import strategy for <see cref="DomainObject"/> instances using XML serialization. This matches <see cref="XmlExportStrategy"/>.
  /// </summary>
  public class XmlImportStrategy : IImportStrategy
  {
    public static readonly XmlImportStrategy Instance = new XmlImportStrategy();

    public IEnumerable<TransportItem> Import (byte[] data)
    {
      try
      {
        using (MemoryStream dataStream = new MemoryStream (data))
        {
          XmlSerializer formatter = new XmlSerializer (typeof (XmlTransportItem[]));
          return XmlTransportItem.Unwrap (PerformDeserialization(dataStream, formatter));
        }
      }
      catch (Exception ex)
      {
        throw new TransportationException ("Invalid data specified: " + ex.Message, ex);
      }
    }

    protected virtual XmlTransportItem[] PerformDeserialization (MemoryStream dataStream, XmlSerializer formatter)
    {
      return (XmlTransportItem[]) formatter.Deserialize (dataStream);
    }
  }
}