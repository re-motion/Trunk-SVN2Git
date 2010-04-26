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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary><see cref="ClassDefinition"/> used when loading the mapping from the reflection meta data.</summary>
  [Serializable]
  public class ReflectionBasedClassDefinition: ClassDefinition
  {
    private static void CheckBaseClass (ClassDefinition baseClass, string id, string storageProviderID, Type classType)
    {
      if (classType != null && baseClass.ClassType != null && !classType.IsSubclassOf (baseClass.ClassType))
      {
        throw CreateMappingException (
            "Type '{0}' of class '{1}' is not derived from type '{2}' of base class '{3}'.",
            classType.AssemblyQualifiedName,
            id,
            baseClass.ClassType.AssemblyQualifiedName,
            baseClass.ID);
      }

      if (baseClass.StorageProviderID != storageProviderID)
      {
        throw CreateMappingException (
            "Cannot derive class '{0}' from base class '{1}' handled by different StorageProviders.",
            id,
            baseClass.ID);
      }
    }

    private static MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }

    [NonSerialized]
    private readonly bool _isAbstract;
    [NonSerialized]
    private readonly Type _classType;
    [NonSerialized]
    private readonly IPersistentMixinFinder _persistentMixinFinder;

    [NonSerialized]
    private readonly ReflectionBasedClassDefinition _baseClass;
    [NonSerialized]
    private ClassDefinitionCollection _derivedClasses;

    public ReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract, ReflectionBasedClassDefinition baseClass, IPersistentMixinFinder persistentMixinFinder)
        : base (id, entityName, storageProviderID)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);
      ArgumentUtility.CheckNotNull ("persistentMixins", persistentMixinFinder);
      if (!classType.IsSubclassOf (typeof (DomainObject)))
        throw CreateMappingException ("Type '{0}' of class '{1}' is not derived from 'Remotion.Data.DomainObjects.DomainObject'.", classType, ID);

      _classType = classType;
      _persistentMixinFinder = persistentMixinFinder;
      _isAbstract = isAbstract;

      _derivedClasses = new ClassDefinitionCollection (new ClassDefinitionCollection (true), true);

      if (baseClass != null)
      {
        CheckBaseClass (baseClass, id, storageProviderID, classType);

        _baseClass = baseClass;
        baseClass.AddDerivedClass (this);
      }
    }

    public override ClassDefinition BaseClass
    {
      get { return _baseClass; }
    }

    public ReflectionBasedClassDefinition ReflectionBasedBaseClass
    {
      get { return _baseClass; }
    }

    public override ClassDefinitionCollection DerivedClasses
    {
      get { return _derivedClasses; }
    }

    public IPersistentMixinFinder PersistentMixinFinder
    {
      get { return _persistentMixinFinder; }
    }

    public IEnumerable<Type> PersistentMixins
    {
      get { return _persistentMixinFinder.GetPersistentMixins(); }
    }

    public override bool IsAbstract
    {
      get { return _isAbstract; }
    }

    public override Type ClassType
    {
      get { return _classType; }
    }

    public override bool IsClassTypeResolved
    {
      get { return true; }
    }

    public Type GetPersistentMixin (Type mixinToSearch)
    {
      ArgumentUtility.CheckNotNull ("mixinToSearch", mixinToSearch);
      if (PersistentMixins.Contains (mixinToSearch))
        return mixinToSearch;
      else
      {
        foreach (Type mixin in PersistentMixins)
        {
          if (mixinToSearch.IsAssignableFrom (mixin))
            return mixin;
        }
        return null;
      }
    }

    public override ClassDefinitionValidator GetValidator ()
    {
      return new ReflectionBasedClassDefinitionValidator (this);
    }

    public override IDomainObjectCreator GetDomainObjectCreator ()
    {
      return InterceptedDomainObjectCreator.Instance;
    }

    public override PropertyDefinition ResolveProperty (PropertyInfo property)
    {
      string propertyIdentifier = MappingConfiguration.Current.NameResolver.GetPropertyName (property);
      var propertyDefinition = GetPropertyDefinition (propertyIdentifier);
      if (propertyDefinition != null)
        return propertyDefinition;

      return GetMixinPropertyDefinition (this, property);
    }

    private PropertyDefinition GetMixinPropertyDefinition (ReflectionBasedClassDefinition classDefinition, PropertyInfo property)
    {
      if (classDefinition != null)
      {
        foreach (var mixin in classDefinition.PersistentMixins)
        {
          if (property.DeclaringType.IsAssignableFrom (mixin))
          {
            string propertyIdentifier = MappingConfiguration.Current.NameResolver.GetPropertyName (mixin, property.Name);
            return classDefinition.GetPropertyDefinition (propertyIdentifier);
          }
        }

        if (MappingConfiguration.Current.ClassDefinitions.Contains (classDefinition.ClassType.BaseType))
          return MappingConfiguration.Current.ClassDefinitions[classDefinition.ClassType.BaseType].ResolveProperty (property);
      }

      return null;
    }

    private void AddDerivedClass (ClassDefinition derivedClass)
    {
      var derivedClasses = new ClassDefinitionCollection (_derivedClasses, false);
      derivedClasses.Add (derivedClass);
      _derivedClasses = new ClassDefinitionCollection (derivedClasses, true);
    }
  }
}
