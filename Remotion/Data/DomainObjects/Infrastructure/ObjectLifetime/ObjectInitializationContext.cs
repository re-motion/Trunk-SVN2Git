// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime
{
  /// <summary>
  /// Represents the context of an object being initialized via <see cref="IObjectLifetimeAgent.NewObject"/>.
  /// </summary>
  public class ObjectInitializationContext : IObjectInitializationContext
  {
    private readonly ObjectID _objectID;
    private readonly ClientTransaction _bindingTransaction;
    private readonly IEnlistedDomainObjectManager _enlistedDomainObjectManager;
    private readonly IDataManager _dataManager;

    private DomainObject _registeredObject;

    public ObjectInitializationContext (
        ObjectID objectID,
        IEnlistedDomainObjectManager enlistedDomainObjectManager,
        IDataManager dataManager,
        ClientTransaction bindingTransaction)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      ArgumentUtility.CheckNotNull ("enlistedDomainObjectManager", enlistedDomainObjectManager);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      _objectID = objectID;
      _dataManager = dataManager;
      _bindingTransaction = bindingTransaction;
      _enlistedDomainObjectManager = enlistedDomainObjectManager;
    }

    public ObjectID ObjectID
    {
      get { return _objectID; }
    }

    public IEnlistedDomainObjectManager EnlistedDomainObjectManager
    {
      get { return _enlistedDomainObjectManager; }
    }

    public IDataManager DataManager
    {
      get { return _dataManager; }
    }

    public ClientTransaction BindingTransaction
    {
      get { return _bindingTransaction; }
    }

    public DomainObject RegisteredObject
    {
      get { return _registeredObject; }
    }

    public void RegisterObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      if (domainObject.ID != _objectID)
        throw new ArgumentException (string.Format ("The given DomainObject must have ID '{0}'.", _objectID), "domainObject");

      if (BindingTransaction != null && (!domainObject.HasBindingTransaction || domainObject.GetBindingTransaction () != BindingTransaction))
        throw new ArgumentException (string.Format ("The given DomainObject must have BindingClientTransaction '{0}'.", _bindingTransaction), "domainObject");

      if (BindingTransaction == null && domainObject.HasBindingTransaction)
        throw new ArgumentException ("The given DomainObject must not have a BindingClientTransaction.", "domainObject");

      if (_registeredObject != null)
        throw new InvalidOperationException ("Only one object can be registered using this context.");

      var newDataContainer = DataContainer.CreateNew (_objectID);
      newDataContainer.SetDomainObject (domainObject);

      _enlistedDomainObjectManager.EnlistDomainObject (domainObject);
       Assertion.IsNull (_dataManager.DataContainers[_objectID], "If enlisting succeeded, there cannot be a DataContainer with this ID.");
      _dataManager.RegisterDataContainer (newDataContainer);

      _registeredObject = domainObject;
    }
  }
}