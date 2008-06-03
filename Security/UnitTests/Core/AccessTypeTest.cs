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
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core
{

  [TestFixture]
  public class AccessTypeTest
  {
    [SetUp]
    public void SetUp ()
    {
      ClearAccessTypeCache ();
    }

    [TearDown]
    public void TearDown ()
    {
      ClearAccessTypeCache ();
    }

    [Test]
    public void GetAccessTypeFromEnum ()
    {
      AccessType accessType = AccessType.Get (new EnumWrapper (TestAccessTypes.First));

      Assert.AreEqual (new EnumWrapper (TestAccessTypes.First), accessType.Value);
    }

    [Test]
    [Ignore]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Enumerated type 'Remotion.Security.UnitTests.Core.SampleDomain.TestAccessTypesWithoutAccessTypeAttribute' cannot be used as an access type. "
        + "Valid access types must have the Remotion.Security.AccessTypeAttribute applied.\r\nParameter name: accessType")]
    public void GetAccessTypeFromEnumWithoutAccessTypeAttribute ()
    {
      AccessType.Get (new EnumWrapper (TestAccessTypesWithoutAccessTypeAttribute.First));
    }

    [Test]
    public void GetFromCache ()
    {
      Assert.AreSame (AccessType.Get (TestAccessTypes.First), AccessType.Get (TestAccessTypes.First));
      Assert.AreSame (AccessType.Get (TestAccessTypes.Second), AccessType.Get (TestAccessTypes.Second));
      Assert.AreSame (AccessType.Get (TestAccessTypes.Third), AccessType.Get (TestAccessTypes.Third));
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

      Assert.AreSame (accessType, deserializedAccessType);
    }

    private void ClearAccessTypeCache ()
    {
      PrivateInvoke.SetNonPublicStaticField (typeof (AccessType), "s_cache", new InterlockedCache<EnumWrapper, AccessType> ());
    }
  }

}
