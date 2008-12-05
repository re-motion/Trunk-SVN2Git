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
using System.Collections;
using NUnit.Framework;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver
{
  public class ClientTransactionEventReceiver
  {
    // types

    // static members and constants

    // member fields

    private ClientTransaction _clientTransaction;
    private ArrayList _loadedDomainObjects;
    private ArrayList _committingDomainObjects;
    private ArrayList _committedDomainObjects;

    // construction and disposing

    public ClientTransactionEventReceiver (ClientTransaction clientTransaction)
    {
      _loadedDomainObjects = new ArrayList ();
      _committingDomainObjects = new ArrayList ();
      _committedDomainObjects = new ArrayList ();
      _clientTransaction = clientTransaction;

      _clientTransaction.Loaded += new ClientTransactionEventHandler (ClientTransaction_Loaded);
      _clientTransaction.Committing += new ClientTransactionEventHandler (ClientTransaction_Committing);
      _clientTransaction.Committed += new ClientTransactionEventHandler (ClientTransaction_Committed);
    }

    // methods and properties

    public void Clear ()
    {
      _loadedDomainObjects = new ArrayList ();
      _committingDomainObjects = new ArrayList ();
      _committedDomainObjects = new ArrayList ();
    }

    public void Unregister ()
    {
      _clientTransaction.Loaded -= new ClientTransactionEventHandler (ClientTransaction_Loaded);
      _clientTransaction.Committing -= new ClientTransactionEventHandler (ClientTransaction_Committing);
      _clientTransaction.Committed -= new ClientTransactionEventHandler (ClientTransaction_Committed);
    }

    private void ClientTransaction_Loaded (object sender, ClientTransactionEventArgs args)
    {
      _loadedDomainObjects.Add (args.DomainObjects);
    }

    private void ClientTransaction_Committing (object sender, ClientTransactionEventArgs args)
    {
      Assert.IsTrue (args.DomainObjects.IsReadOnly);

      _committingDomainObjects.Add (args.DomainObjects);
    }

    private void ClientTransaction_Committed (object sender, ClientTransactionEventArgs args)
    {
      Assert.IsTrue (args.DomainObjects.IsReadOnly);

      _committedDomainObjects.Add (args.DomainObjects);
    }

    public ArrayList LoadedDomainObjects
    {
      get { return _loadedDomainObjects; }
    }

    public ArrayList CommittingDomainObjects
    {
      get { return _committingDomainObjects; }
    }

    public ArrayList CommittedDomainObjects
    {
      get { return _committedDomainObjects; }
    }
  }
}
