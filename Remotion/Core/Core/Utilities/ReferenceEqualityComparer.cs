// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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

//
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Remotion.Utilities
{
  /// <summary>      
  /// An equality comparer that compares equality using ReferenceEquals rather than Equals. This is to ensure that two objects are actually the same 
  /// and not just equal for reference checking purposes.      
  /// </summary>      
  /// <typeparam name="T">the type of object to check</typeparam>  
  public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
  {
    public bool Equals (T x, T y)
    {
      return object.ReferenceEquals (x, y);
    }

    public int GetHashCode (T obj)
    {
      return RuntimeHelpers.GetHashCode (obj); 
    }
  }
}