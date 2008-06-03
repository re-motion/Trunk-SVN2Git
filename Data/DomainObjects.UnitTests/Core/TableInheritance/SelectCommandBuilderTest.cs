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
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance
{
  [TestFixture]
  public class SelectCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    public void CreateForIDLookupWithMultipleValues ()
    {
      ClassDefinition personClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Person));

      SelectCommandBuilder sqlCommandBuilder = SelectCommandBuilder.CreateForIDLookup (
          Provider, personClass.GetEntityName (), new ObjectID[] { DomainObjectIDs.Person, DomainObjectIDs.Customer });

      Assert.IsNotNull (sqlCommandBuilder);

      Provider.Connect ();
      using (IDbCommand command = sqlCommandBuilder.Create ())
      {
        string expectedCommandText = "SELECT * FROM [TableInheritance_Person] WHERE [ID] IN (@ID1, @ID2);";
        Assert.AreEqual (expectedCommandText, command.CommandText);
        Assert.AreEqual (2, command.Parameters.Count);
        Assert.AreEqual (DomainObjectIDs.Person.Value, ((SqlParameter) command.Parameters["@ID1"]).Value);
        Assert.AreEqual (DomainObjectIDs.Customer.Value, ((SqlParameter) command.Parameters["@ID2"]).Value);
      }
    }

  }
}
