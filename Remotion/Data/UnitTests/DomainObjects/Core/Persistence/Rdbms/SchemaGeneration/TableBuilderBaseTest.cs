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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGenerationTestDomain;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class TableBuilderBaseTest : SchemaGenerationTestBase
  {
    private MockRepository _mocks;
    private TableBuilderBase _stubTableBuilder;
    private IEntityDefinition _orderClassEntityDefinition;
    private IEntityDefinition _customerClassEntityDefinition;
    private ISqlDialect _sqlDialectStub;

    public override void SetUp ()
    {
      base.SetUp();

      _mocks = new MockRepository();
      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _stubTableBuilder = _mocks.StrictMock<TableBuilderBase>(_sqlDialectStub);
      _orderClassEntityDefinition = (IEntityDefinition) MappingConfiguration.ClassDefinitions[typeof (Order)].StorageEntityDefinition;
      _customerClassEntityDefinition = (IEntityDefinition)MappingConfiguration.ClassDefinitions[typeof (Customer)].StorageEntityDefinition;
    }

    [Test]
    public void AddTable ()
    {
      using (_mocks.Ordered())
      {
        _stubTableBuilder
            .Expect (
                mock => mock.AddToCreateTableScript (Arg.Is ((TableDefinition) _orderClassEntityDefinition), Arg<StringBuilder>.Is.Anything))
            .Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Order] ()"));
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.IgnoreArguments();
      }
      _mocks.ReplayAll();

      _stubTableBuilder.AddTable (_orderClassEntityDefinition);
      string actualScript = _stubTableBuilder.GetCreateTableScript();

      _mocks.VerifyAll();
      Assert.AreEqual ("CREATE TABLE [dbo].[Order] ()", actualScript);
    }

    [Test]
    public void AddTableTwice ()
    {
      using (_mocks.Ordered())
      {
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (_orderClassEntityDefinition), Is.NotNull()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Order] ()"));
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.IgnoreArguments();
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (_orderClassEntityDefinition), Is.NotNull()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Order] ()"));
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.IgnoreArguments();
      }
      _mocks.ReplayAll();

      _stubTableBuilder.AddTable (_orderClassEntityDefinition);
      _stubTableBuilder.AddTable (_orderClassEntityDefinition);
      string actualScript = _stubTableBuilder.GetCreateTableScript();

      _mocks.VerifyAll();
      Assert.AreEqual ("CREATE TABLE [dbo].[Order] ()\r\nCREATE TABLE [dbo].[Order] ()", actualScript);
    }

    [Test]
    public void GetCreateTableScript_GetDropTableScript_SeveralTablesAdded ()
    {
      using (_mocks.Ordered ())
      {
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (_customerClassEntityDefinition), Is.NotNull ()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Customer] ()"));
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (_customerClassEntityDefinition), Is.NotNull ()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Customer]"));
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (_orderClassEntityDefinition), Is.NotNull ()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Order] ()"));
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (_orderClassEntityDefinition), Is.NotNull ()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Order]"));
      }
      _mocks.ReplayAll ();

      _stubTableBuilder.AddTable (_customerClassEntityDefinition);
      _stubTableBuilder.AddTable (_orderClassEntityDefinition);
      string actualCreateTableScript = _stubTableBuilder.GetCreateTableScript ();
      string actualDropTableScript = _stubTableBuilder.GetDropTableScript ();

      _mocks.VerifyAll ();
      Assert.AreEqual ("CREATE TABLE [dbo].[Customer] ()\r\nCREATE TABLE [dbo].[Order] ()", actualCreateTableScript);
      Assert.AreEqual ("DROP TABLE [dbo].[Customer]\r\nDROP TABLE [dbo].[Order]", actualDropTableScript);
    }

    [Test]
    public void GetCreateTableScript_GetDropTableScript_NoTableAdded ()
    {
      _mocks.ReplayAll ();

      string actualCreateTableScript = _stubTableBuilder.GetCreateTableScript ();
      string actualDropTableScript = _stubTableBuilder.GetDropTableScript ();

      _mocks.VerifyAll ();
      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    [Test]
    public void GetDropTableScript ()
    {
      using (_mocks.Ordered())
      {
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.IgnoreArguments();
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (_orderClassEntityDefinition), Is.NotNull()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Order]"));
      }
      _mocks.ReplayAll();

      _stubTableBuilder.AddTable (_orderClassEntityDefinition);
      string actualScript = _stubTableBuilder.GetDropTableScript();

      _mocks.VerifyAll();
      Assert.AreEqual ("DROP TABLE [dbo].[Order]", actualScript);
    }

    [Test]
    public void GetDropTableScriptWithMultipleTables ()
    {
      using (_mocks.Ordered())
      {
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.IgnoreArguments();
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (_customerClassEntityDefinition), Is.NotNull()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Customer]"));
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.IgnoreArguments();
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (_orderClassEntityDefinition), Is.NotNull ()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Order]"));
      }
      _mocks.ReplayAll();

      _stubTableBuilder.AddTable (_customerClassEntityDefinition);
      _stubTableBuilder.AddTable (_orderClassEntityDefinition);
      string actualScript = _stubTableBuilder.GetDropTableScript();

      _mocks.VerifyAll();
      Assert.AreEqual ("DROP TABLE [dbo].[Customer]\r\nDROP TABLE [dbo].[Order]", actualScript);
    }

    [Test]
    public void AddTableWithAbstractClass ()
    {
      _stubTableBuilder.AddTable ((IEntityDefinition) MappingConfiguration.ClassDefinitions[typeof(Company)].StorageEntityDefinition);
      _mocks.ReplayAll();

      string actualCreateTableScript = _stubTableBuilder.GetCreateTableScript();
      string actualDropTableScript = _stubTableBuilder.GetDropTableScript();

      _mocks.VerifyAll();
      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    [Test]
    public void AddTableWithDerivedClassWithoutEntityName ()
    {
      _stubTableBuilder.AddTable ((IEntityDefinition) MappingConfiguration.ClassDefinitions[typeof(DerivedClass)].StorageEntityDefinition);
      _mocks.ReplayAll();

      string actualCreateTableScript = _stubTableBuilder.GetCreateTableScript();
      string actualDropTableScript = _stubTableBuilder.GetDropTableScript();

      _mocks.VerifyAll();
      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    [Test]
    public void AddTableWithDerivedClassWithEntityName ()
    {
      _stubTableBuilder.AddTable ((IEntityDefinition) MappingConfiguration.ClassDefinitions[typeof(SecondDerivedClass)].StorageEntityDefinition);
      _mocks.ReplayAll();

      string actualCreateTableScript = _stubTableBuilder.GetCreateTableScript();
      string actualDropTableScript = _stubTableBuilder.GetDropTableScript();

      _mocks.VerifyAll();
      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    [Test]
    public void AddTableWithDerivedOfDerivedClassWithEntityName ()
    {
      _stubTableBuilder.AddTable ((IEntityDefinition) MappingConfiguration.ClassDefinitions[typeof(DerivedOfDerivedClass)].StorageEntityDefinition);
      _mocks.ReplayAll();

      string actualCreateTableScript = _stubTableBuilder.GetCreateTableScript();
      string actualDropTableScript = _stubTableBuilder.GetDropTableScript();

      _mocks.VerifyAll();
      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    private Action<TableDefinition, StringBuilder> CreateAddToDropTableScriptDelegate (string statement)
    {
      return delegate (TableDefinition tableDefinition, StringBuilder stringBuilder) { stringBuilder.Append (statement); };
    }

    private Action<TableDefinition, StringBuilder> CreateAddToCreateTableScriptDelegate (string statement)
    {
      return delegate (TableDefinition tableDefinition, StringBuilder stringBuilder) { stringBuilder.Append (statement); };
    }
  }
}
