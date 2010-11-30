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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [Serializable]
  [DebuggerDisplay ("{GetType().Name} for {ClassType.FullName}")]
  public abstract class ClassDefinition : SerializableMappingObject
  {
    // types

    // serialized member fields
    // Note: ClassDefinitions can only be serialized if they are part of the current mapping configuration. Only the fields listed below
    // will be serialized; these are used to retrieve the "real" object at deserialization time.

    private readonly string _id;

    private bool _isReadOnly;

    // nonserialized readonyly member fields

    [NonSerialized]
    private readonly Type _storageGroupType;

    [NonSerialized]
    private readonly PropertyAccessorDataCache _propertyAccessorDataCache;

    [NonSerialized]
    private readonly DoubleCheckedLockingContainer<RelationDefinitionCollection> _cachedRelationDefinitions;

    [NonSerialized]
    private readonly DoubleCheckedLockingContainer<ReadOnlyDictionarySpecific<string, IRelationEndPointDefinition>> _cachedRelationEndPointDefinitions;

    [NonSerialized]
    private readonly DoubleCheckedLockingContainer<PropertyDefinitionCollection> _cachedPropertyDefinitions;

    
    // nonserialized member fields

    [NonSerialized]
    private PropertyDefinitionCollection _propertyDefinitions;

    [NonSerialized]
    private RelationDefinitionCollection _relationDefinitions;

    [NonSerialized]
    private IStorageEntityDefinition _storageEntityDefinition;

    // construction and disposing

    protected ClassDefinition (string id, Type storageGroupType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      
      _id = id;
      _storageGroupType = storageGroupType;

      _propertyAccessorDataCache = new PropertyAccessorDataCache (this);
      _cachedRelationDefinitions = new DoubleCheckedLockingContainer<RelationDefinitionCollection> (FindAllRelationDefinitions);
      _cachedRelationEndPointDefinitions = new DoubleCheckedLockingContainer<ReadOnlyDictionarySpecific<string, IRelationEndPointDefinition>> (
          FindAllRelationEndPointDefinitions);
      _cachedPropertyDefinitions =
          new DoubleCheckedLockingContainer<PropertyDefinitionCollection> (
              () => new PropertyDefinitionCollection (PropertyDefinitionCollection.CreateForAllProperties (this), true));
    }

    // methods and properties

    public PropertyAccessorDataCache PropertyAccessorDataCache
    {
      get { return _propertyAccessorDataCache; }
    }

    public bool IsReadOnly
    {
      get { return _isReadOnly; }
    }

    public void SetReadOnly ()
    {
      DerivedClasses.SetReadOnly();

      _isReadOnly = true;
    }

    public bool IsSameOrBaseClassOf (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (ReferenceEquals (this, classDefinition))
        return true;

      ClassDefinition baseClassOfProvidedClassDefinition = classDefinition.BaseClass;
      while (baseClassOfProvidedClassDefinition != null)
      {
        if (ReferenceEquals (this, baseClassOfProvidedClassDefinition))
          return true;

        baseClassOfProvidedClassDefinition = baseClassOfProvidedClassDefinition.BaseClass;
      }

      return false;
    }

    public string[] GetAllConcreteEntityNames ()
    {
      if (GetEntityName() != null)
        return new[] { GetEntityName() };

      var allConcreteEntityNames = new List<string>();
      FillAllConcreteEntityNames (allConcreteEntityNames);

      return allConcreteEntityNames.ToArray();
    }

    public ClassDefinitionCollection GetAllDerivedClasses ()
    {
      var allDerivedClasses = new ClassDefinitionCollection (IsClassTypeResolved);
      FillAllDerivedClasses (allDerivedClasses);
      return allDerivedClasses;
    }

    public ClassDefinition GetInheritanceRootClass ()
    {
      if (BaseClass != null)
        return BaseClass.GetInheritanceRootClass();

      return this;
    }

    public string GetEntityName ()
    {
      if (_storageEntityDefinition!=null && _storageEntityDefinition.LegacyEntityName != null)
        return _storageEntityDefinition.LegacyEntityName;

      if (BaseClass == null)
        return null;

      return BaseClass.GetEntityName();
    }

    public bool Contains (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      return MyPropertyDefinitions.Contains (propertyDefinition);
    }

    public IRelationEndPointDefinition GetOppositeEndPointDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      IRelationEndPointDefinition relationEndPointDefinition = GetRelationEndPointDefinition (propertyName);
      if (relationEndPointDefinition == null)
        return null;

      return relationEndPointDefinition.GetOppositeEndPointDefinition();
    }

    public IRelationEndPointDefinition GetMandatoryOppositeEndPointDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      IRelationEndPointDefinition relationEndPointDefinition = GetMandatoryRelationEndPointDefinition (propertyName);
      return relationEndPointDefinition.GetMandatoryOppositeEndPointDefinition();
    }

    public PropertyDefinitionCollection GetPropertyDefinitions ()
    {
      return _cachedPropertyDefinitions.Value;
    }

    public RelationDefinitionCollection GetRelationDefinitions ()
    {
      return _cachedRelationDefinitions.Value;
    }

    public ICollection<IRelationEndPointDefinition> GetRelationEndPointDefinitions ()
    {
      return ((IDictionary<string, IRelationEndPointDefinition>) _cachedRelationEndPointDefinitions.Value).Values;
    }

    public IRelationEndPointDefinition[] GetMyRelationEndPointDefinitions ()
    {
      var relationEndPointDefinitions = new ArrayList();

      foreach (RelationDefinition relationDefinition in MyRelationDefinitions)
      {
        foreach (IRelationEndPointDefinition endPointDefinition in relationDefinition.EndPointDefinitions)
        {
          if (IsMyRelationEndPoint (endPointDefinition))
            relationEndPointDefinitions.Add (endPointDefinition);
        }
      }

      return (IRelationEndPointDefinition[]) relationEndPointDefinitions.ToArray (typeof (IRelationEndPointDefinition));
    }

    // TODO Review 3518: Add a test checking that this method throws an exception if no relations have been set
    public RelationDefinition GetRelationDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      foreach (RelationDefinition relationDefinition in MyRelationDefinitions)
      {
        if (relationDefinition.IsEndPoint (_id, propertyName))
          return relationDefinition;
      }

      if (BaseClass != null)
        return BaseClass.GetRelationDefinition (propertyName);

      return null;
    }

    public RelationDefinition GetMandatoryRelationDefinition (string propertyName)
    {
      RelationDefinition relationDefinition = GetRelationDefinition (propertyName);
      if (relationDefinition == null)
        throw CreateMappingException ("No relation found for class '{0}' and property '{1}'.", ID, propertyName);

      return relationDefinition;
    }

    public ClassDefinition GetOppositeClassDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      var endPointDefinition = GetRelationEndPointDefinition (propertyName);
      if (endPointDefinition == null)
        return null;

      return endPointDefinition.GetOppositeClassDefinition();
    }

    public ClassDefinition GetMandatoryOppositeClassDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      ClassDefinition oppositeClassDefinition = GetOppositeClassDefinition (propertyName);

      if (oppositeClassDefinition == null)
        throw CreateMappingException ("No relation found for class '{0}' and property '{1}'.", ID, propertyName);

      return oppositeClassDefinition;
    }

    public IRelationEndPointDefinition GetRelationEndPointDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      IRelationEndPointDefinition value;
      _cachedRelationEndPointDefinitions.Value.TryGetValue (propertyName, out value);
      return value;
    }

    public IRelationEndPointDefinition GetMandatoryRelationEndPointDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      IRelationEndPointDefinition relationEndPointDefinition = GetRelationEndPointDefinition (propertyName);

      if (relationEndPointDefinition == null)
        throw CreateMappingException ("No relation found for class '{0}' and property '{1}'.", ID, propertyName);

      return relationEndPointDefinition;
    }

    public bool IsMyRelationEndPoint (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      return (relationEndPointDefinition.ClassDefinition == this && !relationEndPointDefinition.IsAnonymous);
    }

    public bool IsRelationEndPoint (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      if (IsMyRelationEndPoint (relationEndPointDefinition))
        return true;

      if (BaseClass != null)
        return BaseClass.IsRelationEndPoint (relationEndPointDefinition);

      return false;
    }

    // TODO Review 3518: Add a test checking that this method throws an exception if no properties have been set
    public PropertyDefinition GetPropertyDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      var propertyDefinition = MyPropertyDefinitions[propertyName];

      if (propertyDefinition == null && BaseClass != null)
        return BaseClass.GetPropertyDefinition (propertyName);

      return propertyDefinition;
    }

    public void SetStorageEntity (IStorageEntityDefinition storageEntityDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageEntityDefinition", storageEntityDefinition);

      if (_isReadOnly)
        throw new NotSupportedException (string.Format ("Class '{0}' is read-only.", ID));

      _storageEntityDefinition = storageEntityDefinition;
    }

    public void SetPropertyDefinitions (PropertyDefinitionCollection propertyDefinitions)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinitions", propertyDefinitions);

      if (_isReadOnly)
        throw new NotSupportedException (string.Format ("Class '{0}' is read-only.", ID));

      CheckNewPropertyDefinitions (propertyDefinitions);

      // TODO Review 3518: Set collection read-only
      _propertyDefinitions = propertyDefinitions;
    }

    public void SetRelationDefinitions (RelationDefinitionCollection relationDefinitions)
    {
      ArgumentUtility.CheckNotNull ("relationDefinitions", relationDefinitions);

      if (_isReadOnly)
        throw new NotSupportedException (string.Format ("Class '{0}' is read-only.", ID));

      // TODO Review 3518: Add a CheckNewRelationDefinitions methods that checks for duplicates and ClassDefinition (similar to CheckNewPropertyDefinitions), add tests

      // TODO Review 3518: Set collection read-only
      _relationDefinitions = relationDefinitions;
    }

    public PropertyDefinition GetMandatoryPropertyDefinition (string propertyName)
    {
      PropertyDefinition propertyDefinition = GetPropertyDefinition (propertyName);

      if (propertyDefinition == null)
        throw CreateMappingException ("Class '{0}' does not contain the property '{1}'.", ID, propertyName);

      return propertyDefinition;
    }

    public PropertyDefinition this [string propertyName]
    {
      get
      {
        ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
        return MyPropertyDefinitions[propertyName];
      }
    }

    public string ID
    {
      get { return _id; }
    }

    public IStorageEntityDefinition StorageEntityDefinition
    {
      get { return _storageEntityDefinition; }
    }

    public abstract Type ClassType { get; }

    public abstract bool IsClassTypeResolved { get; }

    public abstract ClassDefinition BaseClass { get; }
    public abstract ClassDefinitionCollection DerivedClasses { get; }
    public abstract IDomainObjectCreator GetDomainObjectCreator ();
    public abstract PropertyDefinition ResolveProperty (IPropertyInformation propertyInformation);
    public abstract IRelationEndPointDefinition ResolveRelationEndPoint (IPropertyInformation propertyInformation);

    public Type StorageGroupType
    {
      get { return _storageGroupType; }
    }

    public PropertyDefinitionCollection MyPropertyDefinitions
    {
      get
      {
        if(_propertyDefinitions == null)
          throw new InvalidOperationException ("No property definitions have been set.");
        
        return _propertyDefinitions; 
      }
    }

    public RelationDefinitionCollection MyRelationDefinitions
    {
      get 
      {
        if (_relationDefinitions == null)
          throw new InvalidOperationException ("No relation definitions have been set.");
        
        return _relationDefinitions; 
      }
    }

    public bool IsPartOfInheritanceHierarchy
    {
      get { return (BaseClass != null || DerivedClasses.Count > 0); }
    }

    public abstract bool IsAbstract { get; }

    public override string ToString ()
    {
      return GetType().FullName + ": " + _id;
    }

    public abstract ReflectionBasedClassDefinitionValidator GetValidator ();

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (String.Format (message, args));
    }

    private void FillAllConcreteEntityNames (List<string> allConcreteEntityNames)
    {
      if (_storageEntityDefinition!=null && _storageEntityDefinition.LegacyEntityName != null)
      {
        allConcreteEntityNames.Add (_storageEntityDefinition.LegacyEntityName);
        return;
      }

      foreach (ClassDefinition derivedClass in DerivedClasses)
        derivedClass.FillAllConcreteEntityNames (allConcreteEntityNames);
    }

    private void FillAllDerivedClasses (ClassDefinitionCollection allDerivedClasses)
    {
      foreach (ClassDefinition derivedClass in DerivedClasses)
      {
        allDerivedClasses.Add (derivedClass);
        derivedClass.FillAllDerivedClasses (allDerivedClasses);
      }
    }

    private RelationDefinitionCollection FindAllRelationDefinitions ()
    {
      var relations = new RelationDefinitionCollection (MyRelationDefinitions.Cast<RelationDefinition>(), false);

      if (BaseClass != null)
      {
        foreach (RelationDefinition baseRelation in BaseClass.GetRelationDefinitions())
        {
          if (!relations.Contains (baseRelation))
            relations.Add (baseRelation);
        }
      }

      return new RelationDefinitionCollection (relations.Cast<RelationDefinition>(), true);
    }

    private ReadOnlyDictionarySpecific<string, IRelationEndPointDefinition> FindAllRelationEndPointDefinitions ()
    {
      IEnumerable<IRelationEndPointDefinition> relationEndPointDefinitions = GetMyRelationEndPointDefinitions();

      if (BaseClass != null)
        relationEndPointDefinitions = relationEndPointDefinitions.Concat (BaseClass.GetRelationEndPointDefinitions());

      return new ReadOnlyDictionarySpecific<string, IRelationEndPointDefinition> (relationEndPointDefinitions.ToDictionary (def => def.PropertyName));
    }

    private void CheckNewPropertyDefinitions (PropertyDefinitionCollection propertyDefinitions)
    {
      foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
      {
        if (!ReferenceEquals (propertyDefinition.ClassDefinition, this))
        {
          throw CreateMappingException (
              "Property '{0}' cannot be added to class '{1}', because it was initialized for class '{2}'.",
              propertyDefinition.PropertyName,
              _id,
              propertyDefinition.ClassDefinition.ID);
        }

        var basePropertyDefinition = BaseClass != null ? BaseClass.GetPropertyDefinition (propertyDefinition.PropertyName) : null;
        if (basePropertyDefinition != null)
        {
          string definingClass =
              basePropertyDefinition.ClassDefinition == this
                  ? "it"
                  : String.Format ("base class '{0}'", basePropertyDefinition.ClassDefinition.ID);

          throw CreateMappingException (
              "Property '{0}' cannot be added to class '{1}', because {2} already defines a property with the same name.",
              propertyDefinition.PropertyName,
              _id,
              definingClass);
        }
      }
    }
    
    #region Serialization

    public override object GetRealObject (StreamingContext context)
    {
      return MappingConfiguration.Current.ClassDefinitions.GetMandatory (_id);
    }

    protected override bool IsPartOfMapping
    {
      get { return MappingConfiguration.Current.Contains (this); }
    }

    protected override string IDForExceptions
    {
      get { return ID; }
    }

    #endregion
  }
}