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
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.TestDomain;

namespace Remotion.Security.UnitTests.Core.Metadata
{

  [TestFixture]
  public class AbstractRoleReflectorTest
  {
    // types

    // static members

    // member fields

    private MockRepository _mocks;
    private IEnumerationReflector _enumeratedTypeReflectorMock;
    private AbstractRoleReflector _abstractRoleReflector;
    private MetadataCache _cache;

    // construction and disposing

    public AbstractRoleReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _enumeratedTypeReflectorMock = _mocks.StrictMock<IEnumerationReflector> ();
      _abstractRoleReflector = new AbstractRoleReflector (_enumeratedTypeReflectorMock);
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsInstanceOfType (typeof (IAbstractRoleReflector), _abstractRoleReflector);
      Assert.AreSame (_enumeratedTypeReflectorMock, _abstractRoleReflector.EnumerationTypeReflector);
    }

    [Test]
    public void GetAbstractRoles ()
    {
      Dictionary<Enum, EnumValueInfo> domainAbstractRoles = new Dictionary<Enum, EnumValueInfo> ();
      domainAbstractRoles.Add (DomainAbstractRoles.Clerk, AbstractRoles.Clerk);
      domainAbstractRoles.Add (DomainAbstractRoles.Secretary, AbstractRoles.Secretary);

      Dictionary<Enum, EnumValueInfo> specialAbstractRoles = new Dictionary<Enum, EnumValueInfo> ();
      specialAbstractRoles.Add (SpecialAbstractRoles.Administrator, AbstractRoles.Administrator);

      Expect.Call (_enumeratedTypeReflectorMock.GetValues (typeof (DomainAbstractRoles), _cache)).Return (domainAbstractRoles);
      Expect.Call (_enumeratedTypeReflectorMock.GetValues (typeof (SpecialAbstractRoles), _cache)).Return (specialAbstractRoles);
      _mocks.ReplayAll ();

      List<EnumValueInfo> values = _abstractRoleReflector.GetAbstractRoles (typeof (File).Assembly, _cache);

      _mocks.VerifyAll ();

      Assert.IsNotNull (values);
      Assert.AreEqual (3, values.Count);
      Assert.Contains (AbstractRoles.Clerk, values);
      Assert.Contains (AbstractRoles.Secretary, values);
      Assert.Contains (AbstractRoles.Administrator, values);
    }

    [Test]
    public void GetAbstractRolesFromCache ()
    {
      AbstractRoleReflector reflector = new AbstractRoleReflector ();
      List<EnumValueInfo> expectedAbstractRoles = reflector.GetAbstractRoles (typeof (File).Assembly, _cache);
      List<EnumValueInfo> actualAbstractRoles = _cache.GetAbstractRoles ();

      Assert.AreEqual (3, expectedAbstractRoles.Count);
      foreach (EnumValueInfo expected in expectedAbstractRoles)
        Assert.Contains (expected, actualAbstractRoles);
    }
  }
}
