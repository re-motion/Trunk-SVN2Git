// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  public class FakeMappingLoader : IMappingLoader
  {
    public FakeMappingLoader ()
    {
    }

    public ClassDefinitionCollection GetClassDefinitions ()
    {
      ClassDefinitionCollection classDefinitionCollection = new ClassDefinitionCollection();
      classDefinitionCollection.Add (
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Fake", "Fake", "Fake", typeof (Company), false));

      return classDefinitionCollection;
    }

    public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      return new RelationDefinitionCollection();
    }

    public bool ResolveTypes
    {
      get { return false; }
    }

    public IMappingNameResolver NameResolver
    {
      get { return new ReflectionBasedNameResolver(); }
    }
  }
}
