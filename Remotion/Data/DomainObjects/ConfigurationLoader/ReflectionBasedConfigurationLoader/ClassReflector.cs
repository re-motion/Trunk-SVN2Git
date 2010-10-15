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
using System.Reflection;
using System.Text;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Reflection;
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
      PersistentMixinFinder = new PersistentMixinFinder (type, IsInheritanceRoot(Type));
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
      var genericTypeValidationRule = new DomainObjectTypeIsNotGenericValidationRule();
      var genericTypeValidationResult = genericTypeValidationRule.Validate (Type);
      if (!genericTypeValidationResult.IsValid)
        throw CreateMappingException (null, Type.GetGenericTypeDefinition(), genericTypeValidationResult.Message);

      var legacyCtorValidationRule = new DomainObjectTypeDoesNotHaveLegacyInfrastructureConstructorValidationRule();
      var legacyCtorValidationResult = legacyCtorValidationRule.Validate (Type);
      if (!legacyCtorValidationResult.IsValid)
        throw CreateMappingException (null, Type, legacyCtorValidationResult.Message);

      var storageGroupValidationRule = new StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule ();
      var storageGroupValidationResult = storageGroupValidationRule.Validate (Type);
      if (!storageGroupValidationResult.IsValid)
        throw CreateMappingException (null, Type, storageGroupValidationResult.Message);
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

    //TODO: COMMONS-842
    //TODO: Move type resolving to storagegrouplist and unify with QueryConfigurationLoader
    //TODO: Test for DefaultStorageProvider
    private string GetStorageProviderID ()
    {
      StorageGroupAttribute storageGroupAttribute = AttributeUtility.GetCustomAttribute<StorageGroupAttribute> (Type, true);
      var defaultStorageProviderDefinition = DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition;
      if (storageGroupAttribute == null)
      {
        //TODO COMMONS-783: Test exception
        if (defaultStorageProviderDefinition == null)
          throw DomainObjectsConfiguration.Current.Storage.CreateMissingDefaultProviderException (null);
        return defaultStorageProviderDefinition.Name;
      }

      string storageGroupName = TypeUtility.GetPartialAssemblyQualifiedName (storageGroupAttribute.GetType());
      StorageGroupElement storageGroup = DomainObjectsConfiguration.Current.Storage.StorageGroups[storageGroupName];
      if (storageGroup == null)
      {
        //TODO COMMONS-783: Test exception
        if (defaultStorageProviderDefinition == null)
          throw DomainObjectsConfiguration.Current.Storage.CreateMissingDefaultProviderException (null);
        return defaultStorageProviderDefinition.Name;
      }
      return storageGroup.StorageProviderName;
    }

    private bool IsAbstract ()
    {
      if (Type.IsAbstract)
        return !Attribute.IsDefined (Type, typeof (InstantiableAttribute), false);

      return false;
    }

    public static bool IsInheritanceRoot (Type type) //TODO: move to reflection utility
    {
      if (IsDomainObjectBase (type.BaseType))
        return true;

      return Attribute.IsDefined (type, typeof (StorageGroupAttribute), false);
    }

    //TODO COMMONS-825: Refactor this
    //TODO COMMONS-839: Refactor this
    public static bool IsDomainObjectBase (Type type) //TODO: move to reflection utility
    {
      //TODO: argument check

      return type.Assembly == typeof (DomainObject).Assembly;
    }

    private ReflectionBasedClassDefinition GetBaseClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      if (IsInheritanceRoot(Type))
        return null;

      ClassReflector classReflector = (ClassReflector) TypesafeActivator.CreateInstance (GetType()).With (Type.BaseType, NameResolver);
      return classReflector.GetClassDefinition (classDefinitions);
    }

    private PropertyInfo[] GetPropertyInfos (ReflectionBasedClassDefinition classDefinition)
    {
      PropertyFinder propertyFinder = new PropertyFinder (Type, IsInheritanceRoot (Type), NameResolver);
      return propertyFinder.FindPropertyInfos (classDefinition);
    }

    private PropertyInfo[] GetRelationPropertyInfos (ReflectionBasedClassDefinition classDefinition, PersistentMixinFinder persistentMixinFinder)
    {
      RelationPropertyFinder relationPropertyFinder = new RelationPropertyFinder (Type, IsInheritanceRoot (Type), NameResolver);
      return relationPropertyFinder.FindPropertyInfos (classDefinition);
    }
  }
}
