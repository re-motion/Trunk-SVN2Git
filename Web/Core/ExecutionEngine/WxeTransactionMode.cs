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
using Remotion.Data;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  //TODO: Doc
  public abstract class WxeTransactionMode<TTransactionFactory>
      where TTransactionFactory : ITransactionFactory, new ()
  {
    public static readonly ITransactionMode None = new NoneTransactionMode();

    public static readonly ITransactionMode CreateRoot = new CreateRootTransactionMode(false, new TTransactionFactory ());

    public static readonly ITransactionMode CreateRootWithAutoCommit = new CreateRootTransactionMode (true, new TTransactionFactory ());

    public static readonly ITransactionMode CreateChildIfParent = new CreateChildIfParentTransactionMode(false, new TTransactionFactory());

    public static readonly ITransactionMode CreateChildIfParentWithAutoCommit = new CreateChildIfParentTransactionMode (true, new TTransactionFactory());

    protected WxeTransactionMode ()
    {
    }

    internal abstract void BlockInstantiableInheritance ();
  }
}
