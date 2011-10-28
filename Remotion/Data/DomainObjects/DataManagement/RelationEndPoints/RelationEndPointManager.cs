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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Manages the <see cref="IRelationEndPoint"/> instances loaded into a <see cref="ClientTransaction"/>, encapsulating the details of registration
  /// and loading.
  /// </summary>
  public class RelationEndPointManager : IRelationEndPointManager
  {
    public static IRelationEndPoint CreateNullEndPoint (ClientTransaction clientTransaction, IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);

      if (endPointDefinition.Cardinality == CardinalityType.Many)
        return new NullCollectionEndPoint (clientTransaction, endPointDefinition);
      else if (endPointDefinition.IsVirtual)
        return new NullVirtualObjectEndPoint (clientTransaction, endPointDefinition);
      else
        return new NullRealObjectEndPoint (clientTransaction, endPointDefinition);
    }

    private readonly ClientTransaction _clientTransaction;
    private readonly ILazyLoader _lazyLoader;
    private readonly IRelationEndPointFactory _endPointFactory;
    
    private readonly RelationEndPointMap _map;
    private readonly IRelationEndPointRegistrationAgent _registrationAgent;
    private readonly DelegatingDataContainerEndPointsRegistrationAgent _dataContainerEndPointsRegistrationAgent;

    public RelationEndPointManager (
        ClientTransaction clientTransaction,
        ILazyLoader lazyLoader,
        IRelationEndPointFactory endPointFactory,
        IRelationEndPointRegistrationAgent registrationAgent)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("endPointFactory", endPointFactory);
      ArgumentUtility.CheckNotNull ("registrationAgent", registrationAgent);

      _clientTransaction = clientTransaction;
      _lazyLoader = lazyLoader;
      _endPointFactory = endPointFactory;
      _registrationAgent = registrationAgent;
      _dataContainerEndPointsRegistrationAgent = new DelegatingDataContainerEndPointsRegistrationAgent (endPointFactory, registrationAgent);

      _map = new RelationEndPointMap (_clientTransaction);
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public ILazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public IRelationEndPointFactory EndPointFactory
    {
      get { return _endPointFactory; }
    }

    public IRelationEndPointRegistrationAgent RegistrationAgent
    {
      get { return _registrationAgent; }
    }

    public IDataContainerEndPointsRegistrationAgent DataContainerEndPointsRegistrationAgent
    {
      get { return _dataContainerEndPointsRegistrationAgent; }
    }

    public IRelationEndPointMapReadOnlyView RelationEndPoints
    {
      get { return _map; }
    }

    public void RegisterEndPointsForDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      _dataContainerEndPointsRegistrationAgent.RegisterEndPoints (dataContainer, _map);
    }

    // When unregistering a DataContainer, its real end-points are always unregistered. This may indirectly unregister opposite virtual end-points.
    // If the DataContainer is New, the virtual end-points are unregistered as well.
    public IDataManagementCommand CreateUnregisterCommandForDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      return _dataContainerEndPointsRegistrationAgent.CreateUnregisterEndPointsCommand (dataContainer, _map);
    }

    public IDataManagementCommand CreateUnloadVirtualEndPointsCommand (IEnumerable<RelationEndPointID> endPointIDs)
    {
      ArgumentUtility.CheckNotNull ("endPointIDs", endPointIDs);

      var virtualEndPoints = new List<IVirtualEndPoint>();
      var exceptionCommands = new List<ExceptionCommand>();
      foreach (var endPointID in endPointIDs)
      {
        if (!endPointID.Definition.IsVirtual)
        {
          var message = string.Format ("The given end point ID '{0}' does not denote a virtual end-point.", endPointID);
          throw new ArgumentException (message, "endPointIDs");
        }

        var virtualEndPoint = (IVirtualEndPoint) GetRelationEndPointWithoutLoading (endPointID);
        if (virtualEndPoint != null)
        {
          if (!virtualEndPoint.CanBeMarkedIncomplete)
          {
            var message = string.Format ("The end point with ID '{0}' has been changed. Changed end points cannot be unloaded.", endPointID);
            exceptionCommands.Add (new ExceptionCommand (new InvalidOperationException (message)));
          }
          else
          {
            virtualEndPoints.Add (virtualEndPoint);
          }
        }
      }

      if (exceptionCommands.Count > 0)
        return new CompositeCommand (exceptionCommands.Cast<IDataManagementCommand>());

      if (virtualEndPoints.Count == 0)
        return new NopCommand ();
      
      return new UnloadVirtualEndPointsCommand (virtualEndPoints, _registrationAgent, _map);
    }

    public IRelationEndPoint GetRelationEndPointWithoutLoading (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (endPointID.ObjectID == null)
        return CreateNullEndPoint (_clientTransaction, endPointID.Definition);

      return _map[endPointID];
    }

    public IRelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckNotAnonymous (endPointID, "GetRelationEndPointWithLazyLoad", "endPointID");

      var existingEndPoint = GetRelationEndPointWithoutLoading (endPointID);
      if (existingEndPoint != null)
        return existingEndPoint;

      var endPoint = GetRelationEndPointWithMinimumLoading (endPointID);
      endPoint.EnsureDataComplete ();
      return endPoint;
    }

    public IRelationEndPoint GetRelationEndPointWithMinimumLoading (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckNotAnonymous (endPointID, "GetRelationEndPointWithMinimumLoading", "endPointID");

      var existingEndPoint = GetRelationEndPointWithoutLoading (endPointID);
      if (existingEndPoint != null)
      {
        return existingEndPoint;
      }
      else if (endPointID.Definition.IsVirtual)
      {
        return RegisterVirtualEndPoint (endPointID);
      }
      else
      {
        _lazyLoader.LoadLazyDataContainer (endPointID.ObjectID); // will trigger indirect call to RegisterEndPointsForDataContainer
        var endPoint = GetRelationEndPointWithoutLoading (endPointID);
        Assertion.IsNotNull (endPoint, "Non-virtual end-points are registered when the DataContainer is loaded.");
        return endPoint;
      }
    }
    
    public void CommitAllEndPoints ()
    {
      _map.CommitAllEndPoints();
    }

    public void RollbackAllEndPoints ()
    {
      _map.RollbackAllEndPoints();
    }

    private IVirtualEndPoint RegisterVirtualEndPoint (RelationEndPointID endPointID)
    {
      var endPoint = _endPointFactory.CreateVirtualEndPoint (endPointID, false);
      _registrationAgent.RegisterEndPoint (endPoint, _map);
      return endPoint;
    }

    private void CheckNotAnonymous (RelationEndPointID endPointID, string methodName, string argumentName)
    {
      if (endPointID.Definition.IsAnonymous)
      {
        var message = string.Format ("{0} cannot be called for anonymous end points.", methodName);
        throw new ArgumentException (message, argumentName);
      }
    }


    #region Serialization

    // Note: RelationEndPointManager should never be serialized on its own; always start from the DataManager.
    protected RelationEndPointManager (FlattenedDeserializationInfo info)
        : this (
            info.GetValueForHandle<ClientTransaction>(),
            info.GetValueForHandle<ILazyLoader>(),
            info.GetValueForHandle<IRelationEndPointFactory>(),
            info.GetValueForHandle<IRelationEndPointRegistrationAgent> ())
    {
      _dataContainerEndPointsRegistrationAgent = new DelegatingDataContainerEndPointsRegistrationAgent (_endPointFactory, _registrationAgent);
      _map = info.GetValue<RelationEndPointMap> ();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_clientTransaction);
      info.AddHandle (_lazyLoader);
      info.AddHandle (_endPointFactory);
      info.AddHandle (_registrationAgent);
      info.AddValue (_map);
    }

    #endregion
  }
}
