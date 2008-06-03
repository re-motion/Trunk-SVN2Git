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
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Design;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.Design;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  [DesignModeMappingLoader(typeof (FakeDesignModeMappingLoader))]
  public class FakeMappingLoader: IMappingLoader
  {
    public FakeMappingLoader()
    {
    }

    public ClassDefinitionCollection GetClassDefinitions()
    {
      return new ClassDefinitionCollection();
    }

    public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      return new RelationDefinitionCollection();
    }

    public bool ResolveTypes
    {
      get { return false; }
    }
  }
}
