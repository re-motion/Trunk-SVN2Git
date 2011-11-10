// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  public class FakeMappingLoader : IMappingLoader
  {
    public FakeMappingLoader ()
    {
    }

    public ClassDefinition[] GetClassDefinitions ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ("Fake", typeof (Company));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      return new[] { classDefinition };
    }

    public RelationDefinition[] GetRelationDefinitions (IDictionary<Type, ClassDefinition> classDefinitions)
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

    public IClassDefinitionValidator CreateClassDefinitionValidator ()
    {
      return new ClassDefinitionValidator ();
    }

    public IPropertyDefinitionValidator CreatePropertyDefinitionValidator ()
    {
      return new PropertyDefinitionValidator ();
    }

    public IRelationDefinitionValidator CreateRelationDefinitionValidator ()
    {
      return new RelationDefinitionValidator ();
    }

    public ISortExpressionValidator CreateSortExpressionValidator ()
    {
      return new SortExpressionValidator ();
    }
  }
}