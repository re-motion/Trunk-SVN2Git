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
using System.ComponentModel.Design;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Design
{
  public class FakeDesignModeMappingLoader : IMappingLoader
  {
    private readonly IDesignerHost _designerHost;

    public FakeDesignModeMappingLoader (IDesignerHost site)
    {
      _designerHost = site;
    }

    public IDesignerHost DesignerHost
    {
      get { return _designerHost; }
    }

    public ClassDefinitionCollection GetClassDefinitions ()
    {
      ClassDefinitionCollection classDefinitionCollection = new ClassDefinitionCollection();
      classDefinitionCollection.Add (
          new ReflectionBasedClassDefinition ("Fake", "Fake", "Fake", typeof (Company), false, new List<Type>()));

      return classDefinitionCollection;
    }

    public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      return new RelationDefinitionCollection();
    }

    bool IMappingLoader.ResolveTypes
    {
      get { return true; }
    }
  }
}
