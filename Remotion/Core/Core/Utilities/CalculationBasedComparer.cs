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

namespace Remotion.Utilities
{
  /// <summary>
  /// Implements <see cref="IComparer{T}"/> for values of a type <typeparamref name="TCompared"/> by calculating a <typeparamref name="TCalculated"/> 
  /// using a delegate.
  /// </summary>
  /// <typeparam name="TCompared">The type of the objects to be compared.</typeparam>
  /// <typeparam name="TCalculated">
  /// The type yielded by the calculation. The values returned by the calculation are used to perform the actual comparison.
  /// </typeparam>
  public class CalculationBasedComparer<TCompared, TCalculated> : IComparer<TCompared>
  {
    private readonly Func<TCompared, TCalculated> _calculation;
    private readonly IComparer<TCalculated> _valueComparer;

    public CalculationBasedComparer (Func<TCompared, TCalculated> calculation, IComparer<TCalculated> valueComparer)
    {
      ArgumentUtility.CheckNotNull ("calculation", calculation);
      ArgumentUtility.CheckNotNull ("valueComparer", valueComparer);

      _calculation = calculation;
      _valueComparer = valueComparer;
    }

    public int Compare (TCompared x, TCompared y)
    {
      return _valueComparer.Compare (_calculation (x), _calculation (y));
    }
  }
}