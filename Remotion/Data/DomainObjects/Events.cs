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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  //public delegate void SubTransactionCreatedEventHandler (object sender, SubTransactionCreatedEventArgs args);

  /// <summary>
  /// Provides data for change events of <see cref="PropertyValue"/> instances.
  /// </summary>
  [Serializable]
  public class ValueChangeEventArgs : EventArgs
  {
    private readonly object _oldValue;
    private readonly object _newValue;

    /// <summary>
    /// Initializes a new instance of the <b>ValueChangingEventArgs</b>.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    public ValueChangeEventArgs (object oldValue, object newValue)
    {
      _oldValue = oldValue;
      _newValue = newValue;
    }

    /// <summary>
    /// Gets the old value.
    /// </summary>
    public object OldValue
    {
      get { return _oldValue; }
    }

    /// <summary>
    /// Gets the new value.
    /// </summary>
    public object NewValue
    {
      get { return _newValue; }
    }
  }

  /// <summary>
  /// Provides data for a <b>PropertyChanging</b> event.
  /// </summary>
  [Serializable]
  public class PropertyChangeEventArgs : ValueChangeEventArgs
  {
    private readonly PropertyValue _propertyValue;

    /// <summary>
    /// Initializes a new instance of the <b>ValueChangingEventArgs</b> class.
    /// </summary>
    /// <param name="propertyValue">The <see cref="PropertyValue"/> that is being changed. Must not be <see langword="null"/>.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyValue"/> is <see langword="null"/>.</exception>
    public PropertyChangeEventArgs (PropertyValue propertyValue, object oldValue, object newValue)
        : base (oldValue, newValue)
    {
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);
      _propertyValue = propertyValue;
    }

    /// <summary>
    /// Gets the <see cref="DataManagement.PropertyValue"/> object that is being changed.
    /// </summary>
    public PropertyValue PropertyValue
    {
      get { return _propertyValue; }
    }
  }

  /// <summary>
  /// Provides data for a <b>RelationChanging</b> event.
  /// </summary>
  [Serializable]
  public class RelationChangingEventArgs : EventArgs
  {
    private readonly string _propertyName;
    private readonly DomainObject _oldRelatedObject;
    private readonly DomainObject _newRelatedObject;

    /// <summary>
    /// Initializes a new instance of the <b>RelationChangingEventArgs</b> class.
    /// </summary>
    /// <param name="propertyName">The name of the property that is being changed due to the relation change. Must not be <see langword="null"/>.</param>
    /// <param name="oldRelatedObject">The old object that was related.</param>
    /// <param name="newRelatedObject">The new object that is related.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    public RelationChangingEventArgs (
        string propertyName,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      _propertyName = propertyName;
      _oldRelatedObject = oldRelatedObject;
      _newRelatedObject = newRelatedObject;
    }

    /// <summary>
    /// Gets the name of the <see cref="PropertyValue"/> that is being changed due to the relation change.
    /// </summary>
    public string PropertyName
    {
      get { return _propertyName; }
    }

    /// <summary>
    /// Gets the <see cref="DomainObject"/> that was related.
    /// </summary>
    public DomainObject OldRelatedObject
    {
      get { return _oldRelatedObject; }
    }

    /// <summary>
    /// Gets the <see cref="DomainObject"/> that is related.
    /// </summary>
    public DomainObject NewRelatedObject
    {
      get { return _newRelatedObject; }
    }
  }

  /// <summary>
  /// Provides data for a <b>RelationChanged</b> event.
  /// </summary>
  [Serializable]
  public class RelationChangedEventArgs : EventArgs
  {
    private readonly string _propertyName;

    /// <summary>
    /// Initializes a new instance of the <b>RelationChangingEventArgs</b> class.
    /// </summary>
    /// <param name="propertyName">The name of the <see cref="PropertyValue"/> that is being changed due to the relation change. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    public RelationChangedEventArgs (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      _propertyName = propertyName;
    }

    /// <summary>
    /// Gets the name of the <see cref="PropertyValue"/> that has been changed due to the relation change.
    /// </summary>
    public string PropertyName
    {
      get { return _propertyName; }
    }
  }

  /// <summary>
  /// Provides data for <see cref="ClientTransaction"/> events.
  /// </summary>
  [Serializable]
  public class ClientTransactionEventArgs : EventArgs
  {
    private readonly ReadOnlyCollection<DomainObject> _domainObjects;

    /// <summary>
    /// Initializes a new instance of the <b>ClientTransactionEventArgs</b> class.
    /// </summary>
    /// <param name="domainObjects">The <see cref="DomainObject"/>s affected by the <see cref="ClientTransaction"/> operation. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObjects"/> is <see langword="null"/>.</exception>
    public ClientTransactionEventArgs (ReadOnlyCollection<DomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);
      _domainObjects = domainObjects;
    }

    /// <summary>
    /// Gets the <see cref="DomainObject"/>s affected by the <see cref="ClientTransaction.Commit"/> operation.
    /// </summary>
    public ReadOnlyCollection<DomainObject> DomainObjects
    {
      get { return _domainObjects; }
    }
  }

  /// <summary>
  /// Provides data for the <see cref="ClientTransaction.SubTransactionCreated"/> event.
  /// </summary>
  [Serializable]
  public class SubTransactionCreatedEventArgs : EventArgs
  {
    private readonly ClientTransaction _subTransaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubTransactionCreatedEventArgs"/> class.
    /// </summary>
    /// <param name="subTransaction">The subtransaction created.</param>
    public SubTransactionCreatedEventArgs (ClientTransaction subTransaction)
    {
      ArgumentUtility.CheckNotNull ("subTransaction", subTransaction);
      _subTransaction = subTransaction;
    }

    /// <summary>
    /// Gets the subtransaction created.
    /// </summary>
    /// <value>The new subtransaction for which the event was raised.</value>
    public ClientTransaction SubTransaction
    {
      get { return _subTransaction; }
    }
  }
}
