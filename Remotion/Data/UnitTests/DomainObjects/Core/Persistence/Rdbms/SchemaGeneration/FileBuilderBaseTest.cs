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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting.Resources;
using File = System.IO.File;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class FileBuilderBaseTest : SchemaGenerationTestBase
  {
    private string _firstStorageProviderSetupDBScript;
    private string _secondStorageProviderSetupDBScript;
    private string _firstStorageProviderSetupDBScriptWithoutTables;

    public override void SetUp ()
    {
      base.SetUp ();

      _firstStorageProviderSetupDBScript = ResourceUtility.GetResourceString (typeof(FileBuilderTest), "TestData.SetupDB_FirstStorageProvider.sql");
      _secondStorageProviderSetupDBScript = ResourceUtility.GetResourceString (typeof(FileBuilderTest), "TestData.SetupDB_SecondStorageProvider.sql");
      _firstStorageProviderSetupDBScriptWithoutTables = ResourceUtility.GetResourceString (typeof (FileBuilderTest), "TestData.SetupDB_FirstStorageProviderWithoutTables.sql");
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

      var classesInFirstStorageProvider = FileBuilderBase.GetClassesInStorageProvider (
          new ClassDefinitionCollection (new[] { classDefinition1, classDefinition2, classDefinition3 }, true, true),
          SchemaGenerationFirstStorageProviderDefinition);

      Assert.That (classesInFirstStorageProvider.Count, Is.EqualTo (2));
    }

    [Test]
    public void GetFileName ()
    {
      var result = FileBuilderBase.GetFileName (SchemaGenerationFirstStorageProviderDefinition, "TestOutputPath", false);

      Assert.That (result, Is.EqualTo ("TestOutputPath\\SetupDB.sql"));
    }

    [Test]
    public void GetFileName_MultipleStorageProviders ()
    {
      var result = FileBuilderBase.GetFileName (SchemaGenerationFirstStorageProviderDefinition, "TestOutputPath", true);

      Assert.That (result, Is.EqualTo ("TestOutputPath\\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
    }

    [Test]
    public void Build_WithMappingConfiguration ()
    {
      FileBuilderBase.Build (MappingConfiguration.ClassDefinitions, DomainObjectsConfiguration.Current.Storage, "TestDirectory");

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
      Assert.AreEqual (_firstStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SchemaGenerationSecondStorageProvider.sql"));
      Assert.AreEqual (_secondStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_SchemaGenerationSecondStorageProvider.sql"));
    }

    [Test]
    public void BuildWithEmptyMappingConfiguration ()
    {
      FileBuilderBase.Build (new ClassDefinitionCollection(), DomainObjectsConfiguration.Current.Storage, "TestDirectory");

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
      Assert.AreEqual (
          _firstStorageProviderSetupDBScriptWithoutTables, File.ReadAllText (@"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
    }
  }
}