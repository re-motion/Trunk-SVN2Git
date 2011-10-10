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
using System.Collections.ObjectModel;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Handles commit validation for <see cref="ClientTransaction"/> instances.
  /// </summary>
  /// <remarks>
  /// Currently, this extension only checks that all mandatory relations are set.
  /// </remarks>
  public class CommitValidationClientTransactionExtension : ClientTransactionExtensionBase
  {
    public static string DefaultKey
    {
      get { return typeof (CommitValidationClientTransactionExtension).FullName; }
    }

    public CommitValidationClientTransactionExtension ()
        : this (DefaultKey)
    {
    }

    protected CommitValidationClientTransactionExtension (string key)
        : base (key)
    {
    }

    public override void CommitValidate (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
      foreach (var domainObject in changedDomainObjects)
      {
        var dataContainer = clientTransaction.DataManager.GetDataContainerWithoutLoading (domainObject.ID);
        if (dataContainer != null)
          clientTransaction.DataManager.ValidateMandatoryRelations (dataContainer);
      }
    }
  }
}