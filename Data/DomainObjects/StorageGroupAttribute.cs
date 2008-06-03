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
  /// <summary>The <see cref="StorageGroupAttribute"/> is the base class for defining storage groups in the domain layer.</summary>
  /// <remarks>
  /// <para>
  /// A storage group is a logical grouping of all classes within a domain that should be persisted with the same storage provider.
  /// </para><para>
  /// Define the <see cref="StorageGroupAttribute"/> at the base classes of the domain layer to signify the root for the persistence hierarchy.
  /// </para><para>
  /// If no storage group is deifned for a persistence hierarchy, the domain object classes are assigned to the default storage provider.
  /// </para> 
  /// </remarks>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public abstract class StorageGroupAttribute: Attribute
  {
    protected StorageGroupAttribute()
    {
    }
  }
}
