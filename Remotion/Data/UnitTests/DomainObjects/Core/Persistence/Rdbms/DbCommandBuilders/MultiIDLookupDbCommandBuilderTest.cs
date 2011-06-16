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
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Mixins;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class MultiIDLookupDbCommandBuilderTest : SqlProviderBaseTest
  {
    private IValueConverter _valueConverterStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _valueConverterStub = MockRepository.GenerateStub<IValueConverter>();
      _valueConverterStub.Stub (stub => stub.IsOfSameStorageProvider (Arg<ObjectID>.Is.Anything)).Return (true);
    }

    [Test]
    public void Create ()
    {
      var expectedCommandText = "SELECT * FROM [Order] WHERE [ID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @ID.nodes('/L/I') T(c));";
      var expectedXml = "<L><I>" + DomainObjectIDs.Order1.Value + "</I><I>" + DomainObjectIDs.Order2.Value + "</I></L>";
      _valueConverterStub.Stub (stub => stub.GetDBValue (Arg<object>.Is.Anything)).Return (expectedXml);

      Provider.Connect ();
      var builder = new MultiIDLookupDbCommandBuilder ("*", 
          "Order", 
          "ID", 
          "uniqueidentifier", 
          Provider.SqlDialect,
          _valueConverterStub,
          new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 });

      using (var command = builder.Create (Provider))
      {
        Assert.AreEqual (expectedCommandText, command.CommandText);
        Assert.AreEqual (1, command.Parameters.Count);

        Assert.AreEqual (expectedXml, ((SqlParameter) command.Parameters["@ID"]).Value);
        Assert.AreEqual (SqlDbType.Xml, ((SqlParameter) command.Parameters["@ID"]).SqlDbType);
      }
    }

    [Test]
    public void WhereClauseBuilder_CanBeMixed ()
    {
      Provider.Connect ();
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (WhereClauseBuilder)).Clear().AddMixins (typeof (WhereClauseBuilderMixin)).EnterScope())
      {
        var builder = new MultiIDLookupDbCommandBuilder ("*",
          "Order",
          "ID",
          "uniqueidentifier",
          Provider.SqlDialect,
          _valueConverterStub,
          new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 });

        using (IDbCommand command = builder.Create(Provider))
        {
          Assert.IsTrue (command.CommandText.Contains ("Mixed!"));
        }
      }
    }
  }
}
