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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Transport;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Transport
{
  /// <summary>
  /// Represents the data transported via a <see cref="DomainObjectTransporter"/> object on the target system.
  /// </summary>
  /// <remarks>
  /// Instantiate this class via <see cref="DomainObjectTransporter.LoadTransportData(byte[])"/> with the data obtained from
  /// <see cref="DomainObjectTransporter.GetBinaryTransportData()"/>.
  /// </remarks>
  public struct TransportedDomainObjects
  {
    private ClientTransaction _dataTransaction;
    private ReadOnlyCollection<DomainObject> _transportedObjects;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransportedDomainObjects"/> class. This constructor is typically only used internally,
    /// use <see cref="DomainObjectTransporter.LoadTransportData(byte[])"/> to instantiate this class.
    /// </summary>
    /// <param name="dataTransaction">The transaction containing the transported objects' data.</param>
    /// <param name="transportedObjects">The transported objects.</param>
    public TransportedDomainObjects (ClientTransaction dataTransaction, List<DomainObject> transportedObjects)
    {
      ArgumentUtility.CheckNotNull ("dataTransaction", dataTransaction);

      _dataTransaction = dataTransaction;
      _transportedObjects = transportedObjects.AsReadOnly();
    }

    /// <summary>
    /// Gets the transaction holding the data of the transported objects. Use <see cref="ClientTransaction.EnterNonDiscardingScope"/> to
    /// inspect the data in an application.
    /// </summary>
    /// <value>The transaction holding the data of the transported objects.</value>
    public ClientTransaction DataTransaction
    {
      get { return _dataTransaction; }
    }

    /// <summary>
    /// Gets the transported objects. Use <see cref="DataTransaction"/> to inspect or change their data before calling <see cref="FinishTransport()"/>.
    /// </summary>
    /// <value>The transported objects.</value>
    public ReadOnlyCollection<DomainObject> TransportedObjects
    {
      get { return _transportedObjects; }
    }

    /// <summary>
    /// Finishes the transport by committing the <see cref="DataTransaction"/> to the database.
    /// </summary>
    /// <remarks>This method invalidated the <see cref="TransportedDomainObjects"/> object, setting <see cref="DataTransaction"/> and
    /// <see cref="TransportedObjects"/> to <see langword="null"/>.</remarks>
    public void FinishTransport ()
    {
      FinishTransport (delegate { return true; });
    }

    /// <summary>
    /// Finishes the transport by committing the <see cref="DataTransaction"/> to the database, providing a filter to exclude some objects
    /// from being committed.
    /// </summary>
    /// <param name="filter">A filter delegate called for each object that would be committed to the database. Return true to include the
    /// object in the commit, or false to leave its state in the database as is.</param>
    /// <remarks>This method invalidated the <see cref="TransportedDomainObjects"/> object, setting <see cref="DataTransaction"/> and
    /// <see cref="TransportedObjects"/> to <see langword="null"/>.</remarks>
    public void FinishTransport (Func<DomainObject, bool> filter)
    {
      ArgumentUtility.CheckNotNull ("filter", filter);

      if (DataTransaction == null)
        throw new InvalidOperationException ("FinishTransport can only be called once.");

      DataTransaction.AddListener (new TransportFinishTransactionListener (DataTransaction, filter));
      DataTransaction.Commit ();

      _dataTransaction = null;
      _transportedObjects = null;
    }
  }
}
