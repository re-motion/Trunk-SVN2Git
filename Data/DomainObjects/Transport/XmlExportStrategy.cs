// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
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
