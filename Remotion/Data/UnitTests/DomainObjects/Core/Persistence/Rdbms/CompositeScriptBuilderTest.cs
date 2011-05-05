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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class CompositeScriptBuilderTest : SchemaGenerationTestBase
  {
    private IScriptBuilder _builder1Mock;
    private IScriptBuilder _builder2Mock;
    private CompositeScriptBuilder _builder;
    private ScriptElementCollection _fakeResultCollection1;
    private ScriptElementCollection _fakeResultCollection2;
    private ScriptStatement _fakeStatement1;
    private ScriptStatement _fakeStatement2;
    private ScriptStatement _fakeStatement3;

    public override void SetUp ()
    {
      base.SetUp ();

      _builder1Mock = MockRepository.GenerateStrictMock<IScriptBuilder>();
      _builder2Mock = MockRepository.GenerateStrictMock<IScriptBuilder>();

      _builder = new CompositeScriptBuilder (SchemaGenerationFirstStorageProviderDefinition, SqlDialect.Instance, _builder1Mock, _builder2Mock);

      _fakeStatement1 = new ScriptStatement ("Fake1");
      _fakeStatement2 = new ScriptStatement ("Fake2");
      _fakeStatement3 = new ScriptStatement ("Fake3");

      _fakeResultCollection1 = new ScriptElementCollection();
      _fakeResultCollection1.AddElement (_fakeStatement1);

      _fakeResultCollection2 = new ScriptElementCollection ();
      _fakeResultCollection2.AddElement (_fakeStatement2);
      _fakeResultCollection2.AddElement (_fakeStatement3);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_builder.RdbmsProviderDefinition, Is.SameAs (SchemaGenerationFirstStorageProviderDefinition));
      Assert.That (_builder.SqlDialect, Is.SameAs (SqlDialect.Instance));
      Assert.That (_builder.ScriptBuilders, Is.EqualTo(new[]{_builder1Mock, _builder2Mock}));
    }

    [Test]
    public void GetCreateScript ()
    {
      var entityDefinition1 = MockRepository.GenerateStub<IEntityDefinition> ();
      var entityDefinition2 = MockRepository.GenerateStub<IEntityDefinition> ();

      _builder1Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _builder1Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _builder1Mock.Expect (mock => mock.GetCreateScript ()).Return (_fakeResultCollection1);
      _builder1Mock.Replay ();

      _builder2Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _builder2Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _builder2Mock.Expect (mock => mock.GetCreateScript ()).Return (_fakeResultCollection2);
      _builder2Mock.Replay ();

      _builder.AddEntityDefinition (entityDefinition1);
      _builder.AddEntityDefinition (entityDefinition2);

      var result = _builder.GetCreateScript ();

      _builder1Mock.VerifyAllExpectations ();
      _builder2Mock.VerifyAllExpectations ();
      
      Assert.That (result.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) result.Elements[0]).Statement, Is.EqualTo ("USE SchemaGenerationTestDomain1"));
      Assert.That (result.Elements[1], Is.SameAs(_fakeStatement1));
      Assert.That (result.Elements[2], Is.SameAs (_fakeStatement2));
      Assert.That (result.Elements[3], Is.SameAs (_fakeStatement3));
    }
    
    [Test]
    public void GetDropScript ()
    {
      var entityDefinition1 = MockRepository.GenerateStub<IEntityDefinition> ();
      var entityDefinition2 = MockRepository.GenerateStub<IEntityDefinition> ();

      _builder1Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _builder1Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _builder1Mock.Expect (mock => mock.GetDropScript ()).Return (_fakeResultCollection1);
      _builder1Mock.Replay ();

      _builder2Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _builder2Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _builder2Mock.Expect (mock => mock.GetDropScript ()).Return (_fakeResultCollection2);
      _builder2Mock.Replay ();

      _builder.AddEntityDefinition (entityDefinition1);
      _builder.AddEntityDefinition (entityDefinition2);

      var result = _builder.GetDropScript ();

      _builder1Mock.VerifyAllExpectations ();
      _builder2Mock.VerifyAllExpectations ();

      Assert.That (result.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) result.Elements[0]).Statement, Is.EqualTo ("USE SchemaGenerationTestDomain1"));
      Assert.That (result.Elements[1], Is.SameAs (_fakeStatement2));
      Assert.That (result.Elements[2], Is.SameAs (_fakeStatement3));
      Assert.That (result.Elements[3], Is.SameAs (_fakeStatement1));
    }

  }
}