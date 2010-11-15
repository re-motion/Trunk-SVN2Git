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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary><see cref="ClassDefinition"/> used when loading the mapping from the reflection meta data.</summary>
  [Serializable]
  public class ReflectionBasedClassDefinition : ClassDefinition
  {
    [NonSerialized]
    private readonly InterlockedCache<IPropertyInformation, PropertyDefinition> _propertyDefinitionCache;

    [NonSerialized]
    private readonly InterlockedCache<IPropertyInformation, IRelationEndPointDefinition> _relationDefinitionCache;

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

    public ReflectionBasedClassDefinition (
        string id,
        string entityName,
        string storageProviderID,
        Type classType,
        bool isAbstract,
        ReflectionBasedClassDefinition baseClass,
        IPersistentMixinFinder persistentMixinFinder)
        : base (id, entityName, storageProviderID)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);
      ArgumentUtility.CheckNotNull ("persistentMixinFinder", persistentMixinFinder);
      
      _classType = classType;
      _persistentMixinFinder = persistentMixinFinder;
      _isAbstract = isAbstract;

      _derivedClasses = new ClassDefinitionCollection (new ClassDefinitionCollection (true), true);
      _propertyDefinitionCache = new InterlockedCache<IPropertyInformation, PropertyDefinition>();
      _relationDefinitionCache = new InterlockedCache<IPropertyInformation, IRelationEndPointDefinition>();

      if (baseClass != null)
      {
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

    public override PropertyDefinition ResolveProperty (IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);

      return _propertyDefinitionCache.GetOrCreateValue (
          propertyInformation,
          key => ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (key, this, GetPropertyDefinition));
    }

    public override IRelationEndPointDefinition ResolveRelationEndPoint (IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);

      return _relationDefinitionCache.GetOrCreateValue (
          propertyInformation,
          key => ReflectionBasedPropertyResolver.ResolveDefinition<IRelationEndPointDefinition> (key, this, GetRelationEndPointDefinition));
    }

    public override ReflectionBasedClassDefinitionValidator GetValidator ()
    {
      return new ReflectionBasedClassDefinitionValidator (this);
    }

    public override IDomainObjectCreator GetDomainObjectCreator ()
    {
      return InterceptedDomainObjectCreator.Instance;
    }

    private void AddDerivedClass (ClassDefinition derivedClass)
    {
      var derivedClasses = new ClassDefinitionCollection (_derivedClasses, false);
      derivedClasses.Add (derivedClass);
      _derivedClasses = new ClassDefinitionCollection (derivedClasses, true);
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }
   
  }
}