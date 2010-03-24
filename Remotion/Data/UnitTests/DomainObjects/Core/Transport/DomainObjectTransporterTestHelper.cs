// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System.Collections.Generic;
using System.IO;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Transport;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transport
{
  public static class DomainObjectTransporterTestHelper
  {
    public static byte[] GetBinaryDataFor (params ObjectID[] ids)
    {
      var transporter = new DomainObjectTransporter();
      foreach (ObjectID id in ids)
        transporter.Load (id);
      return GetBinaryDataFor (transporter);
    }

    public static byte[] GetBinaryDataFor (DomainObjectTransporter transporter)
    {
      using (var stream = new MemoryStream())
      {
        transporter.Export (stream);
        return stream.ToArray();
      }
    }

    public static List<DomainObject> ImportObjects (params ObjectID[] objectsToImport)
    {
      byte[] binaryData = GetBinaryDataFor (objectsToImport);
      return ImportObjects (binaryData);
    }

    public static List<DomainObject> ImportObjects (byte[] binaryData)
    {
      TransportedDomainObjects transportedObjects = Import (binaryData);
      return new List<DomainObject> (transportedObjects.TransportedObjects);
    }

    public static TransportedDomainObjects Import (byte[] binaryData)
    {
      using (var stream = new MemoryStream (binaryData))
      {
        return DomainObjectImporter.CreateImporterFromStream (stream, BinaryImportStrategy.Instance).GetImportedObjects();
      }
    }
  }
}