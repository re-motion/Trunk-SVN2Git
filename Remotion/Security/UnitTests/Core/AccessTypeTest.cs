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
using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core
{

  [TestFixture]
  public class AccessTypeTest
  {
    [Test]
    public void GetAccessTypeFromEnum ()
    {
      AccessType accessType = AccessType.Get (new EnumWrapper (TestAccessTypes.First));

      Assert.AreEqual (new EnumWrapper (TestAccessTypes.First), accessType.Value);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Enumerated type 'Remotion.Security.UnitTests.Core.SampleDomain.TestAccessTypesWithoutAccessTypeAttribute' cannot be used as an access type. "
        + "Valid access types must have the Remotion.Security.AccessTypeAttribute applied.\r\nParameter name: accessType")]
    public void GetAccessTypeFromEnumWithoutAccessTypeAttribute ()
    {
      AccessType.Get (TestAccessTypesWithoutAccessTypeAttribute.First);
    }

    [Test]
    public void GetFromCache ()
    {
      Assert.AreEqual (AccessType.Get (TestAccessTypes.First), AccessType.Get (TestAccessTypes.First));
      Assert.AreEqual (AccessType.Get (new EnumWrapper (TestAccessTypes.Second)), AccessType.Get (TestAccessTypes.Second));
      Assert.AreEqual (AccessType.Get (new EnumWrapper (TestAccessTypes.Third)), AccessType.Get (new EnumWrapper (TestAccessTypes.Third)));
    }

    [Test]
    public void Test_ToString ()
    {
      EnumWrapper wrapper = new EnumWrapper(TestAccessTypes.First);
      AccessType accessType = AccessType.Get (wrapper);

      Assert.AreEqual (wrapper.ToString (), accessType.ToString ());
    }

    [Test]
    public void Serialization ()
    {
      AccessType accessType = AccessType.Get (new EnumWrapper (TestAccessTypes.First));
      AccessType deserializedAccessType = Serializer.SerializeAndDeserialize (accessType);

      Assert.AreEqual (accessType, deserializedAccessType);
    }
  }
}
