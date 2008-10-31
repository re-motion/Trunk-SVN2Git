/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  public class ClientTransactionWithCustomITransaction : RootClientTransaction
  {
    private readonly ITransaction _wrapper;

    public ClientTransactionWithCustomITransaction (ITransaction wrapper)
    {
      _wrapper = wrapper;
    }

    public override ITransaction ToITransation ()
    {
      return _wrapper;
    }
  }
}