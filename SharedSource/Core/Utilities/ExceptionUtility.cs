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
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Remotion.Utilities
{
  static partial class ExceptionUtility
  {
    private static readonly Lazy<Action<Exception>> s_internalPreserveStackTrace = new Lazy<Action<Exception>> (GetInternalPreserveStackTrace);

    public static Exception PreserveStackTrace (this Exception exception)
    {
      if (exception == null)
        throw new ArgumentNullException ("exception");

      // http://weblogs.asp.net/fmarguerie/archive/2008/01/02/rethrowing-exceptions-and-preserving-the-full-call-stack-trace.aspx

      s_internalPreserveStackTrace.Value (exception);
      return exception;
    }

    private static Action<Exception> GetInternalPreserveStackTrace ()
    {
      var methodInfo = typeof (Exception).GetMethod ("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);
      if (methodInfo == null)
        throw new InvalidOperationException ("Type 'System.Exception' does not contain method InternalPreserveStackTrace().");

      return (Action<Exception>) methodInfo.CreateDelegate (typeof (Action<Exception>));
    }
  }
}
