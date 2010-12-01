// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  public class FakeMappingLoader : IMappingLoader
  {
    public FakeMappingLoader ()
    {
    }

    public IEnumerable<ClassDefinition> GetClassDefinitions ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Fake",
          "Fake",
          new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider", typeof (UnitTestStorageObjectFactoryStub)),
          typeof (Company),
          false);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      return new[] { classDefinition };
    }

    public IEnumerable<RelationDefinition> GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      return new RelationDefinition[0];
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