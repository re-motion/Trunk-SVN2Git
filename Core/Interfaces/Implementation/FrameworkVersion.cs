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
using System.Collections.Generic;
using System.Reflection;

namespace Remotion.Implementation
{
  public static class FrameworkVersion
  {
    private static Version s_value;
    private static readonly object s_valueLock = new object();

    public static Version Value
    {
      get
      {
        lock (s_valueLock)
        {
          if (s_value == null)
            s_value = RetrieveFrameworkVersion();
          return s_value;
        }
      }
      set
      {
        lock (s_valueLock)
        {
          ArgumentUtility.CheckNotNull ("value", value);
          if (s_value != null)
            throw new InvalidOperationException (
                string.Format ("The framework version has already been set to {0}. It can only be set once.", s_value));
          s_value = value;
        }
      }
    }

    private static Version RetrieveFrameworkVersion ()
    {
      FrameworkVersionRetriever retriever = new FrameworkVersionRetriever (AppDomain.CurrentDomain.GetAssemblies());
      try
      {
        return retriever.RetrieveVersion ();
      }
      catch (Exception ex)
      {
        string message = string.Format ("The framework version could not be determined automatically. Manually set {0}.Value to specify which "
            + "version should be used. The automatic discovery error was: {1}", typeof (FrameworkVersion).FullName, ex.Message);
        throw new InvalidOperationException (message, ex);
      }
    }

    public static void Reset ()
    {
      s_value = null;
    }
  }
}
