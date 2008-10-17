/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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