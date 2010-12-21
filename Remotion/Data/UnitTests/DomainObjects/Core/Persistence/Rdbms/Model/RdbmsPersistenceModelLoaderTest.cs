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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Customer = Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer;
using File = Remotion.Data.UnitTests.DomainObjects.TestDomain.File;
using Folder = Remotion.Data.UnitTests.DomainObjects.TestDomain.Folder;
using Order = Remotion.Data.UnitTests.DomainObjects.TestDomain.Order;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class RdbmsPersistenceModelLoaderTest
  {
    private string _storageProviderID;

    private ReflectionBasedClassDefinition _baseBaseClassDefinition;
    private ReflectionBasedClassDefinition _baseClassDefinition;
    private ReflectionBasedClassDefinition _tableClassDefinition1;
    private ReflectionBasedClassDefinition _tableClassDefinition2;
    private ReflectionBasedClassDefinition _derivedClassDefinition1;
    private ReflectionBasedClassDefinition _derivedClassDefinition2;
    private ReflectionBasedClassDefinition _derivedDerivedClassDefinition;
    private ReflectionBasedClassDefinition _derivedDerivedDerivedClassDefinition;

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

    // Test Domain:
    //
    //                 BaseBase
    //                     |
    //                   Base
    //                 /      \
    //            Table1       Table2
    //                         /    \
    //                   Derived1  Derived2
    //                                |
    //                          DerivedDerived
    //                                |
    //                       DerivedDerivedDerived
    //
    // All Base classes are persisted as UnionViewDefinitions, all Tables as TableDefinitions, all Derived as FilterViewDefinitions.

    [SetUp]
    public void SetUp ()
    {
      _storageProviderID = "DefaultStorageProvider";
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition (_storageProviderID, typeof (UnitTestStorageObjectFactoryStub));

      _baseBaseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Customer), null);
      _baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Folder), _baseBaseClassDefinition);
      _tableClassDefinition1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), _baseClassDefinition);
      _tableClassDefinition2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Company), _baseClassDefinition);
      _derivedClassDefinition1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Distributor), _tableClassDefinition2);
      _derivedClassDefinition2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Partner), _tableClassDefinition2);
      _derivedDerivedClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Supplier), _derivedClassDefinition2);
      _derivedDerivedDerivedClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (File), _derivedDerivedClassDefinition);

      _baseBaseClassDefinition.SetDerivedClasses (new ClassDefinitionCollection (new[] { _baseClassDefinition }, true, true));
      _baseClassDefinition.SetDerivedClasses (new ClassDefinitionCollection (new[] { _tableClassDefinition1, _tableClassDefinition2 }, true, true));
      _tableClassDefinition2.SetDerivedClasses (
          new ClassDefinitionCollection (new[] { _derivedClassDefinition1, _derivedClassDefinition2 }, true, true));
      _derivedClassDefinition2.SetDerivedClasses (new ClassDefinitionCollection (new[] { _derivedDerivedClassDefinition }, true, true));
      _tableClassDefinition1.SetDerivedClasses (new ClassDefinitionCollection());
      _derivedClassDefinition1.SetDerivedClasses (new ClassDefinitionCollection());
      _derivedDerivedClassDefinition.SetDerivedClasses (new ClassDefinitionCollection (new[] { _derivedDerivedDerivedClassDefinition }, true, true));
      _derivedDerivedDerivedClassDefinition.SetDerivedClasses (new ClassDefinitionCollection());

      _baseBaseClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _baseClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _tableClassDefinition1.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _tableClassDefinition2.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedClassDefinition1.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedClassDefinition2.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedDerivedClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedDerivedDerivedClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());

      _fakeEntityDefinitionBaseBase = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionBase = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionTable1 = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionTable2 = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionDerived1 = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionDerived2 = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionDerivedDerived = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinitionDerivedDerivedDerived = MockRepository.GenerateStub<IEntityDefinition>();

      _entityDefinitionFactoryMock = MockRepository.GenerateStrictMock<IEntityDefinitionFactory>();
      _rdbmsPersistenceModelLoader = new RdbmsPersistenceModelLoader (
          _entityDefinitionFactoryMock, MockRepository.GenerateStub<IColumnDefinitionFactory>(), _storageProviderDefinition);
    }

    [Test]
    public void CreatePersistenceMappingValidator ()
    {
      var validator = (PersistenceMappingValidator) _rdbmsPersistenceModelLoader.CreatePersistenceMappingValidator (_baseBaseClassDefinition);

      Assert.That (validator.ValidationRules.Count, Is.EqualTo (5));
      Assert.That (validator.ValidationRules[0], Is.TypeOf (typeof (OnlyOneTablePerHierarchyValidationRule)));
      Assert.That (validator.ValidationRules[1], Is.TypeOf (typeof (TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule)));
      Assert.That (validator.ValidationRules[2], Is.TypeOf (typeof (ClassAboveTableIsAbstractValidationRule)));
      Assert.That (validator.ValidationRules[3], Is.TypeOf (typeof (ColumnNamesAreUniqueWithinInheritanceTreeValidationRule)));
      Assert.That (validator.ValidationRules[4], Is.TypeOf (typeof (PropertyTypeIsSupportedByStorageProviderValidationRule)));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_AlreadyHasPersistenceModelApplied_AndNoDerivedClassDefinitions ()
    {
      var storageEntityDefinition = new TableDefinition (_storageProviderDefinition, "Test", "TestView", new SimpleColumnDefinition[] { });
      _derivedDerivedDerivedClassDefinition.SetStorageEntity (storageEntityDefinition);

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_derivedDerivedDerivedClassDefinition);

      _entityDefinitionFactoryMock.VerifyAllExpectations();
      Assert.That (_derivedDerivedDerivedClassDefinition.StorageEntityDefinition, Is.SameAs (storageEntityDefinition));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy ()
    {
      _entityDefinitionFactoryMock
          .Expect (
              mock => mock.CreateUnionViewDefinition (
                  Arg.Is (_baseBaseClassDefinition), Arg<IEnumerable<IEntityDefinition>>.List.Equal (new[] { _fakeEntityDefinitionBase }))).Return (
                      _fakeEntityDefinitionBaseBase);
      _entityDefinitionFactoryMock
          .Expect (
              mock => mock.CreateUnionViewDefinition (
                  Arg.Is (_baseClassDefinition),
                  Arg<IEnumerable<IEntityDefinition>>.List.Equal (new[] { _fakeEntityDefinitionTable1, _fakeEntityDefinitionTable2 }))).Return (
                      _fakeEntityDefinitionBase);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateTableDefinition (Arg.Is (_tableClassDefinition1))).Return (_fakeEntityDefinitionTable1);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateTableDefinition (Arg.Is (_tableClassDefinition2))).Return (_fakeEntityDefinitionTable2);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateFilterViewDefinition (Arg.Is (_derivedClassDefinition1), Arg.Is(_fakeEntityDefinitionTable2))).Return (
              _fakeEntityDefinitionDerived1);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateFilterViewDefinition (Arg.Is (_derivedClassDefinition2), Arg.Is(_fakeEntityDefinitionTable2))).Return (
              _fakeEntityDefinitionDerived2);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateFilterViewDefinition (Arg.Is (_derivedDerivedClassDefinition), Arg.Is (_fakeEntityDefinitionDerived2))).Return (
              _fakeEntityDefinitionDerivedDerived);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateFilterViewDefinition (Arg.Is (_derivedDerivedDerivedClassDefinition), Arg.Is (_fakeEntityDefinitionDerivedDerived))).Return (
              _fakeEntityDefinitionDerivedDerivedDerived);
      
      _entityDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_baseBaseClassDefinition);

      _entityDefinitionFactoryMock.VerifyAllExpectations();
      Assert.That (_baseBaseClassDefinition.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionBaseBase));
      Assert.That (_baseClassDefinition.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionBase));
      Assert.That (_tableClassDefinition1.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionTable1));
      Assert.That (_tableClassDefinition2.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionTable2));
      Assert.That (_derivedClassDefinition1.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionDerived1));
      Assert.That (_derivedClassDefinition2.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionDerived2));
      Assert.That (_derivedDerivedClassDefinition.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionDerivedDerived));
      Assert.That (_derivedDerivedDerivedClassDefinition.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionDerivedDerivedDerived));
    }
    
    [Test]
    public void ApplyPersistenceModelToHierarchy_AbstractClassWithAbstractDerivedClassesWhichHasNoDerivations ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (AbstractClassWithoutDerivations), null);
      var derivedClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Distributor), classDefinition);
      derivedClass.SetStorageEntity (new NullEntityDefinition(_storageProviderDefinition));
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection(new[]{derivedClass}, true, true));
      derivedClass.SetDerivedClasses (new ClassDefinitionCollection());
      
      _entityDefinitionFactoryMock.Expect (
          mock => mock.CreateUnionViewDefinition (
              Arg.Is (classDefinition), Arg<IEnumerable<IEntityDefinition>>.List.Equal (Enumerable.Empty<IEntityDefinition>()))).Return (
                  _fakeEntityDefinitionBaseBase);
      _entityDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (classDefinition);

      _entityDefinitionFactoryMock.VerifyAllExpectations();
      Assert.That (classDefinition.StorageEntityDefinition, Is.SameAs(_fakeEntityDefinitionBaseBase));
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException))]
    public void StorageEntityDefinitionDoesNotImplementIEntityDefinition ()
    {
      var invalidStorageEntityDefinition = MockRepository.GenerateStub<IStorageEntityDefinition>();
      _derivedClassDefinition2.SetStorageEntity (invalidStorageEntityDefinition);

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_derivedDerivedClassDefinition);
    }
  }
}