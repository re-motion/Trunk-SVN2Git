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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class RelationEndPointDefinitionCollectionFactoryTest
  {
    private RelationEndPointDefinitionCollectionFactory _factory;
    private IMappingObjectFactory _mappingObjectFactoryMock;
    private IMappingNameResolver _mappingNameResolverMock;

    [SetUp]
    public void SetUp ()
    {
      _mappingObjectFactoryMock = MockRepository.GenerateStrictMock<IMappingObjectFactory>();
      _mappingNameResolverMock = MockRepository.GenerateStrictMock<IMappingNameResolver>();
      _factory = new RelationEndPointDefinitionCollectionFactory(_mappingObjectFactoryMock, _mappingNameResolverMock);
    }

    [Test]
    public void CreateRelationEndPointDefinitionCollection ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (OrderTicket), null);
      var propertyDefinition = PropertyDefinitionFactory.Create (classDefinition, typeof (OrderTicket), "Order", "OrderID");
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection(new[]{propertyDefinition},true));
      var expectedPropertyInfo = PropertyInfoAdapter.Create(typeof (OrderTicket).GetProperty ("Order"));
      var fakeRelationEndPoint = new RelationEndPointDefinition (propertyDefinition, false);
      
      _mappingObjectFactoryMock
          .Expect (mock => mock.CreateRelationEndPointDefinition (Arg.Is (classDefinition), Arg.Is (expectedPropertyInfo)))
          .Return (fakeRelationEndPoint);
      _mappingObjectFactoryMock.Replay();

      var result = _factory.CreateRelationEndPointDefinitionCollection (classDefinition);

      _mappingObjectFactoryMock.VerifyAllExpectations();
      _mappingNameResolverMock.VerifyAllExpectations();
      Assert.That (result.Count, Is.EqualTo (1));
      Assert.That (result[0], Is.SameAs (fakeRelationEndPoint));
    }
  }
}