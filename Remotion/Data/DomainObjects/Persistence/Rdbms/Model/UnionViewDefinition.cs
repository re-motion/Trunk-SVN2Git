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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// <see cref="UnionViewDefinition"/> defines a union view in a relational database.
  /// </summary>
  public class UnionViewDefinition : IEntityDefinition
  {
    private readonly string _viewName;
    private readonly ReadOnlyCollection<IEntityDefinition> _unionedEntities;
    private readonly ReadOnlyCollection<IColumnDefinition> _columns;
    private readonly StorageProviderDefinition _storageProviderDefinition;

    public UnionViewDefinition (StorageProviderDefinition storageProviderDefinition, string viewName, IEnumerable<IEntityDefinition> unionedEntities)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("unionedEntities", unionedEntities);
      ArgumentUtility.CheckNotEmpty ("viewName", viewName);

      _storageProviderDefinition = storageProviderDefinition;
      _viewName = viewName;
      _unionedEntities = unionedEntities.ToList().AsReadOnly();
      _columns = _unionedEntities.SelectMany (entity => entity.GetColumns ()).Distinct().ToList ().AsReadOnly ();
    }

    public string StorageProviderID
    {
      get { return _storageProviderDefinition.Name; }
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
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

    public string LegacyViewName
    {
      get { return _viewName; }
    }

    public ReadOnlyCollection<IColumnDefinition> GetColumns ()
    {
      return _columns;
    }

    public void Accept (IEntityDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitUnionViewDefinition (this);
    }
  }
}