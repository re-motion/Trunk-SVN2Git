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
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class PersistenceModelLoaderTest
  {
    private IStorageProviderDefinitionFinder _storageProviderDefinitionStub;
    private PersistenceModelLoader _persistenceModelLoader;
    private ReflectionBasedClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinitionStub = MockRepository.GenerateStub<IStorageProviderDefinitionFinder> ();
      _persistenceModelLoader = new PersistenceModelLoader (_storageProviderDefinitionStub);
      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), null);
      
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy ()
    {
      _classDefinition.SetDerivedClasses (new ClassDefinitionCollection());
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      Assert.That (_classDefinition.StorageEntityDefinition, Is.Null);

      _storageProviderDefinitionStub.Stub (stub => stub.GetStorageProviderDefinition (_classDefinition)).Return (
          new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider", typeof (UnitTestStorageObjectFactoryStub)));

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_classDefinition);

      Assert.That (_classDefinition.StorageEntityDefinition, Is.Not.Null);
    }

    [Test]
    public void CreatePersistenceMappingValidator ()
    {
      _storageProviderDefinitionStub.Stub (stub => stub.GetStorageProviderDefinition (_classDefinition)).Return (
          new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider", typeof (UnitTestStorageObjectFactoryStub)));

      var result = _persistenceModelLoader.CreatePersistenceMappingValidator (_classDefinition);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.TypeOf (typeof (PersistenceMappingValidator)));
    }
  }
}