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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  //TODO: Run the generated SQL File against a database in the UnitTests and integrate this into the build
  //      Derive ClassWithAllDataTypes from an abstract class to ensure that all data types are selected in a UNION
  [TestFixture]
  public class FileBuilderTest : SchemaGenerationTestBase
  {
    private RdbmsProviderDefinition _firstStorageProviderDefinition;
    private RdbmsProviderDefinition _secondStorageProviderDefinition;
    private FileBuilder _fileBuilder;
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

      _firstStorageProviderDefinition = SchemaGenerationFirstStorageProviderDefinition;
      _secondStorageProviderDefinition = SchemaGenerationSecondStorageProviderDefinition;

      _fileBuilder = new FileBuilder (_firstStorageProviderDefinition);
      _firstStorageProviderSetupDBScript = GetEmbeddedStringResource ("TestData.SetupDB_FirstStorageProvider.sql");
      _firstStorageProviderSetupDBScriptWithoutTables = GetEmbeddedStringResource ("TestData.SetupDB_FirstStorageProviderWithoutTables.sql");
      _secondStorageProviderSetupDBScript = GetEmbeddedStringResource ("TestData.SetupDB_SecondStorageProvider.sql");
    }

    private string GetEmbeddedStringResource (string name)
    {
      Assembly assembly = GetType().Assembly;
      using (StreamReader reader = new StreamReader (assembly.GetManifestResourceStream (typeof (FileBuilderTest), name)))
      {
        return reader.ReadToEnd();
      }
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
      Assert.AreEqual ("TestDomain", _fileBuilder.GetDatabaseName());
    }

    [Test]
    public void GetScriptForSecondStorageProvider ()
    {
      FileBuilder sqlFileBuilder = new FileBuilder (_secondStorageProviderDefinition);

      Assert.AreEqual (
          _secondStorageProviderSetupDBScript,
          sqlFileBuilder.GetScript (FileBuilder.GetClassesInStorageProvider (MappingConfiguration.ClassDefinitions, _secondStorageProviderDefinition)));
    }

    [Test]
    public void GetScriptForFirstStorageProvider ()
    {
      Assert.AreEqual (
          _firstStorageProviderSetupDBScript,
          _fileBuilder.GetScript (FileBuilder.GetClassesInStorageProvider (MappingConfiguration.ClassDefinitions, _firstStorageProviderDefinition)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Class 'Official' has storage provider 'SchemaGenerationSecondStorageProvider' defined, but storage provider 'SchemaGenerationFirstStorageProvider' is required."
        )]
    public void GetScriptForFirstWrongStorageProvider ()
    {
      Assert.AreEqual (
          _firstStorageProviderSetupDBScript,
          _fileBuilder.GetScript (
              FileBuilder.GetClassesInStorageProvider (
                  new ClassDefinitionCollection (new[]{ MappingConfiguration.ClassDefinitions["Official"]}, true, true), _secondStorageProviderDefinition)));
    }

    [Test]
    public void BuildWithMappingConfiguration ()
    {
      FileBuilderBase.Build (MappingConfiguration, DomainObjectsConfiguration.Current.Storage, "TestDirectory");

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
      Assert.AreEqual (_firstStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SchemaGenerationSecondStorageProvider.sql"));
      Assert.AreEqual (_secondStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_SchemaGenerationSecondStorageProvider.sql"));
    }

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
      SetupResult.For (mappingLoaderStub.CreateClassDefinitionValidator()).Return (CreateClassDefinitionValidator());
      SetupResult.For (mappingLoaderStub.CreatePropertyDefinitionValidator()).Return (CreatePropertyDefinitionValidator());
      SetupResult.For (mappingLoaderStub.CreateRelationDefinitionValidator()).Return (CreateRelationDefinitionValidator());
      SetupResult.For (mappingLoaderStub.CreateSortExpressionValidator()).Return (CreateSortExpressionValidator());
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

    private ClassDefinitionValidator CreateClassDefinitionValidator ()
    {
      return new ClassDefinitionValidator();
    }

    private PropertyDefinitionValidator CreatePropertyDefinitionValidator ()
    {
      return new PropertyDefinitionValidator();
    }

    private RelationDefinitionValidator CreateRelationDefinitionValidator ()
    {
      return new RelationDefinitionValidator();
    }

    private SortExpressionValidator CreateSortExpressionValidator ()
    {
      return new SortExpressionValidator();
    }
  }
}