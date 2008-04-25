using System;
using System.IO;
using System.Xml.Serialization;

namespace Remotion.Data.DomainObjects.Transport
{
  /// <summary>
  /// Represents an export strategy for <see cref="DomainObject"/> instances using XML serialization. This matches <see cref="XmlImportStrategy"/>.
  /// </summary>
  public class XmlExportStrategy : IExportStrategy
  {
    public static readonly XmlExportStrategy Instance = new XmlExportStrategy();

    public byte[] Export (TransportItem[] transportedObjects)
    {
      using (MemoryStream dataStream = new MemoryStream ())
      {
        XmlSerializer formatter = new XmlSerializer (typeof (XmlTransportItem[]));
        PerformSerialization(XmlTransportItem.Wrap (transportedObjects), dataStream, formatter);
        return dataStream.ToArray ();
      }
    }

    protected virtual void PerformSerialization (XmlTransportItem[] transportedObjects, MemoryStream dataStream, XmlSerializer formatter)
    {
      formatter.Serialize (dataStream, transportedObjects);
    }
  }
}