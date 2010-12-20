// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="NullEntityDefinition"/> is used for union scenarios without unioned entities.
  /// </summary>
  public class NullEntityDefinition : IEntityDefinition
  {
    private readonly StorageProviderDefinition _storageProviderDefinition;
    
    public NullEntityDefinition (StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      _storageProviderDefinition = storageProviderDefinition;
    }

    public string LegacyEntityName
    {
      get { return null; }
    }

    public string LegacyViewName
    {
      get { return null; }
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
      get { return null; }
    }

    public ReadOnlyCollection<IColumnDefinition> GetColumns ()
    {
      return new ReadOnlyCollection<IColumnDefinition> (new IColumnDefinition[0]);
    }

    public void Accept (IEntityDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitNullEntityDefinition (this);
    }
  }
}