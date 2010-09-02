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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class ReflectionMappingHelperTest
  {
    [Test]
    public void GetPropertyName ()
    {
      string name = ReflectionMappingHelper.GetPropertyName (typeof(Order), "OrderNumber");
      Assert.That (name, Is.EqualTo (typeof (Order).FullName + ".OrderNumber"));
    }

    [Test]
    public void GetPropertyName_ForOverriddenProperty ()
    {
      string name = ReflectionMappingHelper.GetPropertyName (typeof(ClassWithMixedProperties), "Int32");
      Assert.That (name, Is.EqualTo (typeof (ClassWithMixedProperties).FullName + ".Int32"));
    }

    [Test]
    public void GetPropertyName_ForPropertyInGenericType ()
    {
      string name = ReflectionMappingHelper.GetPropertyName (typeof(GenericClassWithManySideRelationPropertiesNotInMapping<>), "BaseUnidirectional");
      Assert.That (name, Is.EqualTo (typeof (GenericClassWithManySideRelationPropertiesNotInMapping<>).FullName + ".BaseUnidirectional"));
    }
  }
}