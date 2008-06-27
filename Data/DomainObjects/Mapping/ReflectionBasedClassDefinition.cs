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
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Text;
using Remotion.Utilities;
using Remotion.Collections;

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
    private readonly ICollection<Type> _persistentMixins;

    public ReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract, ICollection<Type> persistentMixins)
        : this (id, entityName, storageProviderID, classType, isAbstract, null, persistentMixins)
    {
    }

    public ReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract, ReflectionBasedClassDefinition baseClass, ICollection<Type> persistentMixins)
        : base (id, entityName, storageProviderID, true)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);
      ArgumentUtility.CheckNotNull ("persistentMixins", persistentMixins);
      if (!classType.IsSubclassOf (typeof (DomainObject)))
        throw CreateMappingException ("Type '{0}' of class '{1}' is not derived from 'Remotion.Data.DomainObjects.DomainObject'.", classType, ID);
     
      _classType = classType;
      _persistentMixins = new Set<Type> (persistentMixins);
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

    public IEnumerable<Type> PersistentMixins
    {
      get { return _persistentMixins; }
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

    public bool HasPersistentMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      if (_persistentMixins.Contains (mixinType))
        return true;
      else
      {
        foreach (Type mixin in _persistentMixins)
        {
          if (mixinType.IsAssignableFrom (mixin))
            return true;
        }
        return false;
      }
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }

    protected internal override IDomainObjectCreator GetDomainObjectCreator ()
    {
      return FactoryBasedDomainObjectCreator.Instance;
    }

    public override void ValidateCurrentMixinConfiguration ()
    {
      base.ValidateCurrentMixinConfiguration ();
      Set<Type> currentMixins = new Set<Type> (new PersistentMixinFinder (ClassType).GetPersistentMixins ());
      foreach (Type t in _persistentMixins)
      {
        if (!currentMixins.Contains (t))
        {
          string message = string.Format ("A persistence-related mixin was removed from the domain object type {0} after the mapping "
            + "information was built: {1}.", ClassType.FullName, t.FullName);
          throw new MappingException (message);
        }
        currentMixins.Remove (t);
      }
      if (currentMixins.Count > 0)
      {
        string message = string.Format ("One or more persistence-related mixins were added to the domain object type {0} after the mapping "
            + "information was built: {1}.", ClassType.FullName, SeparatedStringBuilder.Build (", ", currentMixins));
        throw new MappingException (message);
      }
    }
  }
}
