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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ReflectionBasedMappingObjectFactoryTest
  {
    private ReflectionBasedMappingObjectFactory _factory;
    private ReflectionBasedNameResolver _mappingNameResolver;

    [SetUp]
    public void SetUp ()
    {
      _mappingNameResolver = new ReflectionBasedNameResolver();
      _factory = new ReflectionBasedMappingObjectFactory (_mappingNameResolver);
    }

    [Test]
    public void CreateClassDefinition ()
    {
      var result = _factory.CreateClassDefinition (typeof (Order), null);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.ClassType, Is.SameAs (typeof (Order)));
      Assert.That (result.BaseClass, Is.Null);
    }

    [Test]
    public void CreateClassDefinition_WithBaseClass ()
    {
      var companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Company));
      companyClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      companyClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var result = _factory.CreateClassDefinition (typeof (Customer), companyClass);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.ClassType, Is.SameAs (typeof (Customer)));
      Assert.That (result.BaseClass, Is.SameAs (companyClass));
    }

    [Test]
    public void CreatePropertyDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var propertyInfo = typeof (Order).GetProperty ("OrderItems");

      var result = _factory.CreatePropertyDefinition (classDefinition, propertyInfo);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.PropertyInfo, Is.SameAs (propertyInfo));
    }

    [Test]
    public void CreateRelationEndPointDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var propertyInfo = typeof (Order).GetProperty ("OrderItems");

      var result = _factory.CreateRelationEndPointDefinition (classDefinition, propertyInfo);

      Assert.That (result, Is.TypeOf (typeof (ReflectionBasedVirtualRelationEndPointDefinition)));
      Assert.That (((ReflectionBasedVirtualRelationEndPointDefinition) result).PropertyInfo, Is.SameAs (propertyInfo));
    }

    [Test]
    public void CreateClassDefinitionCollection ()
    {
      var result = _factory.CreateClassDefinitionCollection (new[] { typeof (Order), typeof (Company) });

      Assert.That (result.Count, Is.EqualTo (2));
      Assert.That (result.Contains (typeof (Order)), Is.True);
      Assert.That (result.Contains (typeof (Company)), Is.True);
    }
    
    [Test]
    public void CreatePropertyDefinitionCollection ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var propertyInfo1 = typeof (Order).GetProperty ("OrderNumber");
      var propertyInfo2 = typeof (Order).GetProperty ("DeliveryDate");

      var result = _factory.CreatePropertyDefinitionCollection (classDefinition, new[] { propertyInfo1, propertyInfo2 });

      Assert.That (result.Count, Is.EqualTo (2));
      Assert.That (result[0].PropertyInfo, Is.SameAs (propertyInfo1));
      Assert.That (result[1].PropertyInfo, Is.SameAs (propertyInfo2));
    }

    [Test]
    public void CreateRelationEndPointDefinitionCollection ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (OrderTicket), null);
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, typeof (OrderTicket), "Order", "OrderID", typeof (ObjectID));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      var result = _factory.CreateRelationEndPointDefinitionCollection (classDefinition);

      Assert.That (result.Count, Is.EqualTo (1));
      Assert.That (((RelationEndPointDefinition) result[0]).PropertyName, 
        Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order"));
    }
  }
}