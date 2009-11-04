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
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Indicates whether a <see cref="DomainObject"/> was loaded as a whole or if only its <see cref="DataContainer"/> was loaded.
  /// </summary>
  public enum LoadMode
  {
    /// <summary>
    /// The whole object has been loaded, e.g. as a reaction to <see cref="DomainObject.GetObject{T}(ObjectID)"/>.
    /// </summary>
    WholeDomainObjectInitialized,
    /// <summary>
    /// Only the object's <see cref="DataContainer"/> has been loaded, e.g. as a reaction to <see cref="ClientTransaction.EnlistDomainObject"/> or
    /// in a substransaction.
    /// </summary>
    DataContainerLoadedOnly
  }
}
