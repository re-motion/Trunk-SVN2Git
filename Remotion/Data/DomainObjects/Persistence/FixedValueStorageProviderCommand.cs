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

namespace Remotion.Data.DomainObjects.Persistence
{
  // TODO Review 4078: Add test
  /// <summary>
  /// Implements <see cref="IStorageProviderCommand{T,TExecutionContext}"/> by always returning the same, fixed value.
  /// </summary>
  /// <typeparam name="T">The type of the value to return.</typeparam>
  /// <typeparam name="TExecutionContext">The type of the execution context. This is not actually used by this command.</typeparam>
  public class FixedValueStorageProviderCommand<T, TExecutionContext> : IStorageProviderCommand<T, TExecutionContext>
  {
    private readonly T _value;

    public FixedValueStorageProviderCommand (T value)
    {
      _value = value;
    }

    public T Execute (TExecutionContext executionContext)
    {
      return _value;
    }
  }
}