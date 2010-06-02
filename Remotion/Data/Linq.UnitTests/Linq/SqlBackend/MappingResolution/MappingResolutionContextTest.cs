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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.MappingResolution
{
  [TestFixture]
  public class MappingResolutionContextTest
  {
    private SqlEntityExpression _entityExpression;
    private MappingResolutionContext _context;
    private SqlTable _sqlTable;

    [SetUp]
    public void SetUp ()
    {
      _context = new MappingResolutionContext();
      _entityExpression = new SqlEntityDefinitionExpression (typeof (Cook), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));
      _sqlTable = new SqlTable (new ResolvedSimpleTableInfo (typeof (Cook), "CookTable", "c"));
    }

    [Test]
    public void AddMapping_EntityExists ()
    {
      _context.AddSqlEntityMapping (_entityExpression, _sqlTable);

      Assert.That (_context.GetSqlTableForEntityExpression (_entityExpression), Is.SameAs (_sqlTable));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No associated table found for entity 'Cook'.")]
    public void GetSqlTableForEntityExpression_EntityDoesNotExist ()
    {
      _context.GetSqlTableForEntityExpression (_entityExpression);
    }

    [Test]
    public void UpdateEntityAndAddMapping ()
    {
      var entity = SqlStatementModelObjectMother.CreateSqlEntityDefinitionExpression (typeof (Cook));
      var sqlTable = SqlStatementModelObjectMother.CreateSqlTable (typeof (Cook));
      _context.AddSqlEntityMapping (entity, sqlTable);

      var result = (SqlEntityDefinitionExpression) _context.UpdateEntityAndAddMapping (entity, entity.Type, "newAlias", "newName");

      Assert.That (result.TableAlias, Is.EqualTo ("newAlias"));
      Assert.That (result.Name, Is.EqualTo ("newName"));
      Assert.That (_context.GetSqlTableForEntityExpression (result), Is.SameAs (sqlTable));
    }
  }
}