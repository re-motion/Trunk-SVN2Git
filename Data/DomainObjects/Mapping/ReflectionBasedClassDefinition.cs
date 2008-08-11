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
using System.Linq;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary><see cref="ClassDefinition"/> used when loading the mapping from the reflection meta data.</summary>
  [Serializable]
  public class ReflectionBasedClassDefinition: ClassDefinition
  {
    [NonSerialized]
    private readonly bool _isAbstract;
    [NonSerialized]
    private readonly Type _classType;
    [NonSerialized]
    private readonly IPersistentMixinFinder _persistentMixinFinder;

    public ReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract, ReflectionBasedClassDefinition baseClass, IPersistentMixinFinder persistentMixinFinder)
        : base (id, entityName, storageProviderID, true)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);
      ArgumentUtility.CheckNotNull ("persistentMixins", persistentMixinFinder);
      if (!classType.IsSubclassOf (typeof (DomainObject)))
        throw CreateMappingException ("Type '{0}' of class '{1}' is not derived from 'Remotion.Data.DomainObjects.DomainObject'.", classType, ID);
     
      _classType = classType;
      _persistentMixinFinder = persistentMixinFinder;
      _isAbstract = isAbstract;

      if (baseClass != null)
      {
        // Note: CheckBasePropertyDefinitions does not have to be called, because member _propertyDefinitions is
        //       initialized to an empty collection during construction.
        SetBaseClass (baseClass);
      }
    }

    public new ReflectionBasedClassDefinition BaseClass
    {
      get { return (ReflectionBasedClassDefinition) base.BaseClass; }
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

#warning TODO: Move to PersistentMixinFinder
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

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }

    protected internal override IDomainObjectCreator GetDomainObjectCreator ()
    {
      return FactoryBasedDomainObjectCreator.Instance;
    }
  }
}
