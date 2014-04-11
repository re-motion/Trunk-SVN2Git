// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 

using System;

// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// Provides a <see cref="Null"/> property that can be assigned arbitrary values, and a type <see cref="T"/> to be used as a dummy generic argument.
  /// </summary>
  public static partial class Dev
  {
    /// <summary>
    /// Defines a dummy type that can be used as a generic argument.
    /// </summary>
    public class T
    {
    }

    /// <summary>
    /// Use this in unit tests where you need to assign a value to
    /// something (e.g., for syntactic reasons, or to remove unused variable warnings), but don't care about the result of the assignment.
    /// </summary>
    public static object Null
    {
      get { return null; }
      // ReSharper disable ValueParameterNotUsed
      set { }
      // ReSharper restore ValueParameterNotUsed
    }
  }

  /// <summary>
  /// Provides a <see cref="Dummy"/> field that can be used as a ref or out parameter, and a typed <see cref="Null"/> property that can be assigned 
  /// arbitrary values and always returns the default value for <typeparamref name="T"/>.
  /// </summary>
  public static class Dev<T>
  {
    /// <summary>
    /// Use this in unit tests where you need a ref or out parameter but but don't care about the result of the assignment.
    /// Never rely on the value of the <see cref="Dummy"/> field, it will be changed by other tests.
    /// </summary>
    public static T Dummy;

    /// <summary>
    /// Use this in unit tests where you need to assign a value to
    /// something (e.g., for syntactic reasons, or to remove unused variable warnings), but don't care about the result of the assignment.
    /// </summary>
    public static T Null
    {
      get { return default (T); }
      set { Dev.Null = value; }
    }
  }
}
