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

    public BindingClientTransaction ()
    {
      AddListener (new BindingClientTransactionListener (this));
    }

    public override ClientTransaction CreateSubTransaction ()
    {
      throw new InvalidOperationException ("Binding transactions cannot have subtransactions.");
    }

    protected internal override bool DoEnlistDomainObject (DomainObject domainObject)
    {
      if (!domainObject.IsBoundToSpecificTransaction || domainObject.ClientTransaction != this)
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
