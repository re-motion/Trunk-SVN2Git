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
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// Generates a SQL order by clause for a <see cref="SortExpressionDefinition"/>. Identifier quotations are
  /// added via <see cref="RdbmsProvider.DelimitIdentifier"/>
  /// </summary>
  public class SortExpressionSqlGenerator
  {
    private readonly ISqlDialect _dialect;

    public SortExpressionSqlGenerator (ISqlDialect dialect)
    {
      ArgumentUtility.CheckNotNull ("dialect", dialect);

      _dialect = dialect;
    }

    public string GenerateOrderByClauseString (SortExpressionDefinition sortExpression)
    {
      ArgumentUtility.CheckNotNull ("sortExpression", sortExpression);

      var orderByExpressionString = GenerateOrderByExpressionString (sortExpression);
      return "ORDER BY " + orderByExpressionString;
    }

    public string GenerateOrderByExpressionString (SortExpressionDefinition sortExpression)
    {
      ArgumentUtility.CheckNotNull ("sortExpression", sortExpression);

      var sb = new StringBuilder();

      for (int i = 0; i < sortExpression.SortedProperties.Count; ++i)
      {
        if (i > 0)
          sb.Append (", ");

        AppendOrderByExpression (sb, sortExpression.SortedProperties[i]);
      }

      return sb.ToString ();
    }

    public string GenerateColumnListString (SortExpressionDefinition sortExpression)
    {
      ArgumentUtility.CheckNotNull ("sortExpression", sortExpression);

      var sb = new StringBuilder ();

      for (int i = 0; i < sortExpression.SortedProperties.Count; ++i)
      {
        if (i > 0)
          sb.Append (", ");

        AppendColumnName (sb, sortExpression.SortedProperties[i]);
      }

      return sb.ToString ();
      
    }

    private void AppendOrderByExpression (StringBuilder sb, SortedPropertySpecification sortedPropertySpecification)
    {
      AppendColumnName (sb, sortedPropertySpecification);
      if (sortedPropertySpecification.Order == SortOrder.Ascending)
        sb.Append (" ASC");
      else
        sb.Append (" DESC");
    }

    private void AppendColumnName (StringBuilder sb, SortedPropertySpecification sortedPropertySpecification)
    {
      sb.Append (_dialect.DelimitIdentifier (sortedPropertySpecification.PropertyDefinition.StoragePropertyDefinition.Name));
    }
  }
}