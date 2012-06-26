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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation.Transport
{
  [Serializable]
  public class TransportTransactionListener : ClientTransactionListenerBase
  {
    [NonSerialized]
    private readonly DomainObjectTransporter _transporter;

    public TransportTransactionListener (DomainObjectTransporter transporter)
    {
      ArgumentUtility.CheckNotNull ("transporter", transporter);
      _transporter = transporter;
    }

    public override void PropertyValueChanging (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      if (_transporter == null)
        throw new InvalidOperationException ("Cannot use the transported transaction for changing properties after it has been deserialized.");

      if (!_transporter.IsLoaded (dataContainer.ID))
      {
        string message = string.Format ("Object '{0}' cannot be modified for transportation because it hasn't been loaded yet. Load it before "
            + "manipulating it.", dataContainer.ID);
        throw new InvalidOperationException(message);
      }
    }

    public override void TransactionCommitting (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      throw new InvalidOperationException ("The transport transaction cannot be committed.");
    }

    public override void TransactionRollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      throw new InvalidOperationException ("The transport transaction cannot be rolled back.");
    }
  }
}
