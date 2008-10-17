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
  public class VersionDependentImplementationException : Exception
  {
    public static VersionDependentImplementationException Wrap (Type versionDependentType, Exception innerException)
    {
      ArgumentUtility.CheckNotNull ("versionDependentType", versionDependentType);
      ArgumentUtility.CheckNotNull ("innerException", innerException);

      string message = string.Format ("The initialization of type '{0}' threw an exception of type '{1}': {2}", versionDependentType.FullName,
                                      innerException.GetType().Name, innerException.Message);
      return new VersionDependentImplementationException (message, innerException);
    }

    private VersionDependentImplementationException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}