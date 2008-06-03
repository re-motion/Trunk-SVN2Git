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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.Transport
{
  /// <summary>
  /// Indicates a problem when exporting or importing <see cref="DomainObject"/> instances using <see cref="DomainObjectTransporter"/>. Usually,
  /// the data or objects either don't match the <see cref="IImportStrategy"/> or <see cref="IExportStrategy"/> being used, or the data has become
  /// corrupted.
  /// </summary>
  public class TransportationException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TransportationException"/> class.
    /// </summary>
    public TransportationException ()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransportationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public TransportationException (string message)
        : base (message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransportationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public TransportationException (string message, Exception innerException)
      : base (message, innerException)
    {
    }

  }
}
