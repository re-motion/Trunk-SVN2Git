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
using System.IO;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting.Resources;
using Rhino.Mocks;
using File = System.IO.File;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  //TODO: Derive ClassWithAllDataTypes from an abstract class to ensure that all data types are selected in a UNION
  [TestFixture]
  public class FileBuilderTest : SchemaGenerationTestBase
  {
    private FileBuilder _fileBuilderForFirstStorageProvider;
    private FileBuilder _fileBuilderForSecondStorageProvider;
    private string _firstStorageProviderSetupDBScript;
    private string _secondStorageProviderSetupDBScript;
    private string _firstStorageProviderSetupDBScriptWithoutTables;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      if (Directory.Exists ("TestDirectory"))
        Directory.Delete ("TestDirectory", true);
    }

    public override void SetUp ()
    {
      base.SetUp();

      _fileBuilderForFirstStorageProvider = new FileBuilder (SchemaGenerationFirstStorageProviderDefinition);
      _fileBuilderForSecondStorageProvider = new FileBuilder (SchemaGenerationSecondStorageProviderDefinition);
      _firstStorageProviderSetupDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.SetupDB_FirstStorageProvider.sql");
      _firstStorageProviderSetupDBScriptWithoutTables = ResourceUtility.GetResourceString (GetType(), "TestData.SetupDB_FirstStorageProviderWithoutTables.sql");
      _secondStorageProviderSetupDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.SetupDB_SecondStorageProvider.sql");
    }

    public override void TearDown ()
    {
      base.TearDown();

      if (Directory.Exists ("TestDirectory"))
        Directory.Delete ("TestDirectory", true);
    }

    [Test]
    public void GetDatabaseName ()
    {
      Assert.AreEqual ("SchemaGenerationTestDomain1", _fileBuilderForFirstStorageProvider.GetDatabaseName());
      Assert.AreEqual ("SchemaGenerationTestDomain2", _fileBuilderForSecondStorageProvider.GetDatabaseName ());
    }

    // TODO Review 3856: Move to FileBuilderIntegrationTest
    [Test]
    public void GetScriptForFirstStorageProvider ()
    {
      Assert.AreEqual (
          _firstStorageProviderSetupDBScript,
          _fileBuilderForFirstStorageProvider.GetScript (
              FileBuilder.GetClassesInStorageProvider (MappingConfiguration.ClassDefinitions, SchemaGenerationFirstStorageProviderDefinition)));
    }

    // TODO Review 3856: Move to FileBuilderIntegrationTest
    [Test]
    public void GetScriptForSecondStorageProvider ()
    {
      Assert.AreEqual (
          _secondStorageProviderSetupDBScript,
          _fileBuilderForSecondStorageProvider.GetScript (
              FileBuilder.GetClassesInStorageProvider (MappingConfiguration.ClassDefinitions, SchemaGenerationSecondStorageProviderDefinition)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Class 'Official' has storage provider 'SchemaGenerationSecondStorageProvider' defined, but storage provider 'SchemaGenerationFirstStorageProvider' is required."
        )]
    public void GetScript_WithWrongStorageProvider ()
    {
      Assert.AreEqual (
          _firstStorageProviderSetupDBScript,
          _fileBuilderForFirstStorageProvider.GetScript (
                  new ClassDefinitionCollection (new[] { MappingConfiguration.ClassDefinitions["Official"] }, true, true)));
    }

    [Test]
    public void GetScript_NoIEntityDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));
      var storageEntityDefinitionStub = MockRepository.GenerateStub<IStorageEntityDefinition>();
      storageEntityDefinitionStub.Stub (stub => stub.StorageProviderDefinition).Return (SchemaGenerationFirstStorageProviderDefinition);
      classDefinition.SetStorageEntity (storageEntityDefinitionStub);

      var result = _fileBuilderForFirstStorageProvider.GetScript (new ClassDefinitionCollection (new[] { classDefinition }, true, true));

      Assert.That (result, Is.EqualTo(_firstStorageProviderSetupDBScriptWithoutTables));
    }

    // TODO Review 3856: Move to FileBuilderBaseTest
    [Test]
    public void Build_WithMappingConfiguration ()
    {
      FileBuilderBase.Build (MappingConfiguration, DomainObjectsConfiguration.Current.Storage, "TestDirectory");

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
      Assert.AreEqual (_firstStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SchemaGenerationSecondStorageProvider.sql"));
      Assert.AreEqual (_secondStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_SchemaGenerationSecondStorageProvider.sql"));
    }

    // TODO Review 3856: Add FileBuilderBaseTest.GetClassesInStorageProvider tests
    // TODO Review 3856: Add FileBuilderBaseTest.GetFileName tests

    // TODO Review 3856: Move to FileBuilderBaseTest
    [Test]
    public void BuildWithEmptyMappingConfiguration ()
    {
      MockRepository mockRepository = new MockRepository();
      IMappingLoader mappingLoaderStub = mockRepository.StrictMock<IMappingLoader>();
      ClassDefinitionCollection classDefinitionCollection = new ClassDefinitionCollection();
      SetupResult.For (mappingLoaderStub.ResolveTypes).Return (true);
      SetupResult.For (mappingLoaderStub.NameResolver).Return (new ReflectionBasedNameResolver());
      SetupResult.For (mappingLoaderStub.GetClassDefinitions()).Return (new ClassDefinition[0]);
      SetupResult.For (mappingLoaderStub.GetRelationDefinitions (classDefinitionCollection)).Return (new RelationDefinition[0]);
      SetupResult.For (mappingLoaderStub.CreateClassDefinitionValidator()).Return (new ClassDefinitionValidator());
      SetupResult.For (mappingLoaderStub.CreatePropertyDefinitionValidator()).Return (new PropertyDefinitionValidator());
      SetupResult.For (mappingLoaderStub.CreateRelationDefinitionValidator()).Return (new RelationDefinitionValidator());
      SetupResult.For (mappingLoaderStub.CreateSortExpressionValidator()).Return (new SortExpressionValidator());
      mockRepository.ReplayAll();

      FileBuilderBase.Build (
          new MappingConfiguration (
              mappingLoaderStub, new PersistenceModelLoader (new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage))),
          DomainObjectsConfiguration.Current.Storage,
          "TestDirectory");

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
      Assert.AreEqual (
          _firstStorageProviderSetupDBScriptWithoutTables, File.ReadAllText (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
    }
  }
}