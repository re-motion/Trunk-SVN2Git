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
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Transaction
{
  public class InvertingClientTransactionMixin : Mixin<ClientTransaction, InvertingClientTransactionMixin.IBaseCallRequirements>
  {
    public interface IBaseCallRequirements
    {
      void Commit ();
      void Rollback ();
    }

    [OverrideTarget]
    public void Commit ()
    {
      Base.Rollback (); // okay, this is not really realistic
    }

    [OverrideTarget]
    public void Rollback ()
    {
      Base.Commit ();
    }
  }
}
