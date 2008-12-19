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

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// Indicates the state of a <see cref="DomainObject"/>.
/// </summary>
public enum StateType
{
  /// <summary>
  /// The <see cref="DomainObject"/> has not changed since it was loaded.
  /// </summary>
  Unchanged = 0,
  /// <summary>
  /// The <see cref="DomainObject"/> has been changed since it was loaded.
  /// </summary>
  Changed = 1,
  /// <summary>
  /// The <see cref="DomainObject"/> has been instanciated and has not been committed.
  /// </summary>
  New = 2,
  /// <summary>
  /// The <see cref="DomainObject"/> has been deleted.
  /// </summary>
  Deleted = 3,
  /// <summary>
  /// The <see cref="DomainObject"/> does not exist any longer.
  /// </summary>
  Discarded = 4
}
}
