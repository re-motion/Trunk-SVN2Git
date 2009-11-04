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
using System.Reflection;
using Remotion.Reflection;

namespace Remotion.Utilities
{
  public static class ExceptionUtility
  {
    public static Exception PreserveStackTrace (this Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);
      // http://weblogs.asp.net/fmarguerie/archive/2008/01/02/rethrowing-exceptions-and-preserving-the-full-call-stack-trace.aspx
      // http://www.dotnetjunkies.com/WebLog/chris.taylor/archive/2004/03/03/8353.aspx
      // PrivateInvoke.SetNonPublicField (exception, "_remoteStackTraceString", exception.StackTrace + Environment.NewLine);
      
      MethodCaller.CallAction ("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic).With (exception);
      return exception;
    }
  }
}
