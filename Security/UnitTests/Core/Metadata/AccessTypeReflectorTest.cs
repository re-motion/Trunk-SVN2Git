using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.SampleDomain;
using Remotion.Security.UnitTests.TestDomain;

namespace Remotion.Security.UnitTests.Core.Metadata
{
  [TestFixture]
  public class AccessTypeReflectorTest
  {
    // types

    // static members

    // member fields

    private IEnumerationReflector _enumeratedTypeReflector;
    private AccessTypeReflector _accessTypeReflector;
    private MetadataCache _cache;

    // construction and disposing

    public AccessTypeReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _enumeratedTypeReflector = new EnumerationReflector ();
      _accessTypeReflector = new AccessTypeReflector (_enumeratedTypeReflector);
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsInstanceOfType (typeof (IAccessTypeReflector), _accessTypeReflector);
      Assert.AreSame (_enumeratedTypeReflector, _accessTypeReflector.EnumerationTypeReflector);
    }

    [Test]
    public void GetAccessTypesFromAssembly ()
    {
      List<EnumValueInfo> actualAccessTypes = _accessTypeReflector.GetAccessTypesFromAssembly (typeof (PaperFile).Assembly, _cache);

      Assert.IsNotNull (actualAccessTypes);
      Assert.AreEqual (2, actualAccessTypes.Count);
      EnumValueInfoAssert.Contains ("Journalize", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Archive", actualAccessTypes);
    }

    [Test]
    public void GetAccessTypesFromInstanceMethods ()
    {
      List<EnumValueInfo> actualAccessTypes = _accessTypeReflector.GetAccessTypesFromType (typeof (SecurableObjectWithSecuredInstanceMethods), _cache);

      Assert.IsNotNull (actualAccessTypes);
      Assert.AreEqual (9, actualAccessTypes.Count);
      EnumValueInfoAssert.Contains ("Create", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Read", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Edit", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Delete", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Search", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Find", actualAccessTypes);
      EnumValueInfoAssert.Contains ("First", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Second", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Third", actualAccessTypes);
    }

    [Test]
    public void GetAccessTypesFromStaticMethods ()
    {
      List<EnumValueInfo> actualAccessTypes = _accessTypeReflector.GetAccessTypesFromType (typeof (SecurableObjectWithSecuredStaticMethods), _cache);

      Assert.IsNotNull (actualAccessTypes);
      Assert.AreEqual (9, actualAccessTypes.Count);
      EnumValueInfoAssert.Contains ("Create", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Read", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Edit", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Delete", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Search", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Find", actualAccessTypes);
      EnumValueInfoAssert.Contains ("First", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Second", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Third", actualAccessTypes);
    }

    [Test]
    public void GetAccessTypesDerivedClassFromInstanceMethods ()
    {
      List<EnumValueInfo> actualAccessTypes = _accessTypeReflector.GetAccessTypesFromType (typeof (DerivedSecurableObjectWithSecuredInstanceMethods), _cache);

      Assert.IsNotNull (actualAccessTypes);
      Assert.AreEqual (10, actualAccessTypes.Count);
      EnumValueInfoAssert.Contains ("Create", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Read", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Edit", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Delete", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Search", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Find", actualAccessTypes);
      EnumValueInfoAssert.Contains ("First", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Second", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Third", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Fourth", actualAccessTypes);
    }

    [Test]
    public void GetAccessTypesDerivedClassFromStaticMethods ()
    {
      List<EnumValueInfo> actualAccessTypes = _accessTypeReflector.GetAccessTypesFromType (typeof (DerivedSecurableObjectWithSecuredStaticMethods), _cache);

      Assert.IsNotNull (actualAccessTypes);
      Assert.AreEqual (10, actualAccessTypes.Count);
      EnumValueInfoAssert.Contains ("Create", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Read", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Edit", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Delete", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Find", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Search", actualAccessTypes);
      EnumValueInfoAssert.Contains ("First", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Second", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Third", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Fourth", actualAccessTypes);
    }

    [Test]
    public void GetAccessTypesFromCache ()
    {
      List<EnumValueInfo> expectedAccessTypes = _accessTypeReflector.GetAccessTypesFromType (typeof (PaperFile), _cache);
      List<EnumValueInfo> actualAccessTypes = _cache.GetAccessTypes ();

      Assert.AreEqual (7, expectedAccessTypes.Count);
      foreach (EnumValueInfo expected in expectedAccessTypes)
        Assert.Contains (expected, actualAccessTypes);
    }

    [Test]
    public void GetAccessTypesFromType_SecuredProperties ()
    {
      List<EnumValueInfo> actualAccessTypes = _accessTypeReflector.GetAccessTypesFromType (typeof (SecurableObjectWithSecuredProperties), _cache);

      Assert.IsNotNull (actualAccessTypes);
      Assert.AreEqual (8, actualAccessTypes.Count);
      EnumValueInfoAssert.Contains ("Create", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Read", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Edit", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Delete", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Search", actualAccessTypes);
      EnumValueInfoAssert.Contains ("Find", actualAccessTypes);
      EnumValueInfoAssert.Contains ("ReadSecret", actualAccessTypes);
      EnumValueInfoAssert.Contains ("WriteSecret", actualAccessTypes);
    }
  }
}