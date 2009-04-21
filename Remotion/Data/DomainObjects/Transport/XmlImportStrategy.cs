// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
