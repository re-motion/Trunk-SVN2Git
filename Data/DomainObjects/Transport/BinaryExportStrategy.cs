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
