/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections.Generic;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.Transport
{
  /// <summary>
  /// Implements a strategy to import a set of transported <see cref="DomainObject"/> instances from a byte array. The imported objects
  /// should be wrapped as <see cref="TransportItem"/> property holders, the <see cref="DomainObjectImporter"/> class creates 
  /// <see cref="DomainObject"/> instances from those holders and synchronizes them with the database.
  /// </summary>
  /// <remarks>
  /// Supply an implementation of this interface to <see cref="DomainObjectTransporter.LoadTransportData(byte[],IImportStrategy)"/>. The strategy
  /// should match the <see cref="IExportStrategy"/> supplied to <see cref="DomainObjectTransporter.GetBinaryTransportData(IExportStrategy)"/>.
  /// </remarks>
  public interface IImportStrategy
  {
    /// <summary>
    /// Imports the specified data.
    /// </summary>
    /// <param name="data">The data to be imported.</param>
    /// <returns>A stream of <see cref="TransportItem"/> values representing <see cref="DomainObject"/> instances.</returns>
    /// <exception cref="TransportationException">The data could not be imported using this strategy.</exception>
    IEnumerable<TransportItem> Import (byte[] data);
  }
}
