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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.ExtensibleEnums;
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.Reflection.TypeDiscovery;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;

namespace Remotion.UnitTests.ExtensibleEnums.Infrastructure
{
  [TestFixture]
  public class ExtensibleEnumDefinitionCacheTest
  {
    private ExtensibleEnumDefinitionCache _cache;

    [SetUp]
    public void SetUp ()
    {
      _cache = ExtensibleEnumDefinitionCache.Instance;
    }

    [Test]
    public void ValueDiscoveryService ()
    {
      Assert.That (_cache.ValueDiscoveryService, Is.InstanceOfType (typeof (ExtensibleEnumValueDiscoveryService)));
      Assert.That (((ExtensibleEnumValueDiscoveryService) _cache.ValueDiscoveryService).TypeDiscoveryService, 
          Is.SameAs (ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService()));
    }

    [Test]
    public void GetDefinition ()
    {
      IExtensibleEnumDefinition instance = _cache.GetDefinition (typeof (Color));

      Assert.That (instance, Is.InstanceOfType (typeof (ExtensibleEnumDefinition<Color>)));
    }

    [Test]
    public void GetDefinition_SetsDiscoveryService ()
    {
      IExtensibleEnumDefinition instance = _cache.GetDefinition (typeof (Color));

      Assert.That (PrivateInvoke.GetNonPublicField (instance, "_valueDiscoveryService"), Is.SameAs (_cache.ValueDiscoveryService));
    }

    [Test]
    public void GetDefinition_Cached ()
    {
      IExtensibleEnumDefinition instance1 = _cache.GetDefinition (typeof (Color));
      IExtensibleEnumDefinition instance2 = _cache.GetDefinition (typeof (Color));

      Assert.That (instance1, Is.SameAs (instance2));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Type 'System.Object' is not an extensible enum type "
        + "derived from ExtensibleEnum<T>.\r\nParameter name: extensibleEnumType")]
    public void GetDefinition_ThrowsOnInvalidType ()
    {
      _cache.GetDefinition (typeof (object));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Type 'Remotion.UnitTests.ExtensibleEnums.TestDomain.MetallicColor' is not an extensible enum type "
        + "derived from ExtensibleEnum<T>.\r\nParameter name: extensibleEnumType")]
    public void GetDefinition_ThrowsOnDerivedEnum ()
    {
      _cache.GetDefinition (typeof (MetallicColor));
    }
  }
}
