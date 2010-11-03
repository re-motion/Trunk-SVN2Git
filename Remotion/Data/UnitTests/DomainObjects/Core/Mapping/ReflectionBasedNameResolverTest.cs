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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ReflectionBasedNameResolverTest : MappingReflectionTestBase
  {
    private ReflectionBasedNameResolver _resolver;
    private IPropertyInformation _propertyInformationStub;

    public override void SetUp ()
    {
      base.SetUp();
      _resolver = new ReflectionBasedNameResolver();
      _propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
    }

    [Test]
    public void GetPropertyName ()
    {
      _propertyInformationStub.Stub (stub => stub.Name).Return ("OrderNumber");
      _propertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType ()).Return (typeof (Order));
      string name = _resolver.GetPropertyName (_propertyInformationStub);
      Assert.That (name, Is.EqualTo (typeof (Order).FullName + ".OrderNumber"));
    }

    [Test]
    public void GetPropertyName_Twice_ReturnsSameResultFromCache ()
    {
      _propertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType ()).Return (typeof (Order));

      _propertyInformationStub.Stub (stub => stub.Name).Return ("OrderNumber1");
      string name1 = _resolver.GetPropertyName (_propertyInformationStub);

      _propertyInformationStub.Stub (stub => stub.Name).Return ("OrderNumber2");
      string name2 = _resolver.GetPropertyName (_propertyInformationStub);

      Assert.That (name1, Is.SameAs(name2));
      Assert.That (name1, Is.EqualTo (typeof (Order).FullName+".OrderNumber1"));
    }

    [Test]
    public void GetPropertyName_ForOverriddenProperty ()
    {
      _propertyInformationStub.Stub (stub => stub.Name).Return ("Int32");
      _propertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType ()).Return (typeof (ClassWithMixedProperties));
      string name = _resolver.GetPropertyName (_propertyInformationStub);
      Assert.That (name, Is.EqualTo (typeof (ClassWithMixedProperties).FullName + ".Int32"));
    }

    [Test]
    public void GetPropertyName_ForPropertyInGenericType ()
    {
      _propertyInformationStub.Stub (stub => stub.Name).Return ("BaseUnidirectional");
      _propertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType ()).Return (typeof (GenericClassWithRealRelationEndPointsNotInMapping<>));
      string name = _resolver.GetPropertyName (_propertyInformationStub);
      Assert.That (name, Is.EqualTo (typeof (GenericClassWithRealRelationEndPointsNotInMapping<>).FullName + ".BaseUnidirectional"));
    }
   
  }
}
