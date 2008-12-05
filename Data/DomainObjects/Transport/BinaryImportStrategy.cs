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
  /// Represents an import strategy for <see cref="DomainObject"/> instances using binary serialization. This matches <see cref="BinaryExportStrategy"/>.
  /// </summary>
  public class BinaryImportStrategy : IImportStrategy
  {
    public static readonly BinaryImportStrategy Instance = new BinaryImportStrategy ();

    public IEnumerable<TransportItem> Import (byte[] data)
    {
      using (MemoryStream stream = new MemoryStream (data))
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        try
        {
          KeyValuePair<string, Dictionary<string, object>>[] deserializedData = PerformDeserialization(stream, formatter);
          TransportItem[] transportedObjects = GetTransportItems (deserializedData);
          return transportedObjects;
        }
        catch (Exception ex)
        {
          throw new TransportationException ("Invalid data specified: " + ex.Message, ex);
        }
      }
    }

    protected virtual KeyValuePair<string, Dictionary<string, object>>[] PerformDeserialization (MemoryStream stream, BinaryFormatter formatter)
    {
      return (KeyValuePair<string, Dictionary<string, object>>[]) formatter.Deserialize (stream);
    }

    private TransportItem[] GetTransportItems (KeyValuePair<string, Dictionary<string, object>>[] deserializedData)
    {
      return Array.ConvertAll<KeyValuePair<string, Dictionary<string, object>>, TransportItem> (deserializedData,
          delegate (KeyValuePair<string, Dictionary<string, object>> pair)
          {
            ObjectID objectID = ObjectID.Parse (pair.Key);
            return new TransportItem (objectID, pair.Value);
          });
    }
  }
}
