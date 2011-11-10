// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Handles commit validation for <see cref="ClientTransaction"/> instances.
  /// </summary>
  /// <remarks>
  /// Currently, this extension only checks that all mandatory relations are set.
  /// </remarks>
  [Serializable]
  public class CommitValidationClientTransactionExtension : ClientTransactionExtensionBase
  {
    private readonly Func<ClientTransaction, IPersistableDataValidator> _validatorFactory;

    public static string DefaultKey
    {
      get { return typeof (CommitValidationClientTransactionExtension).FullName; }
    }

    public Func<ClientTransaction, IPersistableDataValidator> ValidatorFactory
    {
      get { return _validatorFactory; }
    }

    public CommitValidationClientTransactionExtension (Func<ClientTransaction, IPersistableDataValidator> validatorFactory)
      : this (validatorFactory, DefaultKey)
    {
    }

    protected CommitValidationClientTransactionExtension (Func<ClientTransaction, IPersistableDataValidator> validatorFactory, string key)
        : base (key)
    {
      ArgumentUtility.CheckNotNull ("validatorFactory", validatorFactory);
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);

      _validatorFactory = validatorFactory;
    }

    public override void CommitValidate (ClientTransaction clientTransaction, ReadOnlyCollection<PersistableData> committedData)
    {
      var validator = _validatorFactory (clientTransaction);
      foreach (var item in committedData)
        validator.Validate (item);
    }
  }
}