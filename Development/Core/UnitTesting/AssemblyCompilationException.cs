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
using System.Runtime.Serialization;

namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// The <see cref="AssemblyCompilationException"/> is thrown by the <see cref="AssemblyCompiler"/> type when compilation errors occured.
  /// </summary>
  [Serializable]
  public class AssemblyCompilationException : Exception
  {
    public AssemblyCompilationException (string message)
        : base (message)
    {
    }

    public AssemblyCompilationException (string message, Exception inner)
        : base (message, inner)
    {
    }

    protected AssemblyCompilationException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }
  }
}
