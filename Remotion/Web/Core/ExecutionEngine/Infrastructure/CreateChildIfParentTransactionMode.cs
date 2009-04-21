// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  [Serializable]
  public class CreateChildIfParentTransactionMode : CreateRootTransactionMode
  {
    public CreateChildIfParentTransactionMode (bool autoCommit, ITransactionFactory transactionFactory)
      :base (autoCommit, transactionFactory)
    {
    }

    public override TransactionStrategyBase CreateTransactionStrategy (WxeFunction function, WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("context", context);

      if (function.ParentFunction != null)
      {
        var childTransactionStrategy = function.ParentFunction.TransactionStrategy.CreateChildTransactionStrategy (AutoCommit, function, context);
        if (childTransactionStrategy != null)
          return childTransactionStrategy;
      }

      return base.CreateTransactionStrategy (function, context);
    }
  }
}
