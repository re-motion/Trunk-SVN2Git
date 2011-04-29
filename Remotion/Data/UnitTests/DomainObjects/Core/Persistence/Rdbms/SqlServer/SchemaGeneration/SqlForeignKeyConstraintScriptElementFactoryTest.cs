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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlForeignKeyConstraintScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private SqlForeignKeyConstraintScriptElementFactory _factory;
    private SimpleColumnDefinition _column1;
    private SimpleColumnDefinition _column2;
    private ForeignKeyConstraintDefinition _constraint1;
    private ForeignKeyConstraintDefinition _constraint2;
    private EntityNameDefinition _table1;
    private EntityNameDefinition _table2;

    public override void SetUp ()
    {
      base.SetUp();

      _factory = new SqlForeignKeyConstraintScriptElementFactory();

      _column1 = new SimpleColumnDefinition ("Column1", typeof (string), "varchar", false, true);
      _column2 = new SimpleColumnDefinition ("Column2", typeof (int), "integer", true, false);

      _table1 = new EntityNameDefinition (null, "TableName1");
      _table2 = new EntityNameDefinition ("SchemaName", "TableName2");

      _constraint1 = new ForeignKeyConstraintDefinition ("FK1", _table1, new[] { _column1 }, new[] { _column2 });
      _constraint2 = new ForeignKeyConstraintDefinition ("FK2", _table2, new[] { _column1, _column2 }, new[] { _column2, _column1 });
    }
    
    [Test]
    public void GetCreateElement_DefaultSchema ()
    {
      var result = _factory.GetCreateElement (new Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition> (_constraint1, _table1));

      var expectedResult = 
        "ALTER TABLE [dbo].[TableName1] ADD\r\n"
       +"  CONSTRAINT [FK1] FOREIGN KEY ([Column2]) REFERENCES [dbo].[TableName1] ([Column1])";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetCreateElement_CustomSchema ()
    {
      var result = _factory.GetCreateElement (new Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition> (_constraint2, _table2));

      var expectedResult =
        "ALTER TABLE [SchemaName].[TableName2] ADD\r\n"
       + "  CONSTRAINT [FK2] FOREIGN KEY ([Column2], [Column1]) REFERENCES [SchemaName].[TableName2] ([Column1], [Column2])";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_DefaultSchema ()
    {
      var result = _factory.GetDropElement (new Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition> (_constraint1, _table1));

      var expectedResult =
        "IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id where fk.type = 'F' "
        +"AND fk.name = 'FK1' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableName1')\r\n"
        +"  ALTER TABLE [dbo].[TableName1] DROP CONSTRAINT FK1";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_CustomSchema ()
    {
      var result = _factory.GetDropElement (new Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition> (_constraint2, _table2));

      var expectedResult = 
        "IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id where fk.type = 'F' "
        +"AND fk.name = 'FK2' AND schema_name (t.schema_id) = 'SchemaName' AND t.name = 'TableName2')\r\n"
        +"  ALTER TABLE [SchemaName].[TableName2] DROP CONSTRAINT FK2";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }
  }
}