// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.Infrastructure.Interception
{
  /// <summary>
  /// This exception is thrown when the property interception mechanism cannot be applied to a specific <see cref="DomainObject"/> type
  /// because of problems with that type's declaration.
  /// </summary>
  public class NonInterceptableTypeException : DomainObjectException
  {
    private readonly Type _type;

    /// <summary>
    /// Initializes a new instance of the <see cref="NonInterceptableTypeException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="type">The type that cannot be intercepted.</param>
    public NonInterceptableTypeException (string message, Type type)
        : base (message)
    {
      _type = type;
    }

    /// <summary>
    /// The type that cannot be intercepted.
    /// </summary>
    public Type Type
    {
      get { return _type; }
    }
  }
}
