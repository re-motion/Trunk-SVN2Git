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
using System.Reflection;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class TypeNotFoundClassDefinition : ReflectionBasedClassDefinition
  {
    private readonly PropertyInfo _relationProperty;

    public TypeNotFoundClassDefinition (
        string id, IStorageEntityDefinition storageEntityDefinition, string storageProviderID, Type classType, PropertyInfo relationProperty)
        : base (id, storageEntityDefinition, storageProviderID, classType, false, null, new PersistentMixinFinder (classType))
    {
      ArgumentUtility.CheckNotNull ("relationProperty", relationProperty);

      _relationProperty = relationProperty;
    }

    public PropertyInfo RelationProperty
    {
      get { return _relationProperty; }
    }

    public override bool IsClassTypeResolved
    {
      get { return false; }
    }

    public override ClassDefinitionCollection DerivedClasses
    {
      get { throw new NotImplementedException(); }
    }

    public override IDomainObjectCreator GetDomainObjectCreator ()
    {
      throw new NotImplementedException();
    }

    public override PropertyDefinition ResolveProperty (IPropertyInformation propertyInformation)
    {
      throw new NotImplementedException();
    }

    public override IRelationEndPointDefinition ResolveRelationEndPoint (IPropertyInformation propertyInformation)
    {
      throw new NotImplementedException();
    }

    public override ReflectionBasedClassDefinitionValidator GetValidator ()
    {
      throw new NotImplementedException();
    }
  }
}