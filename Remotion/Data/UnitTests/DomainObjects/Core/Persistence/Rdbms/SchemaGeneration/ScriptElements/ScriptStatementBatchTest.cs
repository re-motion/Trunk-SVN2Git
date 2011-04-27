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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration.ScriptElements
{
  [TestFixture]
  public class ScriptStatementBatchTest
  {
    private ScriptStatementBatch _statementBatch;
    private ISqlDialect _sqlDialectMock;

    [SetUp]
    public void SetUp ()
    {
      _statementBatch = new ScriptStatementBatch();
      _sqlDialectMock = MockRepository.GenerateStrictMock<ISqlDialect>();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_statementBatch.Statements, Is.Empty);
    }

    [Test]
    public void AppendToScript_OneStatement ()
    {
      var statement = new ScriptStatement ("Statement 1");
      _statementBatch.AddStatement (statement);
      var scriptStatement = new ScriptStatement ("Something ...");
      var script = new List<ScriptStatement> { scriptStatement };

      _sqlDialectMock
          .Expect (mock => mock.AddBatchSeparatorIfNeeded (script))
          .WhenCalled (mi => script.Add (new ScriptStatement ("GO")))
          .Repeat.Twice();
      _sqlDialectMock.Replay ();

      _statementBatch.AppendToScript (script, _sqlDialectMock);

      _sqlDialectMock.VerifyAllExpectations ();
      Assert.That (script.Count, Is.EqualTo(4));
      Assert.That (script[0], Is.SameAs(scriptStatement));
      Assert.That (script[1].Statement, Is.EqualTo("GO"));
      Assert.That (script[2], Is.SameAs(statement));
      Assert.That (script[3].Statement, Is.EqualTo ("GO"));
    }

    [Test]
    public void AppendToScript_SeveralStatements ()
    {
      var statement1 = new ScriptStatement ("Statement 1");
      var statement2 = new ScriptStatement ("Statement 2");
      var statement3 = new ScriptStatement ("Statement 3");
      _statementBatch.AddStatement (statement1);
      _statementBatch.AddStatement (statement2);
      _statementBatch.AddStatement (statement3);
      var scriptStatement = new ScriptStatement ("Something ...");
      var script = new List<ScriptStatement> { scriptStatement };

      _sqlDialectMock
          .Expect (mock => mock.AddBatchSeparatorIfNeeded (script))
          .WhenCalled (mi => script.Add (new ScriptStatement ("GO")))
          .Repeat.Twice ();
      _sqlDialectMock.Replay ();

      _statementBatch.AppendToScript (script, _sqlDialectMock);

      _sqlDialectMock.VerifyAllExpectations ();
      Assert.That (script.Count, Is.EqualTo (6));
      Assert.That (script[0], Is.SameAs (scriptStatement));
      Assert.That (script[1].Statement, Is.EqualTo ("GO"));
      Assert.That (script[2], Is.SameAs (statement1));
      Assert.That (script[3], Is.SameAs (statement2));
      Assert.That (script[4], Is.SameAs (statement3));
      Assert.That (script[5].Statement, Is.EqualTo ("GO"));
    }
    
    [Test]
    public void AppendToScript_NoStatements ()
    {
      var script = new List<ScriptStatement>();

      _sqlDialectMock.Expect (mock => mock.AddBatchSeparatorIfNeeded (script)).Repeat.Twice();
      _sqlDialectMock.Replay();

      _statementBatch.AppendToScript (script, _sqlDialectMock);

      _sqlDialectMock.VerifyAllExpectations();
      Assert.That (script, Is.Empty);
    }

    [Test]
    public void AddStatement_OneStatement ()
    {
      var statement = new ScriptStatement ("Test");

      _statementBatch.AddStatement (statement);

      Assert.That (_statementBatch.Statements, Is.EqualTo (new[]{statement}));
    }

    [Test]
    public void AddStatement_SeveralStatements ()
    {
      var statement1 = new ScriptStatement ("Test1");
      var statement2 = new ScriptStatement ("Test2");
      var statement3 = new ScriptStatement ("Test3");
      
      _statementBatch.AddStatement (statement1);
      _statementBatch.AddStatement (statement2);
      _statementBatch.AddStatement (statement3);
      
      Assert.That (_statementBatch.Statements, Is.EqualTo (new[] { statement1, statement2, statement3 }));
    }
  }
}