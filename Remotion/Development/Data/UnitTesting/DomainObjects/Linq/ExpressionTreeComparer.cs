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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Backend.SqlGeneration;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Development.Data.UnitTesting.DomainObjects.Linq
{
  /// <summary>
  /// The <see cref="ExpressionTreeComparer"/> compares two expressions for as to wether they result in the same SQL statement and parameters.
  /// Use this comparer to unit test Linq expressions and manually build <see cref="Expression"/> trees.
  /// </summary>
  public class ExpressionTreeComparer
  {
    public delegate void AssertThatActualIsEqualToExpected (object actual, object expected);

    private readonly AssertThatActualIsEqualToExpected _assertThatActualIsEqualToExpected;

    public ExpressionTreeComparer (AssertThatActualIsEqualToExpected thatActualIsEqualToExpected)
    {
      ArgumentUtility.CheckNotNull ("thatActualIsEqualToExpected", thatActualIsEqualToExpected);

      _assertThatActualIsEqualToExpected = thatActualIsEqualToExpected;
    }

    public void Compare<T> (IQueryable<T> expected, IQueryable<T> actual)
        where T: DomainObject
    {
      ArgumentUtility.CheckNotNull ("expected", expected);
      ArgumentUtility.CheckNotNull ("actual", actual);

      CommandData expectedCommandData = GetCommandData (expected);
      CommandData actualCommandData = GetCommandData (actual);

      _assertThatActualIsEqualToExpected (actualCommandData.Statement, expectedCommandData.Statement);
      _assertThatActualIsEqualToExpected (actualCommandData.Parameters, expectedCommandData.Parameters);
    }

    private CommandData GetCommandData<T> (IQueryable<T> queryable)
    {
      QueryModel queryModel = MethodCaller.CallFunc<QueryModel> ("GenerateQueryModel", BindingFlags.Instance | BindingFlags.Public)
          .With ((QueryProviderBase) queryable.Provider, queryable.Expression);

      return ((DomainObjectQueryExecutor) ((QueryProviderBase) queryable.Provider).Executor).CreateStatement (queryModel);
    }
  }
}
