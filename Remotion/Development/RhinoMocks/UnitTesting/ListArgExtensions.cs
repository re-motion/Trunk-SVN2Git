// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections;
using System.Collections.Generic;
using Remotion.Development.UnitTesting;
using Remotion.Text;
using Rhino.Mocks.Constraints;
using Remotion.FunctionalProgramming;
using System.Linq;

namespace Remotion.Development.RhinoMocks.UnitTesting
{
  /// <summary>
  /// Extensions methods for the <see cref="ListArg{T}"/> class.
  /// </summary>
  public static class ListArgExtensions
  {
    /// <summary>
    /// Similiar to <see cref="ListArg{T}.Equal"/> but without considering the order of the elements in the collection.
    /// </summary>
    public static T Equivalent<T> (this ListArg<T> arg, IEnumerable collection) where T : IEnumerable
    {
      var items = collection.Cast<object>().ToArray();
      var type = typeof (ListArg<>).Assembly.GetType ("Rhino.Mocks.ArgManager", true);
      var message = "equivalent to collection [" + SeparatedStringBuilder.Build (", ", items) + "]";
      var constraint = new PredicateConstraintWithMessage<T> (c => c.Cast<object> ().SetEquals (items), message);
      PrivateInvoke.InvokeNonPublicStaticMethod (type, "AddInArgument", constraint);
      return default (T);
    }

    /// <summary>
    /// Similiar to <see cref="ListArg{T}.Equal"/> but without considering the order of the elements in the collection.
    /// </summary>
    public static T Equivalent<T> (this ListArg<T> arg, params object[] items) where T : IEnumerable
    {
      return Equivalent (arg, (IEnumerable) items);
    }

    public static IEnumerable<T> Equivalent<T> (this ListArg<IEnumerable<T>> arg, params T[] items)
    {
      var argManagerType = typeof (ListArg<>).Assembly.GetType ("Rhino.Mocks.ArgManager", true);
      var message = "equivalent to collection [" + SeparatedStringBuilder.Build (", ", items) + "]";
      var constraint = new PredicateConstraintWithMessage<IEnumerable<T>> (c => c.SetEquals (items), message);
      PrivateInvoke.InvokeNonPublicStaticMethod (argManagerType, "AddInArgument", constraint);

      return new T[0];
    }

    class PredicateConstraintWithMessage<T> : PredicateConstraint<T>
    {
      private readonly string _message;

      public PredicateConstraintWithMessage (Predicate<T> predicate, string message)
          : base(predicate)
      {
        _message = message;
      }

      public override string Message
      {
        get
        {
          return _message;
        }
      }
    }
  }
}