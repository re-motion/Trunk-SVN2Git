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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.PropertyFinderTests
{
  public class StubPropertyFinderBase : PropertyFinderBase
  {
    public StubPropertyFinderBase (Type type, bool includeBaseProperties)
      : this (type, includeBaseProperties, PersistentMixinFinder.GetPersistentMixins (type))
    {
    }

    public StubPropertyFinderBase (Type type, bool includeBaseProperties, IEnumerable<Type> persistentMixins)
      : base (type, includeBaseProperties, persistentMixins)
    {
    }
  }
}
