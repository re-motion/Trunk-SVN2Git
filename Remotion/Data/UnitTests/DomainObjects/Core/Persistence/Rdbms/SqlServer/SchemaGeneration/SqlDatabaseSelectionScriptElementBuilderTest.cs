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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlDatabaseSelectionScriptElementBuilderTest : SchemaGenerationTestBase
  {
    private IScriptBuilder _innerScriptBuilderMock;
    private SqlDatabaseSelectionScriptElementBuilder _builder;

    public override void SetUp ()
    {
      base.SetUp ();

      var connectionString = "Data Source=myServerAddress;Initial Catalog=MyDataBase;User Id=myUsername;Password=myPassword;";
      _innerScriptBuilderMock = MockRepository.GenerateMock<IScriptBuilder> ();
      _builder = new SqlDatabaseSelectionScriptElementBuilder (_innerScriptBuilderMock, connectionString);
    }

    [Test]
    public void AddEntityDefinition ()
    {
      var entityDefinitionStub = MockRepository.GenerateStub<IEntityDefinition>();

      _innerScriptBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinitionStub));
      _innerScriptBuilderMock.Replay();

      _builder.AddEntityDefinition (entityDefinitionStub);

      _innerScriptBuilderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetCreateScript_GetDropScript_ValidConnectionString ()
    {
      var statement1 = new ScriptStatement ("Test1");
      var statement2 = new ScriptStatement ("Test2");
      var fakeCreateResult = new ScriptElementCollection ();
      fakeCreateResult.AddElement (statement1);
      fakeCreateResult.AddElement (statement2);
      var fakeDropResult = new ScriptElementCollection();
      fakeDropResult.AddElement (statement2);
      fakeDropResult.AddElement (statement1);

      _innerScriptBuilderMock.Expect (mock => mock.GetCreateScript()).Return(fakeCreateResult);
      _innerScriptBuilderMock.Expect (mock => mock.GetDropScript ()).Return (fakeDropResult);

      var createScriptResult = _builder.GetCreateScript();
      var dropScriptResult = _builder.GetDropScript();

      Assert.That (((ScriptElementCollection) createScriptResult).Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) createScriptResult).Elements[0]).Statement, Is.EqualTo ("USE MyDataBase"));
      Assert.That (((ScriptElementCollection) createScriptResult).Elements[1], Is.SameAs(fakeCreateResult));
      Assert.That (((ScriptElementCollection) dropScriptResult).Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) dropScriptResult).Elements[0]).Statement, Is.EqualTo ("USE MyDataBase"));
      Assert.That (((ScriptElementCollection) dropScriptResult).Elements[1], Is.SameAs (fakeDropResult));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No database-name could be found in the given connection-string.")]
    public void GetCreateScript_InvalidConnectionString ()
    {
      var builder = new SqlDatabaseSelectionScriptElementBuilder (_innerScriptBuilderMock, "Test1234");

      builder.GetCreateScript();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No database-name could be found in the given connection-string.")]
    public void GetDropScript_InvalidConnectionString ()
    {
      var builder = new SqlDatabaseSelectionScriptElementBuilder (_innerScriptBuilderMock, "Test1234");

      builder.GetDropScript ();
    }
  }
}