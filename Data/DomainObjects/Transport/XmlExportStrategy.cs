/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
