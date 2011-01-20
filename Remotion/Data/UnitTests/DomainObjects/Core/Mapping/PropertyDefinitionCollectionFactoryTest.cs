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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class PropertyDefinitionCollectionFactoryTest
  {
    private PropertyDefinitionCollectionFactory _factory;
    private IMappingNameResolver _mappingNameResolverMock;

    [SetUp]
    public void SetUp ()
    {
      _mappingNameResolverMock = MockRepository.GenerateStrictMock<IMappingNameResolver>();
      _factory = new PropertyDefinitionCollectionFactory(_mappingNameResolverMock);
    }

    [Test]
    public void CreatePropertyDefinitions ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var propertyInfo1 = typeof (Order).GetProperty ("OrderNumber");
      var propertyInfo2 = typeof (Order).GetProperty ("DeliveryDate");

      _mappingNameResolverMock
          .Expect (mock => mock.GetPropertyName (Arg<PropertyInfoAdapter>.Matches (pi => pi.PropertyInfo == propertyInfo1)))
          .Return ("FakeName1");
      _mappingNameResolverMock
          .Expect (mock => mock.GetPropertyName (Arg<PropertyInfoAdapter>.Matches (pi => pi.PropertyInfo == propertyInfo2)))
          .Return ("FakeName2");
      _mappingNameResolverMock.Replay();

      var result = _factory.CreatePropertyDefinitions (classDefinition, new[] { propertyInfo1, propertyInfo2 });

      _mappingNameResolverMock.VerifyAllExpectations();
      Assert.That (result.Count, Is.EqualTo (2));
      Assert.That (result[0].PropertyInfo, Is.SameAs (propertyInfo1));
      Assert.That (result[1].PropertyInfo, Is.SameAs (propertyInfo2));
    }
  }
}