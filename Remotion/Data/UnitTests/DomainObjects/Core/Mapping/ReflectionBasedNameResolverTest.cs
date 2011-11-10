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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ReflectionBasedNameResolverTest : MappingReflectionTestBase
  {
    private ReflectionBasedNameResolver _resolver;
    private IPropertyInformation _propertyInformationStub;
    private ITypeInformation _typeInformationStub;

    public override void SetUp ()
    {
      base.SetUp();
      _resolver = new ReflectionBasedNameResolver();
      _propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      _typeInformationStub = MockRepository.GenerateStub<ITypeInformation> ();
    }

    [Test]
    public void GetPropertyName ()
    {
      _typeInformationStub.Stub (stub => stub.FullName).Return ("Namespace.Class");

      _propertyInformationStub.Stub (stub => stub.Name).Return ("Property");
      _propertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType ()).Return (_typeInformationStub);

      Assert.That (_resolver.GetPropertyName (_propertyInformationStub), Is.EqualTo ("Namespace.Class.Property"));
    }

    [Test]
    public void GetPropertyName_Twice_ReturnsSameResultFromCache ()
    {
      _typeInformationStub.Stub (stub => stub.FullName).Return ("Namespace.Class");
      _propertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType ()).Return (_typeInformationStub);

      _propertyInformationStub.Stub (stub => stub.Name).Return ("Property1").Repeat.Once();
      string name1 = _resolver.GetPropertyName (_propertyInformationStub);

      _propertyInformationStub.Stub (stub => stub.Name).Return ("Property2");
      Assert.That (_propertyInformationStub.Name, Is.EqualTo ("Property2"));
      string name2 = _resolver.GetPropertyName (_propertyInformationStub);

      Assert.That (name1, Is.SameAs(name2));
      Assert.That (name1, Is.EqualTo ("Namespace.Class.Property1"));
    }

    [Test]
    public void GetPropertyName_ForOverriddenProperty ()
    {
      var derivedTypeStub = MockRepository.GenerateStub<ITypeInformation>();
      derivedTypeStub.Stub (stub => stub.FullName).Return ("Namespace.Derived");
      _propertyInformationStub.Stub (stub => stub.DeclaringType).Return (derivedTypeStub);

      _typeInformationStub.Stub (stub => stub.FullName).Return ("Namespace.Class");
      _propertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType()).Return (_typeInformationStub);
      _propertyInformationStub.Stub (stub => stub.Name).Return ("Property");
      Assert.That (_resolver.GetPropertyName (_propertyInformationStub), Is.EqualTo ("Namespace.Class.Property"));
    }

    [Test]
    public void GetPropertyName_ForPropertyInClosedGenericType ()
    {
      var genericTypeDefinitionStub = MockRepository.GenerateStub<ITypeInformation> ();
      genericTypeDefinitionStub.Stub (stub => stub.FullName).Return ("Namespace.OpenGeneric<>");

      _typeInformationStub.Stub (stub => stub.FullName).Return ("Namespace.ClosedGeneric");
      _typeInformationStub.Stub (stub => stub.IsGenericType).Return (true);
      _typeInformationStub.Stub (stub => stub.IsGenericTypeDefinition).Return (false);
      _typeInformationStub.Stub (stub => stub.GetGenericTypeDefinition()).Return (genericTypeDefinitionStub);

      _propertyInformationStub.Stub (stub => stub.Name).Return ("Property");
      _propertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType ()).Return (_typeInformationStub);
      Assert.That (_resolver.GetPropertyName (_propertyInformationStub), Is.EqualTo ("Namespace.OpenGeneric<>.Property"));
    }

    [Test]
    public void GetPropertyName_ForPropertyInOpenGenericType ()
    {
      _typeInformationStub.Stub (stub => stub.FullName).Return ("Namespace.OpenGeneric<>");
      _typeInformationStub.Stub (stub => stub.IsGenericType).Return (true);
      _typeInformationStub.Stub (stub => stub.IsGenericTypeDefinition).Return (true);

      _propertyInformationStub.Stub (stub => stub.Name).Return ("Property");
      _propertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType ()).Return (_typeInformationStub);
      Assert.That (_resolver.GetPropertyName (_propertyInformationStub), Is.EqualTo ("Namespace.OpenGeneric<>.Property"));
    }

  }
}
