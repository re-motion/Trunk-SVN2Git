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

    protected internal override bool DoEnlistDomainObject (DomainObject domainObject)
    {
      if (!domainObject.IsBoundToSpecificTransaction || domainObject.BindingTransaction != this)
      {
        string message =
            string.Format (
                "Cannot enlist the domain object {0} in this binding transaction, because it has originally been loaded in another transaction.",
                domainObject.ID);
        throw new InvalidOperationException (message);
      }
      return base.DoEnlistDomainObject (domainObject);
    }
  }
}
