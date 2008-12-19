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
