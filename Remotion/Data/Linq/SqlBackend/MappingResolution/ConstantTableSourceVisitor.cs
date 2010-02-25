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
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.MappingResolution
{
  /// <summary>
  /// <see cref="ConstantTableSourceVisitor"/> modifies <see cref="AbstractTableSource"/>s.
  /// </summary>
  public class ConstantTableSourceVisitor : ITableSourceVisitor
  {
    private readonly ISqlStatementResolver _resolver;

    public static void ReplaceTableSource (SqlTable sqlTable, ISqlStatementResolver resolver)
    {
      ArgumentUtility.CheckNotNull ("sqlTable", sqlTable);
      ArgumentUtility.CheckNotNull ("resolver", resolver);

      var visitor = new ConstantTableSourceVisitor (resolver);
      sqlTable.TableSource = visitor.VisitTableSource (sqlTable.TableSource);
    }

    protected ConstantTableSourceVisitor (ISqlStatementResolver resolver)
    {
      _resolver = resolver;
    }

    public AbstractTableSource VisitTableSource (AbstractTableSource tableSource)
    {
      ArgumentUtility.CheckNotNull ("tableSource", tableSource);
      return tableSource.Accept (this);
    }

    public AbstractTableSource VisitConstantTableSource (ConstantTableSource tableSource)
    {
      return  _resolver.ResolveConstantTableSource (tableSource);
    }

    public AbstractTableSource VisitSqlTableSource (SqlTableSource tableSource)
    {
      return tableSource;
    }
  }
}