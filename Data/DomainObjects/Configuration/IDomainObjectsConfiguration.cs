/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Configuration;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.Configuration
{
  /// <summary>
  /// The <see cref="IDomainObjectsConfiguration"/> interface is an abstraction for the <see cref="ConfigurationSectionGroup"/> and the fake 
  /// implementation of the domain objects configuration.
  /// </summary>
  public interface IDomainObjectsConfiguration
  {
    MappingLoaderConfiguration MappingLoader { get; }

    StorageConfiguration Storage { get; }

    QueryConfiguration Query { get; }
  }
}
