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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="NullEntityDefinition"/> represents a non-existing entity.
  /// </summary>
  public class NullEntityDefinition : IEntityDefinition
  {
    private readonly StorageProviderDefinition _storageProviderDefinition;

    public NullEntityDefinition (StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      _storageProviderDefinition = storageProviderDefinition;
    }

    public string StorageProviderID
    {
      get { return _storageProviderDefinition.Name; }
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public EntityNameDefinition ViewName
    {
      get { return null; }
    }

    public ColumnDefinition IDColumn
    {
      get { return null; }
    }

    public ColumnDefinition ClassIDColumn
    {
      get { return null; }
    }

    public ColumnDefinition TimestampColumn
    {
      get { return null; }
    }

    public IEnumerable<ColumnDefinition> DataColumns
    {
      get { return new ColumnDefinition[0]; }
    }

    public ObjectIDStoragePropertyDefinition ObjectIDProperty
    {
      get { return null; }
    }

    public IRdbmsStoragePropertyDefinition TimestampProperty
    {
      get { return null; }
    }

    public IEnumerable<IRdbmsStoragePropertyDefinition> DataProperties
    {
      get { return new IRdbmsStoragePropertyDefinition[0]; }
    }

    public IEnumerable<IRdbmsStoragePropertyDefinition> GetAllProperties ()
    {
      return new IRdbmsStoragePropertyDefinition[0];
    }

    public IEnumerable<ColumnDefinition> GetAllColumns ()
    {
      return new ColumnDefinition[0];
    }

    public ReadOnlyCollection<IIndexDefinition> Indexes
    {
      get { return Array.AsReadOnly(new IIndexDefinition[0]);  }
    }

    public ReadOnlyCollection<EntityNameDefinition> Synonyms
    {
      get { return Array.AsReadOnly(new EntityNameDefinition[0]); }
    }

    public void Accept (IEntityDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitNullEntityDefinition (this);
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}