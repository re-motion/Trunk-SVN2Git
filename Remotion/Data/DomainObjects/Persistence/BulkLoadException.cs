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
using System.Collections.Generic;
using System.Text;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Thrown when the process of loading multiple objects at the same time fails.
  /// </summary>
  public class BulkLoadException : DomainObjectException
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
