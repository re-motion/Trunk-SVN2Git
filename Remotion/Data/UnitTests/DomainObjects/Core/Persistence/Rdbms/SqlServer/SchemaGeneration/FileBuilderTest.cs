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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting.Resources;
using Rhino.Mocks;
using File = System.IO.File;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class FileBuilderTest : SchemaGenerationTestBase
  {
    private string _firstStorageProviderSetupDBScript;
    private string _secondStorageProviderSetupDBScript;
    private string _firstStorageProviderSetupDBScriptWithoutTables;
    private TestableFileBuilder _fileBuilder;
    private Func<RdbmsProviderDefinition, FileBuilder> _fileBuilderFactory;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      if (Directory.Exists ("TestDirectory"))
        Directory.Delete ("TestDirectory", true);
    }

    public override void SetUp ()
    {
      base.SetUp();

      _fileBuilder = new TestableFileBuilder (new ScriptBuilder (SchemaGenerationFirstStorageProviderDefinition));

      _firstStorageProviderSetupDBScript = ResourceUtility.GetResourceString (typeof (ScriptBuilderTest), "TestData.SetupDB_FirstStorageProvider.sql");
      _secondStorageProviderSetupDBScript = ResourceUtility.GetResourceString (
          typeof (ScriptBuilderTest), "TestData.SetupDB_SecondStorageProvider.sql");
      _firstStorageProviderSetupDBScriptWithoutTables = ResourceUtility.GetResourceString (
          typeof (ScriptBuilderTest), "TestData.SetupDB_FirstStorageProviderWithoutTables.sql");

      _fileBuilderFactory = pd => new FileBuilder (new ScriptBuilder (pd));
    }

    public override void TearDown ()
    {
      base.TearDown();

      if (Directory.Exists ("TestDirectory"))
        Directory.Delete ("TestDirectory", true);
    }

    [Test]
    public void Build_WithMappingConfiguration ()
    {
      FileBuilder.Build (MappingConfiguration.ClassDefinitions, DomainObjectsConfiguration.Current.Storage, "TestDirectory", _fileBuilderFactory);

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
      Assert.AreEqual (_firstStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SchemaGenerationSecondStorageProvider.sql"));
      Assert.AreEqual (_secondStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_SchemaGenerationSecondStorageProvider.sql"));
    }

    [Test]
    public void BuildWithEmptyMappingConfiguration ()
    {
      FileBuilder.Build (new ClassDefinitionCollection(), DomainObjectsConfiguration.Current.Storage, "TestDirectory", _fileBuilderFactory);

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
      Assert.AreEqual (
          _firstStorageProviderSetupDBScriptWithoutTables, File.ReadAllText (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
    }

    [Test]
    public void GetClassesInStorageProvider ()
    {
      var firstStorageProviderTableDefinition = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition, "TestFirstProvider", null, new IColumnDefinition[0], new ITableConstraintDefinition[0]);
      var secondStorageProviderTableDefinition = new TableDefinition (
          SchemaGenerationSecondStorageProviderDefinition, "TestSecondProvider", null, new IColumnDefinition[0], new ITableConstraintDefinition[0]);

      var classDefinition1 = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));
      classDefinition1.SetStorageEntity (firstStorageProviderTableDefinition);
      var classDefinition2 = ClassDefinitionFactory.CreateClassDefinition (typeof (OrderItem));
      classDefinition1.SetStorageEntity (firstStorageProviderTableDefinition);
      var classDefinition3 = ClassDefinitionFactory.CreateClassDefinition (typeof (Customer));
      classDefinition1.SetStorageEntity (secondStorageProviderTableDefinition);

      var classesInFirstStorageProvider = _fileBuilder.GetClassesInStorageProvider (
          new ClassDefinitionCollection (new[] { classDefinition1, classDefinition2, classDefinition3 }, true, true),
          SchemaGenerationFirstStorageProviderDefinition);

      Assert.That (classesInFirstStorageProvider.Count, Is.EqualTo (2));
    }

    [Test]
    public void GetFileName ()
    {
      var result = FileBuilder.GetFileName (SchemaGenerationFirstStorageProviderDefinition, "TestOutputPath", false);

      Assert.That (result, Is.EqualTo ("TestOutputPath\\SetupDB.sql"));
    }

    [Test]
    public void GetFileName_MultipleStorageProviders ()
    {
      var result = FileBuilder.GetFileName (SchemaGenerationFirstStorageProviderDefinition, "TestOutputPath", true);

      Assert.That (result, Is.EqualTo ("TestOutputPath\\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
    }

    [Test]
    public void GetScript_NoIEntityDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));
      var storageEntityDefinitionStub = MockRepository.GenerateStub<IStorageEntityDefinition>();
      storageEntityDefinitionStub.Stub (stub => stub.StorageProviderDefinition).Return (SchemaGenerationFirstStorageProviderDefinition);
      classDefinition.SetStorageEntity (storageEntityDefinitionStub);

      var result = _fileBuilder.GetScript (new ClassDefinitionCollection (new[] { classDefinition }, true, true));

      Assert.That (result, Is.EqualTo (_firstStorageProviderSetupDBScriptWithoutTables));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Class 'Official' has storage provider 'SchemaGenerationSecondStorageProvider' defined, but storage provider 'SchemaGenerationFirstStorageProvider' is required."
        )]
    public void GetScript_WithWrongStorageProvider ()
    {
      Assert.AreEqual (
          _firstStorageProviderSetupDBScript,
          _fileBuilder.GetScript (new ClassDefinitionCollection (new[] { MappingConfiguration.ClassDefinitions["Official"] }, true, true)));
    }

    [Test]
    public void GetEntityDefinitions ()
    {
      var entityDefinition1 = MockRepository.GenerateStub<IEntityDefinition>();
      var entityDefinition2 = MockRepository.GenerateStub<IEntityDefinition> ();
      var storageEntityDefinition = MockRepository.GenerateStub<IStorageEntityDefinition>();

      var classDefinition1 = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));
      var classDefinition2 = ClassDefinitionFactory.CreateClassDefinition (typeof (OrderItem));
      var classDefinition3 = ClassDefinitionFactory.CreateClassDefinition (typeof (Customer));

      classDefinition1.SetStorageEntity (entityDefinition1);
      classDefinition2.SetStorageEntity (storageEntityDefinition);
      classDefinition3.SetStorageEntity (entityDefinition2);

      var result =
          _fileBuilder.GetEntityDefinitions (
              new ClassDefinitionCollection (new[] { classDefinition1, classDefinition2, classDefinition3 }, true, true));

      Assert.That (result.ToArray(), Is.EqualTo (new[] { entityDefinition1, entityDefinition2 }));
    }
  }
}