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

namespace Remotion.Implementation
{
  public static class VersionDependentImplementationBridge<T>
  {
    public static readonly T Implementation = GetImplementation();

    private static T GetImplementation ()
    {
      ConcreteImplementationAttribute[] attributes = 
          (ConcreteImplementationAttribute[]) typeof (T).GetCustomAttributes (typeof (ConcreteImplementationAttribute), false);
      if (attributes.Length != 1)
      {
        string message = string.Format ("Cannot get a version-dependent implementation of type '{0}': Expected one " 
            + "ConcreteImplementationAttribute applied to the type, but found {1}.", typeof (T).FullName, attributes.Length);
        throw new InvalidOperationException(message);
      }
      return (T) attributes[0].InstantiateType();
    }
  }
}
