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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  [Serializable]
  public class BindingClientTransaction : RootClientTransaction
  {
    /// <summary>
    /// Do not use this method, use <see>ClientTransaction.CreateRootTransaction</see> instead.
    /// </summary>
    /// <returns></returns>
    [Obsolete ("Use ClientTransaction.CreateRootTransaction for clarity.")]
    public static new ClientTransaction CreateRootTransaction ()
    {
      return ClientTransaction.CreateRootTransaction();
    }

    public override ClientTransaction CreateSubTransaction ()
    {
      throw new InvalidOperationException ("Binding transactions cannot have subtransactions.");
    }

    protected override void CheckDomainObjectForEnlisting (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      if (!domainObject.HasBindingTransaction || domainObject.GetBindingTransaction() != this)
      {
        string message =
            string.Format (
                "Cannot enlist the domain object {0} in this binding transaction, because it has originally been loaded in another transaction.",
                domainObject.ID);
        throw new InvalidOperationException (message);
      }

      base.CheckDomainObjectForEnlisting (domainObject);
    }
  }
}
