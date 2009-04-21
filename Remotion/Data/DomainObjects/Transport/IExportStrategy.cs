// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
namespace Remotion.Data.DomainObjects.Transport
{
  /// <summary>
  /// Implements a strategy to export a set of <see cref="DomainObject"/> instances to a byte array. The exported objects are
  /// wrapped as <see cref="TransportItem"/> property holders by the <see cref="DomainObjectTransporter"/> class.
  /// </summary>
  /// <remarks>
  /// Supply an implementation of this interface to <see cref="DomainObjectTransporter.GetBinaryTransportData(IExportStrategy)"/>. The strategy
  /// must match the <see cref="IImportStrategy"/> supplied to <see cref="DomainObjectTransporter.LoadTransportData(byte[],IImportStrategy)"/>.
  /// </remarks>
  public interface IExportStrategy
  {
    /// <summary>
    /// Exports the specified transported objects.
    /// </summary>
    /// <param name="transportedObjects">The objects to be exported.</param>
    /// <returns>A byte array representing the transported objects.</returns>
    /// <exception cref="TransportationException">The objects could not be exported using this strategy.</exception>
    byte[] Export (TransportItem[] transportedObjects);
  }
}
