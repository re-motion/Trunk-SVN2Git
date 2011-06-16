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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class ValueConverterTest : SqlProviderBaseTest
  {
    private ValueConverter _converter;

    public override void SetUp ()
    {
      base.SetUp();

      Provider.Connect();
      _converter = new ValueConverter (Provider.StorageProviderDefinition, new ReflectionBasedStorageNameProvider(), TypeConversionProvider.Create());
    }

    [Test]
    public void IsOfSameStorageProvider_True ()
    {
      var objectID = new ObjectID (typeof(Client), Guid.NewGuid());

      var result = _converter.IsOfSameStorageProvider (objectID);

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsOfSameStorageProvider_False ()
    {
      var objectID = new ObjectID ("Order", Guid.NewGuid ());

      var result = _converter.IsOfSameStorageProvider (objectID);

      Assert.That (result, Is.False);
    }

    [Test]
    public void GetObjectIDValue ()
    {
      ClassDefinition personClass = MappingConfiguration.Current.GetTypeDefinition (typeof (Person));
      PropertyDefinition clientProperty =
          personClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.DomainBase.Client");
      ObjectID expectedID = DomainObjectIDs.Client;

      using (IDbCommand command = CreatePersonCommand ((Guid) DomainObjectIDs.Person.Value))
      {
        using (IDataReader reader = command.ExecuteReader())
        {
          Assert.IsTrue (reader.Read());
          Assert.AreEqual (expectedID, _converter.GetValue (personClass, clientProperty, reader));
        }
      }
    }

    [Test]
    public void GetIDWithAbstractClassID ()
    {
      using (IDbCommand command = Provider.Connection.CreateCommand())
      {
        command.CommandText = string.Format ("SELECT '{0}' as ID, 'TI_DomainBase' as ClassID;", DomainObjectIDs.Person.Value);
        using (IDataReader reader = command.ExecuteReader())
        {
          Assert.IsTrue (reader.Read());

          try
          {
            _converter.GetID (reader);
            Assert.Fail ("RdbmsProviderException was expected.");
          }
          catch (RdbmsProviderException ex)
          {
            string expectedMessage = string.Format (
                "Invalid database value encountered. Column 'ClassID' of row with ID '{0}' refers to abstract class 'TI_DomainBase'.",
                DomainObjectIDs.Person.Value);

            Assert.AreEqual (expectedMessage, ex.Message);
          }
        }
      }
    }

    [Test]
    public void GetTimestamp ()
    {
      var timestamp = "0x000000000001F77A";
      using (IDbCommand command = Provider.Connection.CreateCommand())
      {
        command.CommandText = string.Format ("SELECT '{0}' as Timestamp", timestamp);
        using (IDataReader reader = command.ExecuteReader())
        {
          Assert.IsTrue (reader.Read());
          var result = _converter.GetTimestamp (reader);
          Assert.That (result, Is.EqualTo (timestamp));
        }
      }
    }

    [Test]
    public void GetTimestamp_DBNullCOlumn ()
    {
      using (IDbCommand command = Provider.Connection.CreateCommand())
      {
        command.CommandText = "SELECT null as Timestamp";
        using (IDataReader reader = command.ExecuteReader())
        {
          Assert.IsTrue (reader.Read());
          var result = _converter.GetTimestamp (reader);
          Assert.That (result, Is.Null);
        }
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "Incorrect database format encountered. Entity 'TableInheritance_BaseClassWithInvalidRelationClassIDColumns' must have column"
        + " 'DomainBaseIDClassID' defined, because opposite class 'TI_DomainBase' is part of an inheritance hierarchy.")]
    public void GetValueWithMissingRelationClassIDColumn ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (DerivedClassWithInvalidRelationClassIDColumns));

      var id = new ObjectID (classDefinition, new Guid ("{BEBF584B-31A6-4d5e-8628-7EACE9034588}"));

      var builder = new SingleIDLookupDbCommandBuilder ("*",
          classDefinition.GetEntityName(),
          "ID",
          id,
          null,
          Provider.SqlDialect,
          Provider.CreateValueConverter());
      using (IDbCommand command = builder.Create (Provider))
      {
        using (IDataReader reader = command.ExecuteReader())
        {
          Assert.IsTrue (reader.Read());

          _converter.GetValue (
              classDefinition,
              classDefinition.GetMandatoryPropertyDefinition (
                  "Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.BaseClassWithInvalidRelationClassIDColumns.DomainBase"),
              reader);
        }
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "Incorrect database format encountered. Entity 'TableInheritance_BaseClassWithInvalidRelationClassIDColumns' must not contain column"
        + " 'ClientIDClassID', because opposite class 'TI_Client' is not part of an inheritance hierarchy.")]
    public void GetValueWithInvalidRelationClassIDColumn ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (DerivedClassWithInvalidRelationClassIDColumns));

      var id = new ObjectID (classDefinition, new Guid ("{BEBF584B-31A6-4d5e-8628-7EACE9034588}"));

      var builder = new SingleIDLookupDbCommandBuilder ("*",
          classDefinition.GetEntityName(),
          "ID",
          id,
          null,
          Provider.SqlDialect,
          Provider.CreateValueConverter());
      using (IDbCommand command = builder.Create (Provider))
      {
        using (IDataReader reader = command.ExecuteReader())
        {
          Assert.IsTrue (reader.Read());

          _converter.GetValue (
              classDefinition,
              classDefinition.GetMandatoryPropertyDefinition (
                  "Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.BaseClassWithInvalidRelationClassIDColumns.Client"),
              reader);
        }
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "Incorrect database value encountered. Column 'DomainBaseWithInvalidClassIDValueIDClassID' of entity"
        + " 'TableInheritance_BaseClassWithInvalidRelationClassIDColumns' must not contain a value.")]
    public void GetValueWithInvalidRelationClassIDColumnValue ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (DerivedClassWithInvalidRelationClassIDColumns));

      var id = new ObjectID (classDefinition, new Guid ("{BEBF584B-31A6-4d5e-8628-7EACE9034588}"));

      var builder = new SingleIDLookupDbCommandBuilder ("*",
          classDefinition.GetEntityName(),
          "ID",
          id,
          null,
          Provider.SqlDialect,
          Provider.CreateValueConverter());
      using (IDbCommand command = builder.Create (Provider))
      {
        using (IDataReader reader = command.ExecuteReader())
        {
          Assert.IsTrue (reader.Read());

          _converter.GetValue (
              classDefinition,
              classDefinition.GetMandatoryPropertyDefinition (
                  "Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.BaseClassWithInvalidRelationClassIDColumns.DomainBaseWithInvalidClassIDValue"),
              reader);
        }
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "Incorrect database value encountered. Column 'DomainBaseWithInvalidClassIDNullValueIDClassID' of entity"
        + " 'TableInheritance_BaseClassWithInvalidRelationClassIDColumns' must not contain null.")]
    public void GetValueWithInvalidRelationClassIDColumnNullValue ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (DerivedClassWithInvalidRelationClassIDColumns));

      var id = new ObjectID (classDefinition, new Guid ("{BEBF584B-31A6-4d5e-8628-7EACE9034588}"));

      var builder = new SingleIDLookupDbCommandBuilder ("*",
          classDefinition.GetEntityName(),
          "ID",
          id,
          null,
          Provider.SqlDialect,
          Provider.CreateValueConverter());
      using (IDbCommand command = builder.Create (Provider))
      {
        using (IDataReader reader = command.ExecuteReader())
        {
          Assert.IsTrue (reader.Read());

          _converter.GetValue (
              classDefinition,
              classDefinition.GetMandatoryPropertyDefinition (
                  "Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.BaseClassWithInvalidRelationClassIDColumns.DomainBaseWithInvalidClassIDNullValue"),
              reader);
        }
      }
    }

    private IDbCommand CreatePersonCommand (Guid id)
    {
      return CreateCommand ("TableInheritance_Person", id, Provider.Connection);
    }
  }
}