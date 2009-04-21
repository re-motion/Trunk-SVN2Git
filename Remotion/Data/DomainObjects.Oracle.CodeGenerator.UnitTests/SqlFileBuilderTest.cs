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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.NullableValueTypes;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using System.Reflection;
using Remotion.Data.DomainObjects.CodeGenerator.Sql;

namespace Remotion.Data.DomainObjects.Oracle.CodeGenerator.UnitTests
{
  //TODO: Run the generated SQL File against a database in the UnitTests and integrate this into the build
  //      Derive ClassWithAllDataTypes from an abstract class to ensure that all data types are selected in a UNION
  [TestFixture]
  public class SqlFileBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private RdbmsProviderDefinition _firstStorageProviderDefinition;
    private RdbmsProviderDefinition _secondStorageProviderDefinition;
    private SqlFileBuilder _fileBuilder;
    private string _firstStorageProviderSetupDBScript;
    private string _secondStorageProviderSetupDBScript;

    // construction and disposing

    public SqlFileBuilderTest ()
    {
    }

    // methods and properties

    public override void TextFixtureSetUp ()
    {
      base.TextFixtureSetUp ();

      if (Directory.Exists ("TestDirectory"))
        Directory.Delete ("TestDirectory", true);
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _firstStorageProviderDefinition = (RdbmsProviderDefinition) StorageProviderConfiguration.StorageProviderDefinitions.GetMandatory ("FirstStorageProvider");
      _secondStorageProviderDefinition = (RdbmsProviderDefinition) StorageProviderConfiguration.StorageProviderDefinitions.GetMandatory ("SecondStorageProvider");
      _fileBuilder = new SqlFileBuilder (MappingConfiguration, _firstStorageProviderDefinition);
      _firstStorageProviderSetupDBScript = GetEmbeddedStringResource ("TestData.SetupDB_FirstStorageProvider.sql");
      _secondStorageProviderSetupDBScript = GetEmbeddedStringResource ("TestData.SetupDB_SecondStorageProvider.sql");
    }

    private string GetEmbeddedStringResource (string name)
    {
      Assembly assembly = GetType ().Assembly;
      StreamReader reader = new StreamReader (assembly.GetManifestResourceStream (typeof (SqlFileBuilderTest), name));
      return reader.ReadToEnd ();
   }

    public override void TearDown ()
    {
      base.TearDown ();

      //if (Directory.Exists ("TestDirectory"))
      //  Directory.Delete ("TestDirectory", true);
    }

    [Test]
    public void GetDatabaseName ()
    {
      Assert.AreEqual ("CodeGeneratorUnitTests1", _fileBuilder.GetDatabaseName ());
    }

    [Test]
    public void GetScriptForSecondStorageProvider ()
    {
      SqlFileBuilder sqlFileBuilder = new SqlFileBuilder (MappingConfiguration, _secondStorageProviderDefinition);

      Assert.AreEqual (_secondStorageProviderSetupDBScript, sqlFileBuilder.GetScript ());
    }

    [Test]
    public void GetScriptForFirstStorageProvider ()
    {
      Assert.AreEqual (_firstStorageProviderSetupDBScript, _fileBuilder.GetScript ());
    }

    [Test]
    public void BuildWithMappingConfiguration ()
    {
      SqlFileBuilderBase.Build (typeof (SqlFileBuilder), MappingConfiguration, StorageProviderConfiguration, "TestDirectory");

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_FirstStorageProvider.sql"));
      Assert.AreEqual (_firstStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_FirstStorageProvider.sql"));
      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SecondStorageProvider.sql"));
      Assert.AreEqual (_secondStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_SecondStorageProvider.sql"));
    }
  }
}
