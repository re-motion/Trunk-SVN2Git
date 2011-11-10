// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
      Assert.IsInstanceOf (typeof (IAccessTypeReflector), _accessTypeReflector);
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
