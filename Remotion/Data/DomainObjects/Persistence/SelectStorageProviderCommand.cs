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
using System.Collections.Generic;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  // TODO 4113 Implement with tests
  //public class SelectStorageProviderCommand<TIn, TOut, TExecutionContext> : IStorageProviderCommand<IEnumerable<TOut>, TExecutionContext>
  //{
  //  private readonly IStorageProviderCommand<IEnumerable<TIn>, TExecutionContext> _command;
  //  private readonly Func<TIn, TOut> _selector;

  //  public SelectStorageProviderCommand (IStorageProviderCommand<IEnumerable<TIn>, TExecutionContext> command, Func<TIn, TOut> selector)
  //  {
  //    ArgumentUtility.CheckNotNull ("command", command);
  //    ArgumentUtility.CheckNotNull ("selector", selector);

  //    _command = command;
  //    _selector = selector;
  //  }

  //  public IEnumerable<TOut> Execute (TExecutionContext executionContext)
  //  {
  //    return _command.Execute (executionContext).Select (_selector);
  //  }
  //}
}