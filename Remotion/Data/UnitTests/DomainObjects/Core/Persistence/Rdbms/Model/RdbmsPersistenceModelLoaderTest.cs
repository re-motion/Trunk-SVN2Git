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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class RdbmsPersistenceModelLoaderTest
  {
    private string _storageProviderID;
    private RdbmsPersistenceModelLoader _rdbmsPersistenceModelLoader;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private IEntityDefinitionFactory _entityDefinitionFactoryMock;

    private IEntityDefinition _fakeEntityDefinitionBaseBase;
    private IEntityDefinition _fakeEntityDefinitionBase;
    private IEntityDefinition _fakeEntityDefinitionTable1;
    private IEntityDefinition _fakeEntityDefinitionTable2;
    private IEntityDefinition _fakeEntityDefinitionDerived1;
    private IEntityDefinition _fakeEntityDefinitionDerived2;
    private IEntityDefinition _fakeEntityDefinitionDerivedDerived;
    private IEntityDefinition _fakeEntityDefinitionDerivedDerivedDerived;

    private SimpleColumnDefinition _fakeColumnDefinition1;
    private SimpleColumnDefinition _fakeColumnDefinition2;
    private SimpleColumnDefinition _fakeColumnDefinition3;
    private SimpleColumnDefinition _fakeColumnDefinition4;
    private SimpleColumnDefinition _fakeColumnDefinition5;
    private SimpleColumnDefinition _fakeColumnDefinition6;
    private SimpleColumnDefinition _fakeColumnDefinition7;
    private IColumnDefinitionFactory _columnDefinitionFactoryMock;
    private RdbmsPersistenceModelLoaderTestHelper _testModel;
    private IStorageNameProvider _storageNameProviderStub;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderID = "DefaultStorageProvider";
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition (_storageProviderID);
      _testModel = new RdbmsPersistenceModelLoaderTestHelper();

      _fakeEntityDefinitionBaseBase = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionBase = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionTable1 = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionTable2 = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionDerived1 = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionDerived2 = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionDerivedDerived = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionDerivedDerivedDerived = MockRepository.GenerateStub<IEntityDefinition>();

      _fakeColumnDefinition1 = new SimpleColumnDefinition ("Test1", typeof (string), "varchar", true, false);
      _fakeColumnDefinition2 = new SimpleColumnDefinition ("Test2", typeof (int), "int", false, false);
      _fakeColumnDefinition3 = new SimpleColumnDefinition ("Test3", typeof (string), "varchar", true, false);
      _fakeColumnDefinition4 = new SimpleColumnDefinition ("Test4", typeof (int), "int", false, false);
      _fakeColumnDefinition5 = new SimpleColumnDefinition ("Test5", typeof (string), "varchar", true, false);
      _fakeColumnDefinition6 = new SimpleColumnDefinition ("Test6", typeof (int), "int", false, false);
      _fakeColumnDefinition7 = new SimpleColumnDefinition ("Test7", typeof (string), "varchar", true, false);

      _entityDefinitionFactoryMock = MockRepository.GenerateStrictMock<IEntityDefinitionFactory>();
      _columnDefinitionFactoryMock = MockRepository.GenerateStrictMock<IColumnDefinitionFactory>();
      _storageNameProviderStub = MockRepository.GenerateStub<IStorageNameProvider>();
      _storageNameProviderStub.Stub (stub => stub.GetTableName (_testModel.TableClassDefinition1)).Return (_testModel.TableClassDefinition1.ID);
      _storageNameProviderStub.Stub (stub => stub.GetTableName (_testModel.TableClassDefinition2)).Return (_testModel.TableClassDefinition2.ID);
      _rdbmsPersistenceModelLoader = new RdbmsPersistenceModelLoader (
          _entityDefinitionFactoryMock, _columnDefinitionFactoryMock, _storageProviderDefinition, _storageNameProviderStub);
    }

    [Test]
    public void CreatePersistenceMappingValidator ()
    {
      var validator =
          (PersistenceMappingValidator) _rdbmsPersistenceModelLoader.CreatePersistenceMappingValidator (_testModel.BaseBaseClassDefinition);

      Assert.That (validator.ValidationRules.Count, Is.EqualTo (5));
      Assert.That (validator.ValidationRules[0], Is.TypeOf (typeof (OnlyOneTablePerHierarchyValidationRule)));
      Assert.That (validator.ValidationRules[1], Is.TypeOf (typeof (TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule)));
      Assert.That (validator.ValidationRules[2], Is.TypeOf (typeof (ClassAboveTableIsAbstractValidationRule)));
      Assert.That (validator.ValidationRules[3], Is.TypeOf (typeof (ColumnNamesAreUniqueWithinInheritanceTreeValidationRule)));
      Assert.That (validator.ValidationRules[4], Is.TypeOf (typeof (PropertyTypeIsSupportedByStorageProviderValidationRule)));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_CreatesEntities_AndProperties_ForClassAndDerivedClasses ()
    {
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_testModel.BaseBasePropertyDefinition))
          .Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_testModel.BasePropertyDefinition))
          .Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_testModel.TablePropertyDefinition1))
          .Return (_fakeColumnDefinition3);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_testModel.TablePropertyDefinition2))
          .Return (_fakeColumnDefinition4);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_testModel.DerivedPropertyDefinition1))
          .Return (_fakeColumnDefinition5);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_testModel.DerivedPropertyDefinition2))
          .Return (_fakeColumnDefinition6);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_testModel.DerivedDerivedPropertyDefinition))
          .Return (_fakeColumnDefinition7);
      _columnDefinitionFactoryMock.Replay();

      _entityDefinitionFactoryMock
          .Expect (
              mock => mock.CreateUnionViewDefinition (
                  Arg.Is (_testModel.BaseBaseClassDefinition),
                  Arg<IEnumerable<IEntityDefinition>>.List.Equal (new[] { _fakeEntityDefinitionBase })))
          .Return (_fakeEntityDefinitionBaseBase);
      _entityDefinitionFactoryMock
          .Expect (
              mock => mock.CreateUnionViewDefinition (
                  Arg.Is (_testModel.BaseClassDefinition),
                  Arg<IEnumerable<IEntityDefinition>>.List.Equal (new[] { _fakeEntityDefinitionTable1, _fakeEntityDefinitionTable2 })))
          .Return (_fakeEntityDefinitionBase);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateTableDefinition (_testModel.TableClassDefinition1))
          .Return (_fakeEntityDefinitionTable1);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateTableDefinition (_testModel.TableClassDefinition2))
          .Return (_fakeEntityDefinitionTable2);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateFilterViewDefinition (_testModel.DerivedClassDefinition1, _fakeEntityDefinitionTable2))
          .Return (_fakeEntityDefinitionDerived1);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateFilterViewDefinition (_testModel.DerivedClassDefinition2, _fakeEntityDefinitionTable2))
          .Return (_fakeEntityDefinitionDerived2);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateFilterViewDefinition (_testModel.DerivedDerivedClassDefinition, _fakeEntityDefinitionDerived2))
          .Return (_fakeEntityDefinitionDerivedDerived);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateFilterViewDefinition (_testModel.DerivedDerivedDerivedClassDefinition, _fakeEntityDefinitionDerivedDerived))
          .Return (_fakeEntityDefinitionDerivedDerivedDerived);
      _entityDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.BaseBaseClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      _entityDefinitionFactoryMock.VerifyAllExpectations();

      Assert.That (_testModel.BaseBaseClassDefinition.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionBaseBase));
      Assert.That (_testModel.BaseClassDefinition.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionBase));
      Assert.That (_testModel.TableClassDefinition1.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionTable1));
      Assert.That (_testModel.TableClassDefinition2.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionTable2));
      Assert.That (_testModel.DerivedClassDefinition1.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionDerived1));
      Assert.That (_testModel.DerivedClassDefinition2.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionDerived2));
      Assert.That (_testModel.DerivedDerivedClassDefinition.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionDerivedDerived));
      Assert.That (_testModel.DerivedDerivedDerivedClassDefinition.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionDerivedDerivedDerived));

      Assert.That (_testModel.BaseBasePropertyDefinition.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition1));
      Assert.That (_testModel.BasePropertyDefinition.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition2));
      Assert.That (_testModel.TablePropertyDefinition1.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition3));
      Assert.That (_testModel.TablePropertyDefinition2.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition4));
      Assert.That (_testModel.DerivedPropertyDefinition1.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition5));
      Assert.That (_testModel.DerivedPropertyDefinition2.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition6));
      Assert.That (_testModel.DerivedDerivedPropertyDefinition.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition7));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_LeavesExistingEntities_AndProperties ()
    {
      _testModel.TableClassDefinition1.SetStorageEntity (_fakeEntityDefinitionTable1);
      _testModel.TablePropertyDefinition1.SetStorageProperty (_fakeColumnDefinition1);

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.TableClassDefinition1);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      _entityDefinitionFactoryMock.VerifyAllExpectations();

      Assert.That (_testModel.TableClassDefinition1.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionTable1));
      Assert.That (_testModel.TablePropertyDefinition1.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition1));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_CreatesNullEntity_ForAbstractClass_WithAbstractDerivedClass_WithoutConcreteDerivations ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (
          typeof (AbstractClassWithoutDerivations),
          null);
      var derivedClass = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Distributor), classDefinition);

      derivedClass.SetStorageEntity (new NullEntityDefinition (_storageProviderDefinition));
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection (new[] { derivedClass }, true, true));
      derivedClass.SetDerivedClasses (new ClassDefinitionCollection());
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      derivedClass.SetPropertyDefinitions (new PropertyDefinitionCollection());

      _entityDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (classDefinition);

      _entityDefinitionFactoryMock.VerifyAllExpectations();
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf (typeof (NullEntityDefinition)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The storage entity definition of class 'Derived2Class' does not implement interface 'IEntityDefinition'.")]
    public void ApplyPersistenceModelToHierarchy_Throws_WhenExistingEntityDefinitionDoesNotImplementIEntityDefinition ()
    {
      var invalidStorageEntityDefinition = MockRepository.GenerateStub<IStorageEntityDefinition>();
      _testModel.DerivedClassDefinition2.SetStorageEntity (invalidStorageEntityDefinition);
      _testModel.DerivedDerivedPropertyDefinition.SetStorageProperty (_fakeColumnDefinition7);

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.DerivedDerivedClassDefinition);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The property definition 'DerivedDerivedProperty' of class 'DerivedDerivedClass' does not implement interface 'IColumnDefinition'.")]
    public void ApplyPersistenceModelToHierarchy_Throws_WhenExistingPropertyDefinitionDoesNotImplementIColumnDefinition ()
    {
      _testModel.DerivedClassDefinition2.SetStorageEntity (_fakeEntityDefinitionDerived2);
      _testModel.DerivedDerivedPropertyDefinition.SetStorageProperty (new FakeStoragePropertyDefinition ("Fake"));

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.DerivedDerivedClassDefinition);
    }
  }
}