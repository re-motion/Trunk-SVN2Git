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
  /// The <see cref="DBStorageGroupAttribute"/> is the standard <see cref="StorageGroupAttribute"/> for types persisted into a database.
  /// </summary>
  /// <remarks>
  /// The <see cref="DBStorageGroupAttribute"/> can be used whenever there is no need for a more granular distribution of types into different 
  /// storage groups.
  /// </remarks>
  public class DBStorageGroupAttribute : StorageGroupAttribute
  {
    public DBStorageGroupAttribute()
    {
    }
  }
}
