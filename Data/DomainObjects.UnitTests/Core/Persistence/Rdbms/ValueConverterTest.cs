using System;
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence.Rdbms
{
  [TestFixture]
  public class ValueConverterTest : StandardMappingTest
  {
    ValueConverter _converter;
    StorageProviderManager _storageProviderManager;
    IDbConnection _connection;

    ClassDefinition _ceoDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _storageProviderManager = new StorageProviderManager ();
      RdbmsProvider provider = (RdbmsProvider) _storageProviderManager.GetMandatory ("TestDomain");
      provider.Connect ();
      _connection = provider.Connection;

      _ceoDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Ceo");
      _converter = new ValueConverter (provider, TypeConversionProvider.Create ());
    }

    public override void TearDown ()
    {
      base.TearDown ();
      _storageProviderManager.Dispose ();
    }

    [Test]
    public void GetObjectIDWithGuidValue ()
    {
      Guid value = Guid.NewGuid ();
      ObjectID expectedID = new ObjectID ("Order", value);
      ObjectID actualID = _converter.GetObjectID (MappingConfiguration.Current.ClassDefinitions["Order"], value);

      Assert.AreEqual (expectedID, actualID);
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage = "Invalid null value for not-nullable property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Type' encountered. Class: 'Customer'.")]
    public void GetNullValueForEnum ()
    {
      ClassDefinition customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
      PropertyDefinition enumProperty = customerDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Type"];

      _converter.GetValue (customerDefinition, enumProperty, DBNull.Value);
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage = "Invalid null value for not-nullable relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company' encountered. Class: 'Ceo'.")]
    public void GetValueForCeoWithCompanyIDAndCompanyIDClassIDNull ()
    {
      IDbCommand command = CreateCeoCommand (new Guid ("{2927059E-AE59-49a7-8B59-B959E579C629}"));
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());
        _converter.GetValue (_ceoDefinition, _ceoDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company"], reader);
      }
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage = "Invalid null value for not-nullable relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company' encountered. Class: 'Ceo'.")]
    public void GetValueForCeoWithCompanyIDNull ()
    {
      IDbCommand command = CreateCeoCommand (new Guid ("{523B490A-5B18-4f22-AF5B-BD9A4DA3F629}"));
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());
        _converter.GetValue (_ceoDefinition, _ceoDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company"], reader);
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Incorrect database value encountered. Column 'CompanyIDClassID' of entity 'Ceo' must not contain null.")]
    public void GetValueForCeoWithCompanyIDClassIDNull ()
    {
      IDbCommand command = CreateCeoCommand (new Guid ("{04341C7D-7B7C-49fc-82E6-8E481CDACA30}"));
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());

        _converter.GetValue (_ceoDefinition, _ceoDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company"], reader);
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Incorrect database value encountered. Column 'CompanyIDClassID' of"
        + " entity 'TableWithOptionalOneToOneRelationAndOppositeDerivedClass' must not contain a value.")]
    public void GetValueForClassWithOptionalOneToOneRelationAndOppositeDerivedClassWithCompanyIDClassIDNotNull ()
    {
      ClassDefinition classWithOptionalOneToOneRelationAndOppositeDerivedClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (
          "ClassWithOptionalOneToOneRelationAndOppositeDerivedClass");

      IDbCommand command = CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassCommand (new Guid ("{5115A733-5CD1-46C5-81EE-0B50EF0A5858}"));
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());
        _converter.GetValue (
            classWithOptionalOneToOneRelationAndOppositeDerivedClass,
            classWithOptionalOneToOneRelationAndOppositeDerivedClass["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass.Company"], 
            reader);
      }
    }

    [Test]
    public void GetValueForCeo ()
    {
      IDbCommand command = CreateCeoCommand ((Guid) DomainObjectIDs.Ceo1.Value);
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());
        ObjectID actualID = (ObjectID) _converter.GetValue (_ceoDefinition, _ceoDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company"], reader);

        Assert.AreEqual (DomainObjectIDs.Company1, actualID);
      }
    }

    [Test]
    public void GetValueForFolderWithoutParent ()
    {
      IDbCommand command = CreateFileSystemItemCommand (new Guid ("{976A6864-3344-4b3c-8F67-6348CF361D22}"));
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());

        ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Folder");
        PropertyDefinition parentFolderProperty = folderDefinition.GetMandatoryPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder");

        Assert.IsNull (_converter.GetValue (folderDefinition, parentFolderProperty, reader));
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Incorrect database value encountered. Column 'ParentFolderIDClassID' of entity 'FileSystemItem' must not contain a value.")]
    public void GetValueForFileWithParentFolderIDNull ()
    {
      IDbCommand command = CreateFileSystemItemCommand (new Guid ("{DCBE9554-2724-49a6-AECA-B811E20E4110}"));
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());

        ClassDefinition fileDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("File");
        PropertyDefinition parentFolderProperty = fileDefinition.GetMandatoryPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder");

        Assert.IsNull (_converter.GetValue (fileDefinition, parentFolderProperty, reader));
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Incorrect database value encountered. Column 'ParentFolderIDClassID' of entity 'FileSystemItem' must not contain null.")]
    public void GetValueForFileWithParentFolderIDClassIDNull ()
    {
      IDbCommand command = CreateFileSystemItemCommand (new Guid ("{A26B6A4E-D497-4b32-821B-74AFAD7EAD0A}"));
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());

        ClassDefinition fileDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("File");
        PropertyDefinition parentFolderProperty = fileDefinition.GetMandatoryPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder");

        Assert.IsNull (_converter.GetValue (fileDefinition, parentFolderProperty, reader));
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Invalid database value encountered. Column 'ClassID' must not contain null.")]
    public void GetIDWithClassIDNull ()
    {
      using (IDbCommand command = _connection.CreateCommand ())
      {
        command.CommandText = string.Format ("SELECT '{0}' as ID, null as ClassID;", DomainObjectIDs.Person1.Value);
        using (IDataReader reader = command.ExecuteReader ())
        {
          Assert.IsTrue (reader.Read ());
          _converter.GetID (reader);
        }
      }
    }

    [Test]
    public void GetNullID ()
    {
      using (IDbCommand command = _connection.CreateCommand ())
      {
        command.CommandText = string.Format ("SELECT null as ID, 'Client' as ClassID;");
        using (IDataReader reader = command.ExecuteReader ())
        {
          Assert.IsTrue (reader.Read ());
          ObjectID id = _converter.GetID (reader);
          Assert.IsNull (id);
        }
      }
    }

    [Test]
    public void GetNullIDWithClassIDNull ()
    {
      using (IDbCommand command = _connection.CreateCommand ())
      {
        command.CommandText = string.Format ("SELECT null as ID, null as ClassID;");
        using (IDataReader reader = command.ExecuteReader ())
        {
          Assert.IsTrue (reader.Read ());
          ObjectID id = _converter.GetID (reader);
          Assert.IsNull (id);
        }
      }
    }

    private IDbCommand CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassCommand (Guid id)
    {
      return CreateCommand ("TableWithOptionalOneToOneRelationAndOppositeDerivedClass", id, _connection);
    }

    private IDbCommand CreateCeoCommand (Guid id)
    {
      return CreateCommand ("Ceo", id, _connection);
    }

    private IDbCommand CreateFileSystemItemCommand (Guid id)
    {
      return CreateCommand ("FileSystemItem", id, _connection);
    }
  }
}