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
    public static ClassReflector CreateClassReflector (Type type, IMappingNameResolver nameResolver)
    {
      return new ClassReflector (type, nameResolver);
    }

    private static ReflectionBasedClassDefinition GetBaseClassDefinition (
        ClassDefinitionCollection classDefinitions, Type type, IMappingNameResolver nameResolver)
    {
      if (ReflectionUtility.IsInheritanceRoot (type))
        return null;

      var classReflector = new ClassReflector (type.BaseType, nameResolver);
      return GetClassDefinition (classDefinitions, classReflector.Type, classReflector.NameResolver, classReflector);
    }

    public ClassReflector (Type type, IMappingNameResolver nameResolver)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);

      Type = type;
      NameResolver = nameResolver;
      PersistentMixinFinder = new PersistentMixinFinder (type, ReflectionUtility.IsInheritanceRoot (Type));
    }

    public PersistentMixinFinder PersistentMixinFinder { get; private set; }
    public Type Type { get; private set; }
    public IMappingNameResolver NameResolver { get; private set; }

    public static ReflectionBasedClassDefinition GetClassDefinition (
        ClassDefinitionCollection classDefinitions, Type classType, IMappingNameResolver mappingNameResolver, ClassReflector classReflector)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      if (classDefinitions.Contains (classType))
        return (ReflectionBasedClassDefinition) classDefinitions.GetMandatory (classType);

      var classDefinition = classReflector.CreateClassDefinition (GetBaseClassDefinition (classDefinitions, classType, mappingNameResolver));
      classDefinitions.Add (classDefinition);

      return classDefinition;
    }

    public ReflectionBasedClassDefinition CreateClassDefinition (ReflectionBasedClassDefinition baseClassDefinition)
    {
      var classDefinition = new ReflectionBasedClassDefinition (
          GetID(), Type, IsAbstract(), baseClassDefinition, GetStorageGroupType(), PersistentMixinFinder);

      CreatePropertyDefinitions (classDefinition, GetPropertyInfos (classDefinition));

      return classDefinition;
    }

    private void CreatePropertyDefinitions (ReflectionBasedClassDefinition classDefinition, IEnumerable<MemberInfo> propertyInfos)
    {
      var propertyDefinitionsForClass = from PropertyInfo propertyInfo in propertyInfos
                                        select (PropertyDefinition) new PropertyReflector (classDefinition, propertyInfo, NameResolver).GetMetadata();
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (propertyDefinitionsForClass, true));
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