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
using System.Linq;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="ClassReflector"/> is used to build a <see cref="ReflectionBasedClassDefinition"/>.
  /// </summary>
  /// <remarks>Derived classes must have a cosntructor with a matching the <see cref="ClassReflector"/>'s constructor signature. </remarks>
  public class ClassReflector
  {
    private readonly Type _type;
    private readonly IMappingObjectFactory _mappingObjectFactory;
    private readonly IMappingNameResolver _nameResolver;

    public static ClassReflector CreateClassReflector (Type type, IMappingObjectFactory mappingObjectFactory, IMappingNameResolver nameResolver)
    {
      return new ClassReflector (type, mappingObjectFactory, nameResolver);
    }

    public ClassReflector (Type type, IMappingObjectFactory mappingObjectFactory, IMappingNameResolver nameResolver)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));
      ArgumentUtility.CheckNotNull ("mappingObjectFactory", mappingObjectFactory);
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);

      _type = type;
      _mappingObjectFactory = mappingObjectFactory;
      _nameResolver = nameResolver;
    }

    public Type Type
    {
      get { return _type; }
    }

    public IMappingObjectFactory MappingObjectFactory
    {
      get { return _mappingObjectFactory; }
    }

    public IMappingNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    public ReflectionBasedClassDefinition GetMetadata (ReflectionBasedClassDefinition baseClassDefinition)
    {
      var persistentMixinFinder = new PersistentMixinFinder (Type, baseClassDefinition == null);
      var classDefinition = new ReflectionBasedClassDefinition (
          GetID(), Type, IsAbstract(), baseClassDefinition, GetStorageGroupType(), persistentMixinFinder);

      var properties = MappingObjectFactory.CreatePropertyDefinitionCollection (classDefinition, GetPropertyInfos (classDefinition));
      classDefinition.SetPropertyDefinitions (properties);
      var endPoints = MappingObjectFactory.CreateRelationEndPointDefinitionCollection (classDefinition);
      classDefinition.SetRelationEndPointDefinitions (endPoints);

      return classDefinition;
    }

    private string GetID ()
    {
      ClassIDAttribute attribute = AttributeUtility.GetCustomAttribute<ClassIDAttribute> (Type, false);
      if (attribute != null)
        return attribute.ClassID;
      return Type.Name;
    }

    private Type GetStorageGroupType ()
    {
      var storageGroupAttribute = AttributeUtility.GetCustomAttributes<StorageGroupAttribute> (Type, true).FirstOrDefault();
      if (storageGroupAttribute != null)
        return storageGroupAttribute.GetType();
      return null;
    }

    private bool IsAbstract ()
    {
      if (Type.IsAbstract)
        return !Attribute.IsDefined (Type, typeof (InstantiableAttribute), false);

      return false;
    }

    private PropertyInfo[] GetPropertyInfos (ReflectionBasedClassDefinition classDefinition)
    {
      PropertyFinder propertyFinder = new PropertyFinder (
          classDefinition.ClassType,
          classDefinition,
          classDefinition.BaseClass == null,
          true,
          NameResolver,
          classDefinition.PersistentMixinFinder);
      return propertyFinder.FindPropertyInfos();
    }
  }
}