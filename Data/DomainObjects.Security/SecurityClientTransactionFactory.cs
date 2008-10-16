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
using Remotion.Security.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Security
{
  /// <summary>
  /// Implementation of the <see cref="ITransactionFactory"/> interface that creates root <see cref="ClientTransaction"/>s and adds a
  /// <see cref="SecurityClientTransactionExtension"/> when the transaction is created in an application that has a security provider configured.
  /// </summary>
  [Serializable]
  public class SecurityClientTransactionFactory : ClientTransactionFactory
  {
    public SecurityClientTransactionFactory ()
    {
    }

    protected override void OnTransactionCreated (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      if (!SecurityConfiguration.Current.SecurityProvider.IsNull)
        transaction.Extensions.Add (typeof (SecurityClientTransactionExtension).FullName, new SecurityClientTransactionExtension ());
    }
  }
}