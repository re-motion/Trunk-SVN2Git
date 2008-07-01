/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>The storage class options available in the persistence framework.</summary>
  public enum StorageClass
  {
    /// <summary>The property is persistet into the data store.</summary>
    Persistent,
    /// <summary>The property is managed by the <see cref="ClientTransaction"/>, but not persistet.</summary>
    Transaction,
    /// <summary>The property is ignored by the persistence framework.</summary>
    None
  }
}