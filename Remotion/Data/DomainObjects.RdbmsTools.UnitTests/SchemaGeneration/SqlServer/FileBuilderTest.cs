// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration;
using Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration.SqlServer;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.SchemaGeneration.SqlServer
{
  //TODO: Run the generated SQL File against a database in the UnitTests and integrate this into the build
  //      Derive ClassWithAllDataTypes from an abstract class to ensure that all data types are selected in a UNION
  [TestFixture]
  public class FileBuilderTest : StandardMappingTest
  {
    private RdbmsProviderDefinition _firstStorageProviderDefinition;
    private RdbmsProviderDefinition _secondStorageProviderDefinition;
    private FileBuilder _fileBuilder;
    private string _firstStorageProviderSetupDBScript;
    private string _secondStorageProviderSetupDBScript;
    private string _firstStorageProviderSetupDBScriptWithoutTables;

    public override void TextFixtureSetUp ()
    {
      base.TextFixtureSetUp();

      if (Directory.Exists ("TestDirectory"))
        Directory.Delete ("TestDirectory", true);
    }

    public override void SetUp ()
    {
      base.SetUp();

      _firstStorageProviderDefinition =
          (RdbmsProviderDefinition) StorageConfiguration.StorageProviderDefinitions.GetMandatory ("FirstStorageProvider");
      _secondStorageProviderDefinition =
          (RdbmsProviderDefinition) StorageConfiguration.StorageProviderDefinitions.GetMandatory ("SecondStorageProvider");
      _fileBuilder = new FileBuilder (MappingConfiguration, _firstStorageProviderDefinition);
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
      Assert.AreEqual ("RdbmsToolsUnitTests1", _fileBuilder.GetDatabaseName ());
    }

    [Test]
    public void GetScriptForSecondStorageProvider ()
    {
      FileBuilder sqlFileBuilder = new FileBuilder (MappingConfiguration, _secondStorageProviderDefinition);

      Assert.AreEqual (_secondStorageProviderSetupDBScript, sqlFileBuilder.GetScript());
    }

    [Test]
    public void GetScriptForFirstStorageProvider ()
    {
      Assert.AreEqual (_firstStorageProviderSetupDBScript, _fileBuilder.GetScript());
    }

    [Test]
    public void BuildWithMappingConfiguration ()
    {
      FileBuilderBase.Build (typeof (FileBuilder), MappingConfiguration, StorageConfiguration, "TestDirectory");

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_FirstStorageProvider.sql"));
      Assert.AreEqual (_firstStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_FirstStorageProvider.sql"));
      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SecondStorageProvider.sql"));
      Assert.AreEqual (_secondStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_SecondStorageProvider.sql"));
    }

    [Test]
    public void BuildWithEmptyMappingConfiguration ()
    {
      MockRepository mockRepository = new MockRepository();
      IMappingLoader mappingLoaderStub = mockRepository.StrictMock<IMappingLoader>();
      ClassDefinitionCollection classDefinitionCollection = new ClassDefinitionCollection();
      SetupResult.For (mappingLoaderStub.ResolveTypes).Return (true);
      SetupResult.For (mappingLoaderStub.NameResolver).Return (new ReflectionBasedNameResolver());
      SetupResult.For (mappingLoaderStub.GetClassDefinitions ()).Return (classDefinitionCollection);
      SetupResult.For (mappingLoaderStub.GetRelationDefinitions (classDefinitionCollection)).Return (new RelationDefinitionCollection());
      mockRepository.ReplayAll();

      FileBuilderBase.Build (typeof (FileBuilder), new MappingConfiguration (mappingLoaderStub), StorageConfiguration, "TestDirectory");

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_FirstStorageProvider.sql"));
      Assert.AreEqual (_firstStorageProviderSetupDBScriptWithoutTables, File.ReadAllText (@"TestDirectory\SetupDB_FirstStorageProvider.sql"));
    }
  }
}
