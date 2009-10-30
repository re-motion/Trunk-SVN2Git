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
using NUnit.Framework.SyntaxHelpers;
using Remotion.ExtensibleEnums;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;

namespace Remotion.UnitTests.ExtensibleEnums
{
  [TestFixture]
  public class ExtensibleEnumValuesCacheTest
  {
    private ExtensibleEnumValuesCache _cache;

    [SetUp]
    public void SetUp ()
    {
      _cache = ExtensibleEnumValuesCache.Instance;
    }

    [Test]
    public void GetInstance ()
    {
      IExtensibleEnumValues instance = _cache.GetValues (typeof (Color));

      Assert.That (instance, Is.InstanceOfType (typeof (ExtensibleEnumValues<Color>)));
    }

    [Test]
    public void GetValues_Cached ()
    {
      IExtensibleEnumValues instance1 = _cache.GetValues (typeof (Color));
      IExtensibleEnumValues instance2 = _cache.GetValues (typeof (Color));

      Assert.That (instance1, Is.SameAs (instance2));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Type 'System.Object' is not an extensible enum type "
        + "directly derived from ExtensibleEnum<T>.\r\nParameter name: extensibleEnumType")]
    public void GetValues_ThrowsOnInvalidType ()
    {
      _cache.GetValues (typeof (object));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Type 'Remotion.UnitTests.ExtensibleEnums.TestDomain.MetallicColor' is not an extensible enum type "
        + "directly derived from ExtensibleEnum<T>.\r\nParameter name: extensibleEnumType")]
    public void GetValues_ThrowsOnDerivedEnum ()
    {
      _cache.GetValues (typeof (MetallicColor));
    }
  }
}