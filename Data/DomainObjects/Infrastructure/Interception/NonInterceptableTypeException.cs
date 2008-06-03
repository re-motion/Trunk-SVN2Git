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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.Infrastructure.Interception
{
  /// <summary>
  /// This exception is thrown when the property interception mechanism cannot be applied to a specific <see cref="DomainObject"/> type
  /// because of problems with that type's declaration.
  /// </summary>
  public class NonInterceptableTypeException : Exception
  {
    /// <summary>
    /// The type that cannot be intercepted.
    /// </summary>
    public readonly Type Type;

    /// <summary>
    /// Initializes a new instance of the <see cref="NonInterceptableTypeException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="type">The type that cannot be intercepted.</param>
    public NonInterceptableTypeException (string message, Type type)
        : base (message)
    {
      Type = type;
    }
  }
}
