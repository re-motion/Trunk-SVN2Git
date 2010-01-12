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
using System.IO;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Infrastructure;
using System.Linq;

namespace Remotion.Data.DomainObjects.Transport
{
  /// <summary>
  /// Assists in importing data exported by a <see cref="DomainObjectTransporter"/> object. This class is used by
  /// <see cref="DomainObjectTransporter.LoadTransportData(System.IO.Stream,Remotion.Data.DomainObjects.Transport.IImportStrategy)"/> and is usually 
  /// not instantiated by users.
  /// </summary>
  public class DomainObjectImporter
  {
    public static DomainObjectImporter CreateImporterFromStream (Stream stream, IImportStrategy strategy)
    {
      ArgumentUtility.CheckNotNull ("stream", stream);
      ArgumentUtility.CheckNotNull ("strategy", strategy);

      var transportItems = strategy.Import (stream).ToArray();
      return new DomainObjectImporter (transportItems);
    }

    private readonly TransportItem[] _transportItems;

    public DomainObjectImporter (TransportItem[] transportItems)
    {
      ArgumentUtility.CheckNotNull ("transportItems", transportItems);
      _transportItems = transportItems;
    }

    public TransportedDomainObjects GetImportedObjects ()
    {
      ClientTransaction targetTransaction = ClientTransaction.CreateBindingTransaction ();
      List<Tuple<TransportItem, DataContainer>> dataContainerMapping = GetTargetDataContainersForSourceObjects (targetTransaction);

      // grab enlisted objects _before_ properties are synchronized, as synchronizing might load some additional objects
      var transportedObjects = new List<DomainObject> (targetTransaction.GetEnlistedDomainObjects());
      SynchronizeData (dataContainerMapping);

      return new TransportedDomainObjects (targetTransaction, transportedObjects);
    }

    private List<Tuple<TransportItem, DataContainer>> GetTargetDataContainersForSourceObjects (ClientTransaction targetTransaction)
    {
      var result = new List<Tuple<TransportItem, DataContainer>> ();
      if (_transportItems.Length > 0)
      {
        using (targetTransaction.EnterNonDiscardingScope())
        {
          var transportedObjectIDs = GetIDs (_transportItems);

          var existingObjects = targetTransaction.TryGetObjects<DomainObject> (transportedObjectIDs);
          var existingObjectDictionary = existingObjects.Where (obj => obj != null).ToDictionary (obj => obj.ID);

          foreach (TransportItem transportItem in _transportItems)
          {
            DataContainer targetDataContainer = GetTargetDataContainer (transportItem, existingObjectDictionary, targetTransaction);
            result.Add (Tuple.NewTuple (transportItem, targetDataContainer));
          }
        }
      }
      return result;
    }

    private DataContainer GetTargetDataContainer (TransportItem transportItem, Dictionary<ObjectID, DomainObject> existingObjects, ClientTransaction targetTransaction)
    {
      DomainObject existingObject;
      if (existingObjects.TryGetValue (transportItem.ID, out existingObject))
      {
        return targetTransaction.GetDataContainer (existingObject);
      }
      else
      {
        DataContainer targetDataContainer = targetTransaction.CreateNewDataContainer (transportItem.ID);
        var domainObject = targetTransaction.GetObjectForDataContainer (targetDataContainer);
        targetDataContainer.SetDomainObject (domainObject);
        targetTransaction.EnlistDomainObject (domainObject);
        return targetDataContainer;
      }
    }

    private ObjectID[] GetIDs (TransportItem[] items)
    {
      return Array.ConvertAll (items, item => item.ID);
    }

    private void SynchronizeData (IEnumerable<Tuple<TransportItem, DataContainer>> sourceToTargetMapping)
    {
      foreach (Tuple<TransportItem, DataContainer> sourceToTargetContainer in sourceToTargetMapping)
      {
        TransportItem transportItem = sourceToTargetContainer.A;
        DataContainer targetContainer = sourceToTargetContainer.B;
        DomainObject targetObject = targetContainer.DomainObject;
        ClientTransaction targetTransaction = targetContainer.ClientTransaction;

        foreach (KeyValuePair<string, object> sourceProperty in transportItem.Properties)
        {
          PropertyAccessor targetProperty = targetObject.Properties[sourceProperty.Key, targetTransaction];
          switch (targetProperty.PropertyData.Kind)
          {
            case PropertyKind.PropertyValue:
              targetProperty.SetValueWithoutTypeCheck (sourceProperty.Value);
              break;
            case PropertyKind.RelatedObject:
              if (!targetProperty.PropertyData.RelationEndPointDefinition.IsVirtual)
              {
                var relatedObjectID = (ObjectID) sourceProperty.Value;
                DomainObject targetRelatedObject = relatedObjectID != null ? targetTransaction.GetObject (relatedObjectID, false) : null;
                targetProperty.SetValueWithoutTypeCheck (targetRelatedObject);
              }
              break;
          }
        }
      }
    }
  }
}
