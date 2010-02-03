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

namespace Remotion.Data.DomainObjects.DomainImplementation
{
  /// <summary>
  /// Defines how an unload operation should treat transactions that are part of a hierarchy.
  /// </summary>
  public enum UnloadTransactionMode
  {
    /// <summary>
    /// Affect the given transaction only.
    /// </summary>
    ThisTransactionOnly,
    /// <summary>
    /// Affect this transaction and all of its parent transactions, up to the root transaction. This parent transactions are temporarily
    /// made writeable while the operation is executing. The recursive operation is not atomic; if an error occurs in one of the parent 
    /// transactions, the effects of the operation in the subtransactions will already have taken place.
    /// </summary>
    RecurseToRoot
  }
}