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
