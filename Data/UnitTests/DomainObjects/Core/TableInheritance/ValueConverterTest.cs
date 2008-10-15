/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class ValueConverterTest : SqlProviderBaseTest
  {
    ValueConverter _converter;

    public override void SetUp ()
    {
      base.SetUp ();

      Provider.Connect ();
      _converter = new ValueConverter (Provider, TypeConversionProvider.Create ());
    }

    [Test]
    public void GetObjectIDValue ()
    {
      ClassDefinition personClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Person));
      PropertyDefinition clientProperty = personClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.DomainBase.Client");
      ObjectID expectedID = DomainObjectIDs.Client;

      using (IDbCommand command = CreatePersonCommand ((Guid) DomainObjectIDs.Person.Value))
      {
        using (IDataReader reader = command.ExecuteReader ())
        {
          Assert.IsTrue (reader.Read ());
          Assert.AreEqual (expectedID, _converter.GetValue (personClass, clientProperty, reader));
        }
      }
    }

    [Test]
    public void GetIDWithAbstractClassID ()
    {
      using (IDbCommand command = Provider.Connection.CreateCommand ())
      {
        command.CommandText = string.Format ("SELECT '{0}' as ID, 'TI_DomainBase' as ClassID;", DomainObjectIDs.Person.Value);
        using (IDataReader reader = command.ExecuteReader ())
        {
          Assert.IsTrue (reader.Read ());

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
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "Incorrect database format encountered. Entity 'TableInheritance_BaseClassWithInvalidRelationClassIDColumns' must have column"
        + " 'DomainBaseIDClassID' defined, because opposite class 'TI_DomainBase' is part of an inheritance hierarchy.")]
    public void GetValueWithMissingRelationClassIDColumn ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (
          typeof (DerivedClassWithInvalidRelationClassIDColumns));

      ObjectID id = new ObjectID (classDefinition, new Guid ("{BEBF584B-31A6-4d5e-8628-7EACE9034588}"));

      SelectCommandBuilder builder = SelectCommandBuilder.CreateForIDLookup (Provider, "*", classDefinition.GetEntityName (), id);
      using (IDbCommand command = builder.Create ())
      {
        using (IDataReader reader = command.ExecuteReader ())
        {
          Assert.IsTrue (reader.Read ());

          _converter.GetValue (
              classDefinition, 
              classDefinition.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.BaseClassWithInvalidRelationClassIDColumns.DomainBase"),
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
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (
          typeof (DerivedClassWithInvalidRelationClassIDColumns));

      ObjectID id = new ObjectID (classDefinition, new Guid ("{BEBF584B-31A6-4d5e-8628-7EACE9034588}"));

      SelectCommandBuilder builder = SelectCommandBuilder.CreateForIDLookup (Provider, "*", classDefinition.GetEntityName (), id);
      using (IDbCommand command = builder.Create ())
      {
        using (IDataReader reader = command.ExecuteReader ())
        {
          Assert.IsTrue (reader.Read ());

          _converter.GetValue (
              classDefinition, 
              classDefinition.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.BaseClassWithInvalidRelationClassIDColumns.Client"),
              reader);
        }
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException),
        ExpectedMessage = "Incorrect database value encountered. Column 'DomainBaseWithInvalidClassIDValueIDClassID' of entity"
        + " 'TableInheritance_BaseClassWithInvalidRelationClassIDColumns' must not contain a value.")]
    public void GetValueWithInvalidRelationClassIDColumnValue ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (
          typeof (DerivedClassWithInvalidRelationClassIDColumns));

      ObjectID id = new ObjectID (classDefinition, new Guid ("{BEBF584B-31A6-4d5e-8628-7EACE9034588}"));

      SelectCommandBuilder builder = SelectCommandBuilder.CreateForIDLookup (Provider, "*", classDefinition.GetEntityName (), id);
      using (IDbCommand command = builder.Create ())
      {
        using (IDataReader reader = command.ExecuteReader ())
        {
          Assert.IsTrue (reader.Read ());

          _converter.GetValue (
              classDefinition, 
              classDefinition.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.BaseClassWithInvalidRelationClassIDColumns.DomainBaseWithInvalidClassIDValue"),
              reader);
        }
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException),
        ExpectedMessage = "Incorrect database value encountered. Column 'DomainBaseWithInvalidClassIDNullValueIDClassID' of entity"
        + " 'TableInheritance_BaseClassWithInvalidRelationClassIDColumns' must not contain null.")]
    public void GetValueWithInvalidRelationClassIDColumnNullValue ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (
          typeof (DerivedClassWithInvalidRelationClassIDColumns));

      ObjectID id = new ObjectID (classDefinition, new Guid ("{BEBF584B-31A6-4d5e-8628-7EACE9034588}"));

      SelectCommandBuilder builder = SelectCommandBuilder.CreateForIDLookup (Provider, "*", classDefinition.GetEntityName (), id);
      using (IDbCommand command = builder.Create ())
      {
        using (IDataReader reader = command.ExecuteReader ())
        {
          Assert.IsTrue (reader.Read ());

          _converter.GetValue (
              classDefinition, 
              classDefinition.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.BaseClassWithInvalidRelationClassIDColumns.DomainBaseWithInvalidClassIDNullValue"),
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
