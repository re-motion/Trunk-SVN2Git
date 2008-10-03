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
using Remotion.Data;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  //TODO: Doc
  public abstract class WxeTransactionMode<TTransactionFactory>
      where TTransactionFactory : ITransactionFactory, new ()
  {
    public static readonly ITransactionMode None = new NoneTransactionMode();

    public static readonly ITransactionMode CreateRoot = new CreateRootTransactionMode<TTransactionFactory>(false);

    public static readonly ITransactionMode CreateRootWithAutoCommit = new CreateRootTransactionMode<TTransactionFactory> (true);

    public static readonly ITransactionMode CreateChildIfParent = new CreateChildIfParentTransactionMode<TTransactionFactory>(false);

    public static readonly ITransactionMode CreateChildIfParentWithAutoCommit = new CreateChildIfParentTransactionMode<TTransactionFactory> (true);

    private WxeTransactionMode ()
    {
    }
  }
}