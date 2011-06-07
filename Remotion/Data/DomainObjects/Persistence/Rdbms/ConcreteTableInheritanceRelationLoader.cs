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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class ConcreteTableInheritanceRelationLoader
  {
    private readonly RdbmsProvider _provider;
    private readonly ClassDefinition _classDefinition;
    private readonly PropertyDefinition _propertyDefinition;
    private readonly ObjectID _relatedID;
    private readonly DataContainerLoader _dataContainerLoader;
    private readonly ObjectIDLoader _objectIDLoader;
    private readonly IStorageNameProvider _storageNameProvider;

    public ConcreteTableInheritanceRelationLoader (
        RdbmsProvider provider,
        IStorageNameProvider storageNameProvider,
        ClassDefinition classDefinition,
        PropertyDefinition propertyDefinition,
        ObjectID relatedID)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);

      _provider = provider;
      _storageNameProvider = storageNameProvider;
      _classDefinition = classDefinition;
      _propertyDefinition = propertyDefinition;
      _relatedID = relatedID;
      _dataContainerLoader = new DataContainerLoader (_provider);
      _objectIDLoader = new ObjectIDLoader (_provider);
    }

    public RdbmsProvider Provider
    {
      get { return _provider; }
    }

    public DataContainerCollection LoadDataContainers ()
    {
      List<ObjectID> objectIDsInCorrectOrder = GetObjectIDsInCorrectOrder();
      if (objectIDsInCorrectOrder.Count == 0)
        return new DataContainerCollection();

      return _dataContainerLoader.LoadDataContainersFromIDs (objectIDsInCorrectOrder);
    }

    private List<ObjectID> GetObjectIDsInCorrectOrder ()
    {
      var builder = UnionSelectDbCommandBuilder.CreateForRelatedIDLookup (
          _provider, _storageNameProvider, _classDefinition, _propertyDefinition, _relatedID, _provider.SqlDialect, _provider);

      return _objectIDLoader.LoadObjectIDsFromCommandBuilder (builder);
    }
  }
}