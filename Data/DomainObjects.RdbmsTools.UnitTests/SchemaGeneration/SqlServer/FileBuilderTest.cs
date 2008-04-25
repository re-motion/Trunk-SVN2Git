using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration;
using Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration.SqlServer;

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
      _secondStorageProviderSetupDBScript = GetEmbeddedStringResource ("TestData.SetupDB_SecondStorageProvider.sql");
    }

    private string GetEmbeddedStringResource (string name)
    {
      Assembly assembly = GetType().Assembly;
      StreamReader reader = new StreamReader (assembly.GetManifestResourceStream (typeof (FileBuilderTest), name));
      return reader.ReadToEnd();
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
  }
}