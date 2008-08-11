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
using System.Text;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Thrown when the process of loading multiple objects at the same time fails.
  /// </summary>
  public class BulkLoadException : Exception
  {
    /// <summary>
    /// The exceptions that occurred while the objects were loaded.
    /// </summary>
    private readonly List<Exception> _exceptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="BulkLoadException"/> class.
    /// </summary>
    /// <param name="exceptions">The exceptions thrown while the objects were loaded.</param>
    public BulkLoadException (IEnumerable<Exception> exceptions)
        : base (CreateMessage (exceptions))
    {
      _exceptions = new List<Exception> (exceptions);
    }

    /// <summary>
    /// The exceptions that occurred while the objects were loaded.
    /// </summary>
    public List<Exception> Exceptions
    {
      get { return _exceptions; }
    }

    private static string CreateMessage (IEnumerable<Exception> exceptions)
    {
      StringBuilder message = new StringBuilder("There were errors when loading a bulk of DomainObjects:");
      message.AppendLine();
      foreach (Exception exception in exceptions)
        message.AppendLine (exception.Message);
      return message.ToString();
    }
  }
}
