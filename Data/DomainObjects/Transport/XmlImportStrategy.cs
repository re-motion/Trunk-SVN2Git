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
