/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Transport
{
  /// <summary>
  /// Assists in importing data exported by a <see cref="DomainObjectTransporter"/> object. This class is used by
  /// <see cref="DomainObjectTransporter.LoadTransportData(byte[])"/> and is usually not instantiated by users.
  /// </summary>
  public class DomainObjectImporter
  {
    private readonly TransportItem[] _transportItems;

    public DomainObjectImporter (byte[] data, IImportStrategy importStrategy)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("data", data);
      ArgumentUtility.CheckNotNull ("importStrategy", importStrategy);

      _transportItems = EnumerableUtility.ToArray (importStrategy.Import (data));
    }

    public TransportedDomainObjects GetImportedObjects ()
    {
      ClientTransaction targetTransaction = ClientTransaction.CreateBindingTransaction ();
      List<Tuple<TransportItem, DataContainer>> dataContainerMapping = GetTargetDataContainersForSourceObjects (targetTransaction);

      // grab enlisted objects _before_ properties are synchronized, as synchronizing might load some additional objects
      var transportedObjects = new List<DomainObject> (targetTransaction.EnlistedDomainObjects);
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
          ObjectID[] transportedObjectIDs = GetIDs (_transportItems);
          ObjectList<DomainObject> existingObjects = targetTransaction.TryGetObjects<DomainObject> (transportedObjectIDs);

          foreach (TransportItem transportItem in _transportItems)
          {
            DataContainer targetDataContainer = GetTargetDataContainer(transportItem, existingObjects, targetTransaction);
            result.Add (Tuple.NewTuple (transportItem, targetDataContainer));
          }
        }
      }
      return result;
    }

    private DataContainer GetTargetDataContainer (TransportItem transportItem, ObjectList<DomainObject> existingObjects, ClientTransaction targetTransaction)
    {
      DomainObject existingObject = existingObjects[transportItem.ID];
      if (existingObject != null)
        return targetTransaction.GetDataContainer(existingObject);
      else
      {
        DataContainer targetDataContainer = targetTransaction.CreateNewDataContainer (transportItem.ID);
        targetTransaction.EnlistDomainObject (targetDataContainer.DomainObject);
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
                DomainObject targetRelatedObject = relatedObjectID != null ? targetTransaction.GetObject (relatedObjectID) : null;
                targetProperty.SetValueWithoutTypeCheck (targetRelatedObject);
              }
              break;
          }
        }
      }
    }
  }
}
