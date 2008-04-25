using System;
using System.Collections.Generic;
using NUnit.Framework;
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
      PrivateInvoke.SetNonPublicStaticField (typeof (AccessType), "s_cache", new Dictionary<EnumWrapper, AccessType> ());
    }
  }

}