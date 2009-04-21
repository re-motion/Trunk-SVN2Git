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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class ObjectIDTest : StandardMappingTest
  {
    [Test]
    public void ObjectIDIsSerializable ()
    {
      ObjectID id = Serializer.SerializeAndDeserialize (DomainObjectIDs.Order1);
      Assert.AreEqual (DomainObjectIDs.Order1, id);
    }

    [Test]
    public void DeserializedContent_Value ()
    {
      ObjectID id = DomainObjectIDs.Order1;
      ObjectID deserializedID = Serializer.SerializeAndDeserialize (id);
     
      Assert.AreEqual (id.Value, deserializedID.Value);
    }

    [Test]
    public void DeserializedContent_ClassDefinition ()
    {
      ObjectID id = DomainObjectIDs.Order1;
      ObjectID deserializedID = Serializer.SerializeAndDeserialize (id);

      Assert.AreEqual (id.ClassDefinition, deserializedID.ClassDefinition);
    }
  }
}
