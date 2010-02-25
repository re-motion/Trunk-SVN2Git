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
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses.Expressions;

namespace Remotion.Data.Linq.UnitTests.Backend.DataObjectModel
{
  [TestFixture]
  public class SubQueryExpressionTest
  {
    [Test]
    public void Initialize_CorrectType ()
    {
      var model = new QueryModel (ExpressionHelper.CreateMainFromClause_Int(), ExpressionHelper.CreateSelectClause());
      var sqe = new SubQueryExpression (model);

      Assert.That (sqe.Type, Is.EqualTo (model.GetOutputDataInfo ().DataType));
    }

    [Test]
    public void Initialize_DontOverwriteExpressionType ()
    {
      Assert.That (Enum.IsDefined (typeof (ExpressionType), new SubQueryExpression (ExpressionHelper.CreateQueryModel_Cook ()).NodeType), Is.False);
    }
  }
}
