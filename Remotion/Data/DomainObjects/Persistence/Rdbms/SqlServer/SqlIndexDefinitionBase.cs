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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer
{
  public abstract class SqlIndexDefinitionBase : IIndexDefinition
  {
    //TODO RM-3882: constants or fields??
    //bool PAD_INDEX
    //int FILLFACTOR
    //bool SORT_IN_TEMPDB
    //bool STATISTICS_NORECOMPUTE
    //bool DROP_EXISTING
    //bool ALLOW_ROW_LOCKS
    //bool ALLOW_PAGE_LOCKS
    //int MAXDOP


    protected SqlIndexDefinitionBase ()
    {
      
    }

    public abstract string IndexName { get; }
    public abstract EntityNameDefinition ObjectName { get; }
    
    public void Accept (IIndexDefinitionVisitor visitor)
    {
      var specificVisitor = visitor as ISqlIndexDefinitionVisitor;
      if (specificVisitor != null)
        Accept (specificVisitor);
    }

    public abstract void Accept (ISqlIndexDefinitionVisitor visitor);
  }
}