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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// <see cref="UnionViewDefinition"/> represents a union view in a relational database.
  /// </summary>
  public class UnionViewDefinition : IEntityDefinition
  {
    private readonly string _viewName;
    private readonly ReadOnlyCollection<IEntityDefinition> _unionedEntities;

    public UnionViewDefinition (string viewName, IEnumerable<IEntityDefinition> unionedEntities)
    {
      ArgumentUtility.CheckNotNull ("unionedEntities", unionedEntities);

      _viewName = viewName;
      _unionedEntities = unionedEntities.ToList().AsReadOnly();
    }

    public string ViewName
    {
      get { return _viewName; }
    }

    public ReadOnlyCollection<IEntityDefinition> UnionedEntities
    {
      get { return _unionedEntities; }
    }

    public string LegacyEntityName
    {
      get { return null; }
    }

    public ReadOnlyCollection<ColumnDefinition> GetColumns ()
    {
      return _unionedEntities.SelectMany (entity => entity.GetColumns()).ToList().AsReadOnly();
    }

    
  }
}