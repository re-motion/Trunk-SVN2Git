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
  /// <summary>
  /// Holds the <see cref="Version"/> object used by <see cref="VersionDependentImplementationBridge{T}"/> to retrieve types from
  /// the re-motion implementation assemblies.
  /// </summary>
  public static class FrameworkVersion
  {
    private static Version s_value;
    private static readonly object s_valueLock = new object();

    /// <summary>
    /// Gets or sets the framework version value.
    /// </summary>
    /// <value>The framework version value. If none has been set, the getter will attempt to retrieve the version automatically (and throw
    /// an exception if it cannot do so). When one version has been set, no other version can be set unless <see cref="Reset"/> is called before.
    /// (The same version can be set without an exception being thrown.)</value>
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
          if (s_value != null && s_value != value)
            throw new InvalidOperationException (
                string.Format ("The framework version has already been set to {0}. It can only be set once.", s_value));
          s_value = value;
        }
      }
    }

    private static Version RetrieveFrameworkVersion ()
    {
      var retriever = new FrameworkVersionRetriever (AppDomain.CurrentDomain.GetAssemblies());
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

    /// <summary>
    /// Resets the version information so that the next retrieval of <see cref="Value"/> will trigger an automatic resolution. After calling this
    /// method, <see cref="Value"/> can be used to set a different <see cref="Version"/> value.
    /// </summary>
    public static void Reset ()
    {
      s_value = null;
    }

    /// <summary>
    /// Retrieves from version information from the assembly defining the given type. This is a shortcut for the following statemnt:
    /// <code>Value = frameworkType.Assembly.GetName ().Version;</code>
    /// </summary>
    /// <param name="frameworkType">A framework type to retrieve the version from.</param>
    public static void RetrieveFromType (Type frameworkType)
    {
      Value = frameworkType.Assembly.GetName ().Version;
    }
  }
}
