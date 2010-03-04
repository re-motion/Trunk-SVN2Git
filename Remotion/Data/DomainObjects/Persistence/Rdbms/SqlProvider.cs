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
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{

public class SqlProvider : RdbmsProvider
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SqlProvider (RdbmsProviderDefinition definition, IPersistenceTracer persistenceTracer)
    : base (definition, persistenceTracer)
  {
  }

  // methods and properties

  public override string GetColumnsFromSortExpression (string sortExpression)
  {
    CheckDisposed ();

    if (string.IsNullOrEmpty (sortExpression))
      return sortExpression;

    // Collapse all whitespaces (space, tab, carrriage return, ...) to a single space
    string formattedSortExpression = Regex.Replace (sortExpression, @"\s+", " ");

    // Remove any leading or trailing whitespace
    formattedSortExpression = formattedSortExpression.Trim ();

    // Collate is not supported. If collate should be supported later, UnionSelectCommandBuilder must 
    // add collate columns to select list (because it uses a UNION).
    if (formattedSortExpression.IndexOf (" COLLATE", StringComparison.InvariantCultureIgnoreCase) >= 0)
      throw CreateArgumentException ("sortExpression", "Collations cannot be used in sort expressions. Sort expression: '{0}'.", sortExpression);

    // Space after "," must be removed to avoid the following regex " asc| desc" matching a column beginning with "asc" or "desc"
    formattedSortExpression = Regex.Replace (formattedSortExpression, @" ?, ?", ",");
    
    // Remove sort orders
    formattedSortExpression = Regex.Replace (formattedSortExpression, @" asc| desc", string.Empty, RegexOptions.IgnoreCase);
    
    // Add space after each "," for better readability of resulting SQL expression
    formattedSortExpression = Regex.Replace (formattedSortExpression, @" ?, ?", ", ");

    // Remove any leading or trailing whitespace
    return formattedSortExpression.Trim ();
  }

  public override string GetParameterName (string name)
  {
    CheckDisposed ();
    
    if (name.StartsWith ("@"))
      return name;
    else
      return "@" + name;
  }

  protected override TracingDbConnection CreateConnection ()
  {
    CheckDisposed ();
    
    return new TracingDbConnection  (new SqlConnection (), PersistenceTracer);
  }
  
  public new SqlConnection Connection
  {
    get
    {
      CheckDisposed ();
      return (SqlConnection) (base.Connection == null ? null : base.Connection.WrappedInstance);
    }
  }

  public new SqlTransaction Transaction
  {
    get
    {
      CheckDisposed ();
      return (SqlTransaction) (base.Transaction == null ? null : base.Transaction.WrappedInstance);
    }
  }

  public override string DelimitIdentifier (string identifier)
  {
    return "[" + identifier + "]";
  }

  public override string StatementDelimiter
  {
    get { return ";"; }
  }
}
}
