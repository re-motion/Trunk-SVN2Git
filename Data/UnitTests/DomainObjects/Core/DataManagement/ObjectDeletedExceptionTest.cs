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
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class ObjectDeletedExceptionTest : StandardMappingTest
  {
    [Test]
    public void Serialization ()
    {
      ObjectDeletedException exception = new ObjectDeletedException (DomainObjectIDs.Order1);

      using (MemoryStream memoryStream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (memoryStream, exception);
        memoryStream.Seek (0, SeekOrigin.Begin);

        formatter = new BinaryFormatter ();

        exception = (ObjectDeletedException) formatter.Deserialize (memoryStream);

        Assert.AreEqual (DomainObjectIDs.Order1, exception.ID);
      }
    }
  }
}
