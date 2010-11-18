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
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class PersistenceModelLoaderTest
  {
    private ReflectionBasedClassDefinition _orderClassDefinition;
    private IStoragePropertyDefinitionFactory _columnDefinitionFactoryMock;
    private PersistenceModelLoader _persistenceModelLoader;
    private ColumnDefinition _fakeColumnDefinition1;
    private ColumnDefinition _fakeColumnDefinition2;
    private ReflectionBasedPropertyDefinition _propertyDefinition1;
    private ReflectionBasedPropertyDefinition _propertyDefinition2;
    private ReflectionBasedClassDefinition _companyClassDefinition;
    private ReflectionBasedClassDefinition _partnerClassDefinition;
    private ReflectionBasedPropertyDefinition _propertyDefinition3;
    private ReflectionBasedPropertyDefinition _propertyDefinition4;
    private ReflectionBasedClassDefinition _abstractClassDefinition;

    [SetUp]
    public void SetUp ()
    {
      _orderClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageDefinition (typeof (Order), null);
      _companyClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageDefinition (typeof (Company), null);
      _partnerClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageDefinition (typeof (Partner), _companyClassDefinition);
      _abstractClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageDefinition (typeof (AbstractClass), null);
      _columnDefinitionFactoryMock = MockRepository.GenerateStrictMock<IStoragePropertyDefinitionFactory>();
      _persistenceModelLoader = new PersistenceModelLoader (_columnDefinitionFactoryMock);
      _propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.Create (
          _orderClassDefinition, StorageClass.Persistent, typeof (Order).GetProperty ("OrderNumber"), "OrderNo");
      _propertyDefinition2 = ReflectionBasedPropertyDefinitionFactory.Create (
          _orderClassDefinition, StorageClass.Persistent, typeof (Order).GetProperty ("DeliveryDate"), "DeliveryDate");
      _propertyDefinition3 = ReflectionBasedPropertyDefinitionFactory.Create (
         _companyClassDefinition, StorageClass.Persistent, typeof (Company).GetProperty ("Name"), "Name");
      _propertyDefinition4 = ReflectionBasedPropertyDefinitionFactory.Create (
         _partnerClassDefinition, StorageClass.Persistent, typeof (Company).GetProperty ("Name"), "Name");
      _fakeColumnDefinition1 = new ColumnDefinition ("Test1", typeof (string), true);
      _fakeColumnDefinition2 = new ColumnDefinition ("Test2", typeof (int), false);
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_HasAlreadyPersistenceModelApplied ()
    {
      var storageEntityDefinition = new TableDefinition ("Test", new ColumnDefinition[] { });
      _orderClassDefinition.SetStorageEntity (storageEntityDefinition);

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_orderClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations ();
      Assert.That (_orderClassDefinition.StorageEntityDefinition, Is.SameAs (storageEntityDefinition));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_ClassDefinitionHasDBTableAttributeApplied_And_HasNoPropertyDefinitions ()
    {
      Assert.That (_orderClassDefinition.StorageEntityDefinition, Is.Null);
      Assert.That (_orderClassDefinition.MyPropertyDefinitions.Count, Is.EqualTo (0));

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_orderClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations ();
      Assert.That (_orderClassDefinition.StorageEntityDefinition, Is.TypeOf(typeof(TableDefinition)));
      Assert.That (((TableDefinition) _orderClassDefinition.StorageEntityDefinition).TableName, Is.EqualTo("Order"));
      Assert.That (((TableDefinition) _orderClassDefinition.StorageEntityDefinition).GetColumns().Count, Is.EqualTo (0));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_ClassDefinitionHasDBTableAttributeApplied_And_HasPropertyDefinitions ()
    {
      Assert.That (_orderClassDefinition.StorageEntityDefinition, Is.Null);
      _orderClassDefinition.MyPropertyDefinitions.Add (_propertyDefinition1);
      _orderClassDefinition.MyPropertyDefinitions.Add (_propertyDefinition2);

      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_propertyDefinition1)).Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_propertyDefinition2)).Return (_fakeColumnDefinition2);
      
      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_orderClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations ();
      Assert.That (_orderClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (((TableDefinition) _orderClassDefinition.StorageEntityDefinition).TableName, Is.EqualTo ("Order"));
      Assert.That (((TableDefinition) _orderClassDefinition.StorageEntityDefinition).GetColumns ().Count, Is.EqualTo (2));
      Assert.That (((TableDefinition) _orderClassDefinition.StorageEntityDefinition).GetColumns ()[0], Is.SameAs(_fakeColumnDefinition1));
      Assert.That (((TableDefinition) _orderClassDefinition.StorageEntityDefinition).GetColumns ()[1], Is.SameAs (_fakeColumnDefinition2));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_BaseClassDefinitionHasDBTableAttributeApplied_And_HasNoPropertyDefinitions ()
    {
      Assert.That (_partnerClassDefinition.StorageEntityDefinition, Is.Null);
      Assert.That (_companyClassDefinition.StorageEntityDefinition, Is.Null);
      Assert.That (_partnerClassDefinition.MyPropertyDefinitions.Count, Is.EqualTo (0));
      Assert.That (_companyClassDefinition.MyPropertyDefinitions.Count, Is.EqualTo (0));
      _partnerClassDefinition.SetReadOnly();
      _companyClassDefinition.SetReadOnly();

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_partnerClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations ();

      Assert.That (_companyClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (((TableDefinition) _companyClassDefinition.StorageEntityDefinition).TableName, Is.EqualTo ("Company"));
      Assert.That (((TableDefinition) _companyClassDefinition.StorageEntityDefinition).GetColumns().Count, Is.EqualTo (0));

      Assert.That (_partnerClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (FilterViewDefinition)));
      Assert.That (((FilterViewDefinition) _partnerClassDefinition.StorageEntityDefinition).ViewName, Is.EqualTo ("PartnerView"));
      Assert.That (((FilterViewDefinition) _partnerClassDefinition.StorageEntityDefinition).ClassID, Is.EqualTo("Partner"));
      Assert.That (((FilterViewDefinition) _partnerClassDefinition.StorageEntityDefinition).BaseEntity, Is.SameAs (_companyClassDefinition.StorageEntityDefinition));
      Assert.That (((FilterViewDefinition) _partnerClassDefinition.StorageEntityDefinition).GetColumns ().Count, Is.EqualTo (0));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_BaseClassDefinitionHasDBTableAttributeApplied_And_HasPropertyDefinitions ()
    {
      Assert.That (_partnerClassDefinition.StorageEntityDefinition, Is.Null);
      Assert.That (_companyClassDefinition.StorageEntityDefinition, Is.Null);
      Assert.That (_partnerClassDefinition.MyPropertyDefinitions.Count, Is.EqualTo (0));
      Assert.That (_companyClassDefinition.MyPropertyDefinitions.Count, Is.EqualTo (0));
      _companyClassDefinition.MyPropertyDefinitions.Add (_propertyDefinition3);
      _partnerClassDefinition.SetReadOnly ();
      _companyClassDefinition.SetReadOnly ();

      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_propertyDefinition3)).Return (_fakeColumnDefinition1);
      
      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_partnerClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations ();

      Assert.That (_companyClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (((TableDefinition) _companyClassDefinition.StorageEntityDefinition).TableName, Is.EqualTo ("Company"));
      Assert.That (((TableDefinition) _companyClassDefinition.StorageEntityDefinition).GetColumns ().Count, Is.EqualTo (1));
      Assert.That (((TableDefinition) _companyClassDefinition.StorageEntityDefinition).GetColumns ()[0], Is.SameAs(_fakeColumnDefinition1));

      Assert.That (_partnerClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (FilterViewDefinition)));
      Assert.That (((FilterViewDefinition) _partnerClassDefinition.StorageEntityDefinition).ViewName, Is.EqualTo ("PartnerView"));
      Assert.That (((FilterViewDefinition) _partnerClassDefinition.StorageEntityDefinition).ClassID, Is.EqualTo ("Partner"));
      Assert.That (((FilterViewDefinition) _partnerClassDefinition.StorageEntityDefinition).BaseEntity, Is.SameAs (_companyClassDefinition.StorageEntityDefinition));
      Assert.That (((FilterViewDefinition) _partnerClassDefinition.StorageEntityDefinition).GetColumns ().Count, Is.EqualTo (1));
      Assert.That (((FilterViewDefinition) _partnerClassDefinition.StorageEntityDefinition).GetColumns ()[0], Is.SameAs(_fakeColumnDefinition1));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_BaseClassDefinitionHasDBTableAttributeApplied_And_HasPropertyDefinitions_PropertyIsFiltered ()
    {
      Assert.That (_partnerClassDefinition.StorageEntityDefinition, Is.Null);
      Assert.That (_companyClassDefinition.StorageEntityDefinition, Is.Null);
      Assert.That (_partnerClassDefinition.MyPropertyDefinitions.Count, Is.EqualTo (0));
      Assert.That (_companyClassDefinition.MyPropertyDefinitions.Count, Is.EqualTo (0));
      _partnerClassDefinition.MyPropertyDefinitions.Add (_propertyDefinition4);
      _partnerClassDefinition.SetReadOnly ();
      _companyClassDefinition.SetReadOnly ();

      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_propertyDefinition4)).Return (_fakeColumnDefinition1);

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_partnerClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations ();

      Assert.That (_companyClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (((TableDefinition) _companyClassDefinition.StorageEntityDefinition).TableName, Is.EqualTo ("Company"));
      Assert.That (((TableDefinition) _companyClassDefinition.StorageEntityDefinition).GetColumns ().Count, Is.EqualTo (0));

      Assert.That (_partnerClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (FilterViewDefinition)));
      Assert.That (((FilterViewDefinition) _partnerClassDefinition.StorageEntityDefinition).ViewName, Is.EqualTo ("PartnerView"));
      Assert.That (((FilterViewDefinition) _partnerClassDefinition.StorageEntityDefinition).ClassID, Is.EqualTo ("Partner"));
      Assert.That (((FilterViewDefinition) _partnerClassDefinition.StorageEntityDefinition).BaseEntity, Is.SameAs (_companyClassDefinition.StorageEntityDefinition));
      Assert.That (((FilterViewDefinition) _partnerClassDefinition.StorageEntityDefinition).GetColumns ().Count, Is.EqualTo (0));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_ActualAndBaseClassDefinitionHaveNoDBTableAttributeApplied_And_NoDerivedClasses()
    {
      Assert.That (_abstractClassDefinition.StorageEntityDefinition, Is.Null);
      Assert.That (_abstractClassDefinition.MyPropertyDefinitions.Count, Is.EqualTo (0));
      
      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_abstractClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations ();

      Assert.That (_abstractClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (UnionViewDefinition)));
      Assert.That (((UnionViewDefinition) _abstractClassDefinition.StorageEntityDefinition).ViewName, Is.EqualTo("AbstractClassView"));
      Assert.That (((UnionViewDefinition) _abstractClassDefinition.StorageEntityDefinition).UnionedEntities.Count, Is.EqualTo (0));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_ActualAndBaseClassDefinitionHaveNoDBTableAttributeApplied_And_WithDerivedClassesn ()
    {
      Assert.That (_abstractClassDefinition.StorageEntityDefinition, Is.Null);
      Assert.That (_abstractClassDefinition.MyPropertyDefinitions.Count, Is.EqualTo (0));
      PrivateInvoke.SetNonPublicField (
          _abstractClassDefinition,
          "_derivedClasses",
          new ClassDefinitionCollection (new ClassDefinition[] { _orderClassDefinition, _partnerClassDefinition }, true, true));
      _abstractClassDefinition.SetReadOnly();
      _partnerClassDefinition.SetReadOnly();
      
      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_abstractClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations ();

      Assert.That (_abstractClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (UnionViewDefinition)));
      Assert.That (((UnionViewDefinition) _abstractClassDefinition.StorageEntityDefinition).ViewName, Is.EqualTo ("AbstractClassView"));
      Assert.That (((UnionViewDefinition) _abstractClassDefinition.StorageEntityDefinition).UnionedEntities.Count, Is.EqualTo (2));
      Assert.That (((UnionViewDefinition) _abstractClassDefinition.StorageEntityDefinition).UnionedEntities[0], Is.SameAs(_orderClassDefinition.StorageEntityDefinition));
      Assert.That (((UnionViewDefinition) _abstractClassDefinition.StorageEntityDefinition).UnionedEntities[1], Is.SameAs(_partnerClassDefinition.StorageEntityDefinition));

      Assert.That (_orderClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (_partnerClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (FilterViewDefinition)));
      Assert.That (_orderClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
    }

  }
}