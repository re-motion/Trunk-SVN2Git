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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  // TODO Review 3869: Implement IEntityDefinition (most members abstract)
  /// <summary>
  /// <see cref="EntityDefinitionBase"/> is the base-class for all entity definitions.
  /// </summary>
  public abstract class EntityDefinitionBase
  {
    private readonly EntityNameDefinition _viewName;
    private readonly ReadOnlyCollection<IColumnDefinition> _columns;
    private readonly ReadOnlyCollection<EntityNameDefinition> _synonyms;

    protected EntityDefinitionBase (
        EntityNameDefinition viewName, 
        IEnumerable<IColumnDefinition> columns, 
        IEnumerable<EntityNameDefinition> synonyms)
    {
      ArgumentUtility.CheckNotNull ("columns", columns);
      ArgumentUtility.CheckNotNull ("synonyms", synonyms);

      _viewName = viewName;
      _columns = columns.ToList().AsReadOnly();
      _synonyms = synonyms.ToList().AsReadOnly();
    }

    public EntityNameDefinition ViewName
    {
      get { return _viewName; }
    }

    public ReadOnlyCollection<IColumnDefinition> Columns
    {
      get { return _columns; }
    }

    public ReadOnlyCollection<EntityNameDefinition> Synonyms
    {
      get { return _synonyms; }
    }
  }
}