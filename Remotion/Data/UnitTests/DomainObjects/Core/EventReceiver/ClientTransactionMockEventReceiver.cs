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
using Remotion.Data.DomainObjects;
using Remotion.Utilities;
using Rhino.Mocks;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver
{
  public abstract class ClientTransactionMockEventReceiver
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    protected ClientTransactionMockEventReceiver (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      clientTransaction.Loaded += Loaded;
      clientTransaction.Committing += Committing;
      clientTransaction.Committed += Committed;
      clientTransaction.RollingBack += RollingBack;
      clientTransaction.RolledBack += RolledBack;
      clientTransaction.SubTransactionCreated += SubTransactionCreated;
    }

    // abstract methods and properties

    public abstract void Loaded (object sender, ClientTransactionEventArgs args);
    public abstract void Committing (object sender, ClientTransactionEventArgs args);
    public abstract void Committed (object sender, ClientTransactionEventArgs args);
    public abstract void RollingBack (object sender, ClientTransactionEventArgs args);
    public abstract void RolledBack (object sender, ClientTransactionEventArgs args);
    public abstract void SubTransactionCreated (object sender, SubTransactionCreatedEventArgs args);

    // methods and properties

    public void RollingBack (object sender, params DomainObject[] domainObjects)
    {
      RollingBack (Arg.Is (sender), Arg<ClientTransactionEventArgs>.Matches (args => args.DomainObjects.SetEquals (domainObjects)));
    }

    public void RolledBack (object sender, params DomainObject[] domainObjects)
    {
      RolledBack (Arg.Is (sender), Arg<ClientTransactionEventArgs>.Matches (args => args.DomainObjects.SetEquals (domainObjects)));
    }

    public void Committing (object sender, params DomainObject[] domainObjects)
    {
      Committing (Arg.Is (sender), Arg<ClientTransactionEventArgs>.Matches (args => args.DomainObjects.SetEquals (domainObjects)));
    }

    public void Committed (object sender, params DomainObject[] domainObjects)
    {
      Committed (Arg.Is (sender), Arg<ClientTransactionEventArgs>.Matches (args => args.DomainObjects.SetEquals (domainObjects)));
    }
  }
}
