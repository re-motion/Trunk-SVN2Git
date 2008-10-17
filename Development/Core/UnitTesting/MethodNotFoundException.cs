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
  [Serializable]
  public class MethodNotFoundException: Exception
  {
    private const string c_errorMessage = "There is no method \"{0}\" in type {1} that accepts the specified argument types.";

    public MethodNotFoundException (string methodName, Type type)
        : this (string.Format (c_errorMessage, methodName, type.FullName))
    {
    }

    public MethodNotFoundException (string message)
        : base (message)
    {
    }

    protected MethodNotFoundException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }
  }
}