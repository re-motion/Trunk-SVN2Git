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
using System.Reflection;
using System.Text;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Reflection;
using Remotion.Utilities;
using TypeUtility=Remotion.Utilities.TypeUtility;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="ClassReflector"/> is used to build a <see cref="ReflectionBasedClassDefinition"/> and the <see cref="RelationDefinition"/> 
  /// objects for a type.
  /// </summary>
  /// <remarks>Derived classes must have a cosntructor with a matching the <see cref="ClassReflector"/>'s constructor signature. </remarks>
  public class ClassReflector
  {
    public static ClassReflector CreateClassReflector (Type type, IMappingNameResolver nameResolver)
    {
      return new RdbmsClassReflector (type, nameResolver);
    }

    public ClassReflector (Type type, IMappingNameResolver nameResolver)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);

      Type = type;
      NameResolver = nameResolver;
      PersistentMixinFinder = new PersistentMixinFinder (type, IsInheritanceRoot());
    }

    public PersistentMixinFinder PersistentMixinFinder { get; private set; }
    public Type Type { get; private set; }
    public IMappingNameResolver NameResolver { get; private set; }

    public ReflectionBasedClassDefinition GetClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      if (classDefinitions.Contains (Type))
        return (ReflectionBasedClassDefinition) classDefinitions.GetMandatory (Type);

      ReflectionBasedClassDefinition classDefinition = CreateClassDefinition (classDefinitions);
      classDefinitions.Add (classDefinition);

      return classDefinition;
    }

    public List<RelationDefinition> GetRelationDefinitions (
        ClassDefinitionCollection classDefinitions, RelationDefinitionCollection relationDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("relationDefinitions", relationDefinitions);

      List<RelationDefinition> relations = new List<RelationDefinition>();
      ReflectionBasedClassDefinition classDefinition = (ReflectionBasedClassDefinition) classDefinitions.GetMandatory (Type);

      foreach (PropertyInfo propertyInfo in GetRelationPropertyInfos (classDefinition, PersistentMixinFinder))
      {
        RelationReflector relationReflector = RelationReflector.CreateRelationReflector (classDefinition, propertyInfo, NameResolver);
        RelationDefinition relationDefinition = relationReflector.GetMetadata (classDefinitions, relationDefinitions);
        if (relationDefinition != null)
          relations.Add (relationDefinition);
      }

      return relations;
    }

    protected MappingException CreateMappingException (Exception innerException, Type type, string message, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);

      StringBuilder messageBuilder = new StringBuilder();
      messageBuilder.AppendFormat (message, args);
      messageBuilder.AppendLine();
      messageBuilder.AppendFormat ("Type: {0}", type.FullName);

      return new MappingException (messageBuilder.ToString(), innerException);
    }

    private ReflectionBasedClassDefinition CreateClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      ValidateType();

      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          GetID(),
          GetStorageSpecificIdentifier(),
          GetStorageProviderID(),
          Type,
          IsAbstract(),
          GetBaseClassDefinition (classDefinitions),
          PersistentMixinFinder);

      CreatePropertyDefinitions (classDefinition, GetPropertyInfos (classDefinition));

      return classDefinition;
    }

    //TODO: Add constructor checks
    private void ValidateType ()
    {
      if (Type.IsGenericType && !IsDomainObjectBase (Type))
        throw CreateMappingException (null, Type.GetGenericTypeDefinition(), "Generic domain objects are not supported.");
      
      if (!IsAbstract())
      {
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.ExactBinding;
        ConstructorInfo legacyLoadConstructor = Type.GetConstructor (flags, null, new Type[] {typeof (DataContainer)}, null);
        if (legacyLoadConstructor != null)
        {
          throw CreateMappingException (
              null,
              Type,
              "The domain object type has a legacy infrastructure constructor for loading (a nonpublic constructor taking a single DataContainer "
              + "argument). The reflection-based mapping does not use this constructor any longer and requires it to be removed.");
        }
      }

      if (IsInheritanceRoot() && Attribute.IsDefined (Type.BaseType, typeof (StorageGroupAttribute), true))
      {
        Type baseType = Type.BaseType;
        while (!AttributeUtility.IsDefined<StorageGroupAttribute> (baseType, false))
          baseType = baseType.BaseType;

        throw CreateMappingException (
            null,
            Type,
            "The domain object type cannot redefine the '{0}' already defined on base type '{1}'.",
            typeof (StorageGroupAttribute),
            baseType);
      }
    }

    private void CreatePropertyDefinitions (ReflectionBasedClassDefinition classDefinition, MemberInfo[] propertyInfos)
    {
      foreach (PropertyInfo propertyInfo in propertyInfos)
      {
        PropertyReflector propertyReflector = new PropertyReflector (classDefinition, propertyInfo, NameResolver);
        classDefinition.MyPropertyDefinitions.Add (propertyReflector.GetMetadata());
      }
    }

    private string GetID ()
    {
      ClassIDAttribute attribute = AttributeUtility.GetCustomAttribute<ClassIDAttribute> (Type, false);
      if (attribute != null)
        return attribute.ClassID;
      return Type.Name;
    }

    protected virtual string GetStorageSpecificIdentifier ()
    {
      IStorageSpecificIdentifierAttribute attribute = AttributeUtility.GetCustomAttribute<IStorageSpecificIdentifierAttribute> (Type, false);
      if (attribute != null && !string.IsNullOrEmpty (attribute.Identifier))
        return attribute.Identifier;
      return GetID();
    }

    //TODO: Move type resolving to storagegrouplist
    //TODO: Test for DefaultStorageProvider
    private string GetStorageProviderID ()
    {
      StorageGroupAttribute storageGroupAttribute = AttributeUtility.GetCustomAttribute<StorageGroupAttribute> (Type, true);
      if (storageGroupAttribute == null)
        return DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition.Name;

      string storageGroupName = TypeUtility.GetPartialAssemblyQualifiedName (storageGroupAttribute.GetType());
      StorageGroupElement storageGroup = DomainObjectsConfiguration.Current.Storage.StorageGroups[storageGroupName];
      if (storageGroup == null)
        return DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition.Name;
      return storageGroup.StorageProviderName;
    }

    private bool IsAbstract ()
    {
      if (Type.IsAbstract)
        return !Attribute.IsDefined (Type, typeof (InstantiableAttribute), false);

      return false;
    }

    private bool IsInheritanceRoot ()
    {
      if (IsDomainObjectBase (Type.BaseType))
        return true;

      return Attribute.IsDefined (Type, typeof (StorageGroupAttribute), false);
    }

    private bool IsDomainObjectBase (Type type)
    {
      return type == typeof (DomainObject) || (type.IsGenericType && type.GetGenericTypeDefinition () == typeof (SimpleDomainObject<>));
    }

    private ReflectionBasedClassDefinition GetBaseClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      if (IsInheritanceRoot())
        return null;

      ClassReflector classReflector = (ClassReflector) TypesafeActivator.CreateInstance (GetType()).With (Type.BaseType, NameResolver);
      return classReflector.GetClassDefinition (classDefinitions);
    }

    private PropertyInfo[] GetPropertyInfos (ReflectionBasedClassDefinition classDefinition)
    {
      PropertyFinder propertyFinder = new PropertyFinder (Type, IsInheritanceRoot (), NameResolver);
      return propertyFinder.FindPropertyInfos (classDefinition);
    }

    private PropertyInfo[] GetRelationPropertyInfos (ReflectionBasedClassDefinition classDefinition, PersistentMixinFinder persistentMixinFinder)
    {
      RelationPropertyFinder relationPropertyFinder = new RelationPropertyFinder (Type, IsInheritanceRoot (), NameResolver);
      return relationPropertyFinder.FindPropertyInfos (classDefinition);
    }
  }
}
