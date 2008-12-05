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
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.TestDomain;

namespace Remotion.Security.UnitTests.Core.Metadata
{

  [TestFixture]
  public class EnumerationReflectorTest
  {
    // types

    // static members

    // member fields

    private EnumerationReflector _enumerationReflector;
    private MetadataCache _cache;

    // construction and disposing

    public EnumerationReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _enumerationReflector = new EnumerationReflector ();
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsInstanceOfType (typeof (IEnumerationReflector), _enumerationReflector);
    }

    [Test]
    public void GetValues ()
    {
      Dictionary<Enum, EnumValueInfo> values = _enumerationReflector.GetValues (typeof (DomainAccessTypes), _cache);

      Assert.IsNotNull (values);
      Assert.AreEqual (2, values.Count);

      Assert.AreEqual (0, values[DomainAccessTypes.Journalize].Value);
      Assert.AreEqual ("Journalize", values[DomainAccessTypes.Journalize].Name);
      Assert.AreEqual ("00000002-0001-0000-0000-000000000000", values[DomainAccessTypes.Journalize].ID);
      
      Assert.AreEqual (1, values[DomainAccessTypes.Archive].Value);
      Assert.AreEqual ("Archive", values[DomainAccessTypes.Archive].Name);
      Assert.AreEqual ("00000002-0002-0000-0000-000000000000", values[DomainAccessTypes.Archive].ID);
    }

    [Test]
    public void GetValue ()
    {
      EnumValueInfo value = _enumerationReflector.GetValue (DomainAccessTypes.Journalize, _cache);

      Assert.IsNotNull (value);

      Assert.AreEqual (0, value.Value);
      Assert.AreEqual ("Journalize", value.Name);
      Assert.AreEqual ("00000002-0001-0000-0000-000000000000", value.ID);
    }

    [Test]
    public void GetValuesFromCache ()
    {
      Dictionary<Enum, EnumValueInfo> values = _enumerationReflector.GetValues (typeof (DomainAccessTypes), _cache);

      Assert.AreSame (values[DomainAccessTypes.Journalize], _cache.GetEnumValueInfo (DomainAccessTypes.Journalize));
      Assert.AreSame (values[DomainAccessTypes.Archive], _cache.GetEnumValueInfo (DomainAccessTypes.Archive));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The type 'System.String' is not an enumerated type.\r\nParameter name: type")]
    public void GetMetadataWithInvalidType ()
    {
      new EnumerationReflector ().GetValues (typeof (string), _cache);
    }
  }
}
