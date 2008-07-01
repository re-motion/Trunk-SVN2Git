/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects
{
  /// <summary>Defines the property as managed in the <see cref="ClientTransaction"/> but not-persisted in the underlying data store.</summary>
  public sealed class StorageClassTransactionAttribute : StorageClassAttribute
  {
    public StorageClassTransactionAttribute()
        : base (StorageClass.Transaction)
    {
    }
  }
}
