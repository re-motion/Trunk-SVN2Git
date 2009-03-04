// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Data;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class DataContainerLoader
  {
    private readonly RdbmsProvider _provider;
    private readonly IDataContainerLoaderHelper _loaderHelper;

    public DataContainerLoader (RdbmsProvider provider)
        : this (new DataContainerLoaderHelper(), provider)
    {
    }

    public DataContainerLoader (IDataContainerLoaderHelper loaderHelper, RdbmsProvider provider)
    {
      ArgumentUtility.CheckNotNull ("loaderHelper", loaderHelper);
      ArgumentUtility.CheckNotNull ("provider", provider);

      _provider = provider;
      _loaderHelper = loaderHelper;
    }

    public RdbmsProvider Provider
    {
      get { return _provider; }
    }

    public DataContainer LoadDataContainerFromID (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      SelectCommandBuilder commandBuilder = SelectCommandBuilder.CreateForIDLookup (Provider, "*", id.ClassDefinition.GetEntityName (), id);
      using (IDbCommand command = commandBuilder.Create ())
      {
        using (IDataReader reader = Provider.ExecuteReader (command, CommandBehavior.SingleRow))
        {
          DataContainerFactory dataContainerFactory = new DataContainerFactory (Provider, reader);
          return dataContainerFactory.CreateDataContainer ();
        }
      }
    }

    public DataContainerCollection LoadDataContainersFromIDs (IEnumerable<ObjectID> orderedIDs)
    {
      Dictionary<string, List<ObjectID>> objectIDsPerEntityName = GetObjectIDsPerEntityName (orderedIDs);
      DataContainerCollection allDataContainers = new DataContainerCollection();
      foreach (string entityName in objectIDsPerEntityName.Keys)
        allDataContainers = DataContainerCollection.Join (allDataContainers, GetDataContainers (entityName, objectIDsPerEntityName[entityName]));

      return GetOrderedDataContainers (orderedIDs, allDataContainers);
    }

    private DataContainerCollection GetOrderedDataContainers (IEnumerable<ObjectID> objectIDsInCorrectOrder, DataContainerCollection unorderedDataContainers)
    {
      DataContainerCollection orderedDataContainers = new DataContainerCollection ();
      foreach (ObjectID objectID in objectIDsInCorrectOrder)
      {
        DataContainer dataContainer = unorderedDataContainers[objectID];
        if (dataContainer != null)
          orderedDataContainers.Add (dataContainer);
      }

      return orderedDataContainers;
    }

    private DataContainerCollection GetDataContainers (string entityName, IEnumerable<ObjectID> objectIDs)
    {
      SelectCommandBuilder commandBuilder = _loaderHelper.GetSelectCommandBuilder(Provider, entityName, objectIDs);

      using (IDbCommand command = commandBuilder.Create ())
      {
        using (IDataReader reader = Provider.ExecuteReader (command, CommandBehavior.SingleResult))
        {
          var dataContainerFactory = new DataContainerFactory (Provider, reader);
          return new DataContainerCollection  (dataContainerFactory.CreateCollection (false), false);
        }
      }
    }

    private Dictionary<string, List<ObjectID>> GetObjectIDsPerEntityName (IEnumerable<ObjectID> objectIDsInCorrectOrder)
    {
      Dictionary<string, List<ObjectID>> objectIDsPerEntityName = new Dictionary<string, List<ObjectID>> ();
      foreach (ObjectID objectID in objectIDsInCorrectOrder)
      {
        string entityName = objectID.ClassDefinition.GetEntityName ();
        if (!objectIDsPerEntityName.ContainsKey (entityName))
          objectIDsPerEntityName.Add (entityName, new List<ObjectID> ());

        objectIDsPerEntityName[entityName].Add (objectID);
      }
      return objectIDsPerEntityName;
    }

    public DataContainer[] LoadDataContainersFromCommandBuilder (CommandBuilder commandBuilder, bool allowNulls)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);

      using (IDbCommand command = commandBuilder.Create ())
      {
        using (IDataReader dataReader = Provider.ExecuteReader (command, CommandBehavior.SingleResult))
        {
          DataContainerFactory dataContainerFactory = new DataContainerFactory (Provider, dataReader);
          return dataContainerFactory.CreateCollection (allowNulls);
        }
      }
    }

    public DataContainerCollection LoadDataContainersByRelatedID (ClassDefinition classDefinition, string propertyName, ObjectID relatedID)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);

      PropertyDefinition propertyDefinition = classDefinition.GetPropertyDefinition (propertyName);
      if (propertyDefinition == null)
        throw Provider.CreateRdbmsProviderException ("Class '{0}' does not contain property '{1}'.", classDefinition.ID, propertyName);

      if (propertyDefinition.StorageClass != StorageClass.Persistent)
        return new DataContainerCollection ();
      else if (classDefinition.GetEntityName () != null)
      {
        SelectCommandBuilder commandBuilder = _loaderHelper.GetSelectCommandBuilderForRelatedIDLookup (
            Provider, classDefinition.GetEntityName(), propertyDefinition, relatedID);
        return new DataContainerCollection (Provider.LoadDataContainers (commandBuilder, false), false);
      }
      else
      {
        ConcreteTableInheritanceRelationLoader loader = _loaderHelper.GetConcreteTableInheritanceRelationLoader (
            Provider, classDefinition, propertyDefinition, relatedID);

        return loader.LoadDataContainers ();
      }
    }
  }
}
