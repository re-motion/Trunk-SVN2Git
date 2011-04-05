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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class RelationDefinitionCollectionFactoryTest : StandardMappingTest
  {
    private RelationDefinitionCollectionFactory _factory;
    private IMappingObjectFactory _mappingObjectFactoryMock;
    private ClassDefinition _orderClassDefinition;
    private ClassDefinition _orderItemClassDefinition;
    private RelationDefinition _fakeRelationDefinition1;
    private RelationDefinition _fakeRelationDefinition2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      
      _mappingObjectFactoryMock = MockRepository.GenerateStrictMock<IMappingObjectFactory>();
      _factory = new RelationDefinitionCollectionFactory(_mappingObjectFactoryMock);
      _orderClassDefinition = MappingConfiguration.Current.ClassDefinitions["Order"];
      _orderItemClassDefinition = MappingConfiguration.Current.ClassDefinitions["OrderItem"];
      _fakeRelationDefinition1 = new RelationDefinition (
          "Fake1",
          new AnonymousRelationEndPointDefinition (_orderClassDefinition),
          new AnonymousRelationEndPointDefinition (_orderItemClassDefinition));
      _fakeRelationDefinition2 = new RelationDefinition (
          "Fake2",
          new AnonymousRelationEndPointDefinition (_orderItemClassDefinition),
          new AnonymousRelationEndPointDefinition (_orderClassDefinition));
    }

    [Test]
    public void CreateRelationDefinitionCollection_OneClassDefinitionWithOneEndPoint ()
    {
      var classDefinitions = new ClassDefinitionCollection (new[] { _orderItemClassDefinition }, true, true);

      _mappingObjectFactoryMock
          .Expect (
              mock =>
              mock.CreateRelationDefinition (
                  classDefinitions,
                  _orderItemClassDefinition,
                  _orderItemClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"].
                      PropertyInfo))
          .Return (_fakeRelationDefinition1);
      _mappingObjectFactoryMock.Replay();

      
      var result = _factory.CreateRelationDefinitionCollection (classDefinitions);

      _mappingObjectFactoryMock.VerifyAllExpectations();
      Assert.That (result.Count, Is.EqualTo (1));
      Assert.That (result[0], Is.SameAs (_fakeRelationDefinition1));
    }

    [Test]
    public void CreateRelationDefinitionCollection_OneClassDefinitionWithSeveralEndPoints_DuplicatedRelationDefinitionsGetFiltered ()
    {
      var classDefinitions = new ClassDefinitionCollection (new[] { _orderClassDefinition }, true, true);

      _mappingObjectFactoryMock
          .Expect (
              mock =>
              mock.CreateRelationDefinition (
                  classDefinitions,
                  _orderClassDefinition,
                  _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"].
                      PropertyInfo))
          .Return (_fakeRelationDefinition1);
      _mappingObjectFactoryMock
          .Expect (
              mock =>
              mock.CreateRelationDefinition (
                  classDefinitions,
                  _orderClassDefinition,
                  _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].
                      PropertyInfo))
          .Return (_fakeRelationDefinition1);
      _mappingObjectFactoryMock
          .Expect (
              mock =>
              mock.CreateRelationDefinition (
                  classDefinitions,
                  _orderClassDefinition,
                  _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].
                      PropertyInfo))
          .Return (_fakeRelationDefinition2);
      _mappingObjectFactoryMock
          .Expect (
              mock =>
              mock.CreateRelationDefinition (
                  classDefinitions,
                  _orderClassDefinition,
                  _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].
                      PropertyInfo))
          .Return (_fakeRelationDefinition2);
      _mappingObjectFactoryMock.Replay ();
      
      var result = _factory.CreateRelationDefinitionCollection (classDefinitions);

      _mappingObjectFactoryMock.VerifyAllExpectations ();
      Assert.That (result.Count, Is.EqualTo (2));
      Assert.That (result[0], Is.SameAs (_fakeRelationDefinition1));
      Assert.That (result[1], Is.SameAs (_fakeRelationDefinition2));
    }

    [Test]
    public void CreateRelationDefinitionCollection_SeveralClassDefinitionWithSeveralEndPoints_DuplicatedRelationDefinitionsGetFiltered ()
    {
      var classDefinitions = new ClassDefinitionCollection (new[] { _orderClassDefinition, _orderItemClassDefinition }, true, true);

      _mappingObjectFactoryMock
          .Expect (
              mock =>
              mock.CreateRelationDefinition (
                  classDefinitions,
                  _orderClassDefinition,
                  _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"].
                      PropertyInfo))
          .Return (_fakeRelationDefinition1);
      _mappingObjectFactoryMock
          .Expect (
              mock =>
              mock.CreateRelationDefinition (
                  classDefinitions,
                  _orderClassDefinition,
                  _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].
                      PropertyInfo))
          .Return (_fakeRelationDefinition1);
      _mappingObjectFactoryMock
          .Expect (
              mock =>
              mock.CreateRelationDefinition (
                  classDefinitions,
                  _orderClassDefinition,
                  _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].
                      PropertyInfo))
          .Return (_fakeRelationDefinition2);
      _mappingObjectFactoryMock
          .Expect (
              mock =>
              mock.CreateRelationDefinition (
                  classDefinitions,
                  _orderClassDefinition,
                  _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].
                      PropertyInfo))
          .Return (_fakeRelationDefinition2);
      _mappingObjectFactoryMock
          .Expect (
              mock =>
              mock.CreateRelationDefinition (
                  classDefinitions,
                  _orderItemClassDefinition,
                  _orderItemClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"].
                      PropertyInfo))
          .Return (_fakeRelationDefinition1);
      _mappingObjectFactoryMock.Replay ();

      var result = _factory.CreateRelationDefinitionCollection (classDefinitions);

      _mappingObjectFactoryMock.VerifyAllExpectations ();
      Assert.That (result.Count, Is.EqualTo (2));
      Assert.That (result[0], Is.SameAs (_fakeRelationDefinition1));
      Assert.That (result[1], Is.SameAs (_fakeRelationDefinition2));
    }

  }
}