﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Reflection;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  ///  Extends the <see cref="SqlGeneratingOuterSelectExpressionVisitor"/> with support for addtional features, e.g., selecting <see langword="null" /> 
  /// <see cref="ObjectID"/> values.
  /// </summary>
  public class ExtendedSqlGeneratingOuterSelectExpressionVisitor : SqlGeneratingOuterSelectExpressionVisitor
  {
    private static readonly MethodInfo s_getObjectIDOrNullMethod = MemberInfoFromExpressionUtility.GetMethod (() => GetObjectIDOrNull (null, null));
    private static readonly ConstructorInfo s_objectIDConstructor = MemberInfoFromExpressionUtility.GetConstructor (() => new ObjectID ((string) null, null));

    public static new void GenerateSql (Expression expression, ISqlCommandBuilder commandBuilder, ISqlGenerationStage stage)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      ArgumentUtility.CheckNotNull ("stage", stage);

      EnsureNoCollectionExpression (expression);

      var visitor = new ExtendedSqlGeneratingOuterSelectExpressionVisitor (commandBuilder, stage);
      visitor.VisitExpression (expression);
    }

    public static ObjectID GetObjectIDOrNull (string classID, object value)
    {
      if (value == null)
        return null;

      return new ObjectID (classID, value);
    }

    protected ExtendedSqlGeneratingOuterSelectExpressionVisitor (ISqlCommandBuilder commandBuilder, ISqlGenerationStage stage)
        : base(commandBuilder, stage)
    {
    }

    protected override Expression VisitNewExpression (NewExpression expression)
    {
      var baseResult = base.VisitNewExpression (expression);
      if (expression.Type == typeof (ObjectID))
      {
        // If the NewExpression represents a selected ObjectID, we want to return null if the ID value is null. Therefore, change the projection
        // to use the GetObjectIDOrNull method.

        var originalObjectIDProjection = (NewExpression) CommandBuilder.GetInMemoryProjectionBody();
        Assertion.IsNotNull (originalObjectIDProjection);
        Assertion.IsTrue (originalObjectIDProjection.Constructor.Equals (s_objectIDConstructor));

        var nullSafeObjectIDProjection = Expression.Call (
            s_getObjectIDOrNullMethod, 
            originalObjectIDProjection.Arguments[0], 
            originalObjectIDProjection.Arguments[1]);
        CommandBuilder.SetInMemoryProjectionBody (nullSafeObjectIDProjection);
      }

      return baseResult;
    }
  }
}