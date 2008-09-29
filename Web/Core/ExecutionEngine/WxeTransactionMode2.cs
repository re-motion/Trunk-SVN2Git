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
  public abstract class WxeTransactionMode<TScopeManager>
      where TScopeManager : ITransactionScopeManager, new ()
  {
    public static readonly ITransactionMode Null = new NullTransactionMode();

    public static readonly ITransactionMode CreateRoot = new CreateRootTransactionMode<TScopeManager>(false);

    public static readonly ITransactionMode CreateRootWithAutoCommit = new CreateRootTransactionMode<TScopeManager> (true);

    public static readonly ITransactionMode CreateChildIfParent = new CreateChildIfParentTransactionMode<TScopeManager>(false);

    public static readonly ITransactionMode CreateChildIfParentWithAutoCommit = new CreateChildIfParentTransactionMode<TScopeManager> (true);

    private WxeTransactionMode ()
    {
    }
  }
}