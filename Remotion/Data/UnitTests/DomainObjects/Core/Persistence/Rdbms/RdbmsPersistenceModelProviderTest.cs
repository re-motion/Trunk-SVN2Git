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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
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
      base.SetUp();

      _provider = new RdbmsPersistenceModelProvider();
      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
    }

    [Test]
    public void GetEntityDefinition ()
    {
      var entityDefinition = MockRepository.GenerateStub<IRdbmsStorageEntityDefinition>();
      _classDefinition.SetStorageEntity (entityDefinition);

      var result = _provider.GetEntityDefinition (_classDefinition);

      Assert.That (result, Is.SameAs (entityDefinition));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "The Rdbms provider classes require a storage definition object of type 'IRdbmsStorageEntityDefinition' for class-definition 'Order', "
        + "but that class has no storage definition object.")]
    public void GetEntityDefinition_EmptyViewDefinition ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      Assert.That (classDefinition.StorageEntityDefinition, Is.Null);

      _provider.GetEntityDefinition (classDefinition);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "The Rdbms provider classes require a storage definition object of type 'IRdbmsStorageEntityDefinition' for class-definition 'Order', "
        + "but that class has a storage definition object of type 'FakeStorageEntityDefinition'.")]
    public void GetEntityDefinition_WrongEntityDefinition ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      classDefinition.SetStorageEntity (new FakeStorageEntityDefinition (TestDomainStorageProviderDefinition, "Test"));

      _provider.GetEntityDefinition (classDefinition);
    }

    [Test]
    public void GetStoragePropertyDefinition ()
    {
      var storagePropertyDefinition = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      var propertyDefinition = PropertyDefinitionFactory.Create (
          _classDefinition, StorageClass.Persistent, typeof (Order).GetProperty ("OrderNumber"), storagePropertyDefinition);

      var result = _provider.GetStoragePropertyDefinition (propertyDefinition);

      Assert.That (result, Is.SameAs (storagePropertyDefinition));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The Rdbms provider classes require a storage definition object of type 'IRdbmsStoragePropertyDefinition' for property 'OrderNumber' of "
        + "class-definition 'Order', but that property has no storage definition object.")]
    public void GetStoragePropertyDefinition_NoDefinition ()
    {
      var propertyDefinition = PropertyDefinitionFactory.Create (
          _classDefinition, StorageClass.Persistent, typeof (Order).GetProperty ("OrderNumber"), null);
      Assert.That (propertyDefinition.StoragePropertyDefinition, Is.Null);

      _provider.GetStoragePropertyDefinition (propertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The Rdbms provider classes require a storage definition object of type 'IRdbmsStoragePropertyDefinition' for property 'OrderNumber' of "
        + "class-definition 'Order', but that property has a storage definition object of type 'FakeStoragePropertyDefinition'.")]
    public void GetStoragePropertyDefinition_NoRdbmsDefinition ()
    {
      var storagePropertyDefinition = new FakeStoragePropertyDefinition ("Test");
      var propertyDefinition = PropertyDefinitionFactory.Create (
          _classDefinition, StorageClass.Persistent, typeof (Order).GetProperty ("OrderNumber"), storagePropertyDefinition);

      _provider.GetStoragePropertyDefinition (propertyDefinition);
    }
  }
}