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
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class RdbmsPersistenceModelProviderTest : StandardMappingTest
  {
    private RdbmsPersistenceModelProvider _provider;
    private ClassDefinition _classDefinition;

    public override void SetUp ()
    {
      _provider = new RdbmsPersistenceModelProvider();

      _classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
    }

    [Test]
    public void GetEntityDefinition ()
    {
      var entityDefinition = MockRepository.GenerateStub<IEntityDefinition>();
      _classDefinition.SetStorageEntity (entityDefinition);

      var result = _provider.GetEntityDefinition (_classDefinition);

      Assert.That (result, Is.SameAs (entityDefinition));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The RdbmsProvider expected a storage definition object of type 'IEntityDefinition' for class-definition 'Order', "
        + "but found a storage definition object of type 'IStorageEntityDefinition*.", MatchType = MessageMatch.Regex)]
    public void GetEntityDefinition_NoIEntityDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      classDefinition.SetStorageEntity (MockRepository.GenerateStub<IStorageEntityDefinition>());

      _provider.GetEntityDefinition (classDefinition);
    }

    [Test]
    public void GetColumnDefinition ()
    {
      var columnDefinition = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      var propertyDefinition = PropertyDefinitionFactory.Create (
          _classDefinition, StorageClass.Persistent, typeof (Order).GetProperty ("OrderNumber"), columnDefinition);

      var result = _provider.GetColumnDefinition (propertyDefinition);

      Assert.That (result, Is.SameAs (columnDefinition));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The RdbmsProvider expected a storage definition object of type 'IRdbmsStoragePropertyDefinition' for property 'OrderNumber' of class-definition 'Order', "
        + "but found a storage definition object of type 'IStoragePropertyDefinition*.", MatchType = MessageMatch.Regex)]
    public void GetColumnDefinition_NoIColumnDefinition ()
    {
      var columnDefinition = MockRepository.GenerateStub<IStoragePropertyDefinition>();
      var propertyDefinition = PropertyDefinitionFactory.Create (
          _classDefinition, StorageClass.Persistent, typeof (Order).GetProperty ("OrderNumber"), columnDefinition);

      _provider.GetColumnDefinition (propertyDefinition);
    }

    [Test]
    public void GetIDColumnDefinition ()
    {
      var columnDefinition = new ObjectIDStoragePropertyDefinition (
          ColumnDefinitionObjectMother.ObjectIDColumn, ColumnDefinitionObjectMother.ClassIDColumn);
      var propertyDefinition = PropertyDefinitionFactory.Create (
          _classDefinition, "OrderNo", true, true, null, StorageClass.Persistent, typeof (Order).GetProperty ("OrderNumber"), columnDefinition);
      var relationEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);

      var result = _provider.GetIDColumnDefinition (relationEndPointDefinition);

      Assert.That (result, Is.SameAs (columnDefinition));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The RdbmsProvider expected a storage definition object of type 'ObjectIDStoragePropertyDefinition' for property 'OrderNo' of class-definition 'Order', "
        + "but found a storage definition object of type 'IStoragePropertyDefinition*.", MatchType = MessageMatch.Regex)]
    public void GetIDColumnDefinition_NoIDColumnDefinition ()
    {
      var columnDefinition = MockRepository.GenerateStub<IStoragePropertyDefinition>();
      var propertyDefinition = PropertyDefinitionFactory.Create (
          _classDefinition, "OrderNo", true, true, null, StorageClass.Persistent, typeof (Order).GetProperty ("OrderNumber"), columnDefinition);
      var relationEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);

      _provider.GetIDColumnDefinition (relationEndPointDefinition);
    }
  }
}