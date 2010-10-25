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
using System.Data.SqlClient;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{

public class SqlProvider : RdbmsProvider
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SqlProvider (RdbmsProviderDefinition definition, IPersistenceListener persistenceListener)
    : base (definition, SqlDialect.Instance, persistenceListener)
  {
  }

  // methods and properties

  protected override TracingDbConnection CreateConnection ()
  {
    CheckDisposed ();
    
    return new TracingDbConnection  (new SqlConnection (), PersistenceListener);
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

  
}
}
