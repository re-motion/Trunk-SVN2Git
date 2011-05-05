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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Linq.SqlBackend.SqlStatementModel;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer
{
  [TestFixture]
  public class SqlDialectTest
  {
    private SqlDialect _dialect;

    [SetUp]
    public void SetUp ()
    {
      _dialect = SqlDialect.Instance;
    }

    [Test]
    public void StatementDelimiter ()
    {
      Assert.That (_dialect.StatementDelimiter, Is.EqualTo (";"));
    }

    [Test]
    public void DelimitIdentifier ()
    {
      Assert.That (_dialect.DelimitIdentifier ("x"), Is.EqualTo ("[x]"));
    }

    [Test]
    public void BatchSeparator ()
    {
      Assert.That (_dialect.BatchDelimiter, Is.EqualTo ("GO"));
    }

    [Test]
    public void GetParameterName ()
    {
      Assert.That (_dialect.GetParameterName ("parameter"), Is.EqualTo ("@parameter"));
      Assert.That (_dialect.GetParameterName ("@parameter"), Is.EqualTo ("@parameter"));
    }

    [Test]
    public void AdjustForConnectionString ()
    {
      var script = new List<ScriptStatement>();
      var connectionString = "Data Source=myServerAddress;Initial Catalog=MyDataBase;User Id=myUsername;Password=myPassword;";
      script.Add (new ScriptStatement ("Test"));

      _dialect.AdjustForConnectionString (script, connectionString);

      Assert.That (script.Count, Is.EqualTo (2));
      Assert.That (script[0].Statement, Is.EqualTo ("USE MyDataBase"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No database-name could be found in the given connection-string.")]
    public void AdjustForConnectionString_NoMarkerFound ()
    {
      _dialect.AdjustForConnectionString (new List<ScriptStatement> (), "Teststring");
    }
    
  }
}