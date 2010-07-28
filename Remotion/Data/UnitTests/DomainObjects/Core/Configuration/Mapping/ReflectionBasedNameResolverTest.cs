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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class ReflectionBasedNameResolverTest : MappingReflectionTestBase
  {
    private PropertyInfo _orderNumberProperty;
    private PropertyInfo _overriddenProperty;
    private PropertyInfo _overriddenPropertyOnBase;
    private PropertyInfo _propertyInGenericClass;
    private PropertyInfo _propertyInOpenGenericClass;
    private ReflectionBasedNameResolver _resolver;

    public override void SetUp ()
    {
      base.SetUp();
      _resolver = new ReflectionBasedNameResolver();
      _orderNumberProperty = typeof (Order).GetProperty ("OrderNumber");
      _overriddenProperty = typeof (DerivedClassWithMixedProperties).GetProperty ("Int32");
      _overriddenPropertyOnBase = typeof (ClassWithMixedProperties).GetProperty ("Int32");
      _propertyInGenericClass = typeof (ClosedGenericClassWithManySideRelationProperties).GetProperty ("BaseUnidirectional");
      _propertyInOpenGenericClass = typeof (GenericClassWithManySideRelationPropertiesNotInMapping<>).GetProperty ("BaseUnidirectional");
    }

    [Test]
    public void GetPropertyName ()
    {
      string name = _resolver.GetPropertyName (_orderNumberProperty);
      Assert.That (name, Is.EqualTo (typeof (Order).FullName + ".OrderNumber"));
    }

    [Test]
    public void GetPropertyName_ForOverriddenProperty ()
    {
      string name = _resolver.GetPropertyName (_overriddenProperty);
      Assert.That (name, Is.EqualTo (typeof (ClassWithMixedProperties).FullName + ".Int32"));
    }

    [Test]
    public void GetPropertyName_ForPropertyInGenericType ()
    {
      string name = _resolver.GetPropertyName (_propertyInGenericClass);
      Assert.That (name, Is.EqualTo (typeof (GenericClassWithManySideRelationPropertiesNotInMapping<>).FullName + ".BaseUnidirectional"));
    }
  }
}
