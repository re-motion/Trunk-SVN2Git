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
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests
{
  public static class MethodOptionsExtensions
  {
    /// <summary>
    /// Provides support for ordered Rhino.Mocks expectations without dedicated <see cref="MockRepository"/> instance. Create an 
    /// <see cref="OrderedExpectationCounter"/>, then call <see cref="Ordered{T}"/> with that counter for all expectations that should occur 
    /// in the same order as declared.
    /// Uses <see cref="IMethodOptions{T}.WhenCalled"/> internally.
    /// </summary>
    public static IMethodOptions<T> Ordered<T> (this IMethodOptions<T> options, OrderedExpectationCounter counter)
    {
      var expectedPosition = counter.GetNextExpectedPosition();
      return options.WhenCalled (mi => counter.CheckPosition (mi.Method.ToString(), expectedPosition));
    }
  }
}