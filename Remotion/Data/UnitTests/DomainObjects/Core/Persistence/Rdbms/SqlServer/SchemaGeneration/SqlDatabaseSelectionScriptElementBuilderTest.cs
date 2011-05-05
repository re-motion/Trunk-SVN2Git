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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlDatabaseSelectionScriptElementBuilderTest : SchemaGenerationTestBase
  {
    [Test]
    public void GetCreateScript_GetDropScript_ValidConnectionString ()
    {
      var connectionString = "Data Source=myServerAddress;Initial Catalog=MyDataBase;User Id=myUsername;Password=myPassword;";
      var builder = new SqlDatabaseSelectionScriptElementBuilder (connectionString);

      var createScriptResult = builder.GetCreateScript();
      var dropScriptResult = builder.GetDropScript();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("USE MyDataBase"));
      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("USE MyDataBase"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No database-name could be found in the given connection-string.")]
    public void GetCreateScript_InvalidConnectionString ()
    {
      var builder = new SqlDatabaseSelectionScriptElementBuilder ("Test1234");

      builder.GetCreateScript();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No database-name could be found in the given connection-string.")]
    public void GetDropScript_InvalidConnectionString ()
    {
      var builder = new SqlDatabaseSelectionScriptElementBuilder ("Test1234");

      builder.GetDropScript ();
    }
  }
}