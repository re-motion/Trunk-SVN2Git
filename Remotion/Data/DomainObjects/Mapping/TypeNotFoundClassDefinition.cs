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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class TypeNotFoundClassDefinition : ClassDefinition
  {
    private readonly Type _classType;

    public TypeNotFoundClassDefinition (string id, string entityName, string storageProviderID, Type classType)
        : base(id, entityName, storageProviderID)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);
     
      _classType = classType;
    }

    public override Type ClassType
    {
      get { return _classType; }
    }

    public override bool IsClassTypeResolved
    {
      get { return false; }
    }

    public override ClassDefinition BaseClass
    {
      get { return null; }
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

    public override bool IsAbstract
    {
      get { return false; }
    }

    public override ReflectionBasedClassDefinitionValidator GetValidator ()
    {
      throw new NotImplementedException();
    }
  }
}