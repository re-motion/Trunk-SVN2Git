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
using System.Diagnostics;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Infrastructure;
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

    [NonSerialized]
    private bool _isReadOnly;

    [NonSerialized]
    private readonly Type _storageGroupType;

    [NonSerialized]
    private readonly PropertyAccessorDataCache _propertyAccessorDataCache;

    [NonSerialized]
    private readonly DoubleCheckedLockingContainer<RelationEndPointDefinitionCollection> _cachedRelationEndPointDefinitions;

    [NonSerialized]
    private readonly DoubleCheckedLockingContainer<PropertyDefinitionCollection> _cachedPropertyDefinitions;

    [NonSerialized]
    private readonly ClassDefinition _baseClass;

    [NonSerialized]
    private PropertyDefinitionCollection _propertyDefinitions;

    [NonSerialized]
    private RelationEndPointDefinitionCollection _relationEndPoints;

    [NonSerialized]
    private IStorageEntityDefinition _storageEntityDefinition;

    [NonSerialized]
    private ClassDefinitionCollection _derivedClasses;

    protected ClassDefinition (string id, Type storageGroupType, ClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      _id = id;
      _storageGroupType = storageGroupType;

      _propertyAccessorDataCache = new PropertyAccessorDataCache (this);
       _cachedRelationEndPointDefinitions = new DoubleCheckedLockingContainer<RelationEndPointDefinitionCollection> (
            () => RelationEndPointDefinitionCollection.CreateForAllRelationEndPoints (this, true));
      _cachedPropertyDefinitions =
          new DoubleCheckedLockingContainer<PropertyDefinitionCollection> (
              () => new PropertyDefinitionCollection (PropertyDefinitionCollection.CreateForAllProperties (this, true), true));

      _baseClass = baseClass;
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
      if (_storageEntityDefinition != null && _storageEntityDefinition.LegacyEntityName != null)
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

    public RelationEndPointDefinitionCollection GetRelationEndPointDefinitions ()
    {
      return _cachedRelationEndPointDefinitions.Value;
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

      return _cachedRelationEndPointDefinitions.Value[propertyName];
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

      if (_propertyDefinitions != null)
        throw new InvalidOperationException (string.Format ("The property-definitions for class '{0}' have already been set.", ID));

      if (_isReadOnly)
        throw new NotSupportedException (string.Format ("Class '{0}' is read-only.", ID));

      CheckPropertyDefinitions (propertyDefinitions);

      _propertyDefinitions = propertyDefinitions;
      _propertyDefinitions.SetReadOnly();
    }

    public void SetRelationEndPointDefinitions (RelationEndPointDefinitionCollection relationEndPoints)
    {
      ArgumentUtility.CheckNotNull ("relationEndPoints", relationEndPoints);

      if (_relationEndPoints != null)
        throw new InvalidOperationException (string.Format ("The relation end point definitions for class '{0}' have already been set.", ID));

      if (_isReadOnly)
        throw new NotSupportedException (string.Format ("Class '{0}' is read-only.", ID));

      CheckRelationEndPointDefinitions (relationEndPoints);

      _relationEndPoints = relationEndPoints;
      _relationEndPoints.SetReadOnly();
    }

    public void SetDerivedClasses (ClassDefinitionCollection derivedClasses)
    {
      ArgumentUtility.CheckNotNull ("derivedClasses", derivedClasses);

      if (_derivedClasses != null)
        throw new InvalidOperationException (string.Format ("The derived-classes for class '{0}' have already been set.", ID));

      if (_isReadOnly)
        throw new NotSupportedException (string.Format ("Class '{0}' is read-only.", ID));

      CheckDerivedClass (derivedClasses);

      _derivedClasses = derivedClasses;
      _derivedClasses.SetReadOnly();
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

    public abstract IDomainObjectCreator GetDomainObjectCreator ();
    public abstract PropertyDefinition ResolveProperty (IPropertyInformation propertyInformation);
    public abstract IRelationEndPointDefinition ResolveRelationEndPoint (IPropertyInformation propertyInformation);

    public ClassDefinition BaseClass
    {
      get { return _baseClass; }
    }

    public Type StorageGroupType
    {
      get { return _storageGroupType; }
    }

    public PropertyDefinitionCollection MyPropertyDefinitions
    {
      get
      {
        if (_propertyDefinitions == null)
          throw new InvalidOperationException (string.Format ("No property definitions have been set for class '{0}'.", ID));

        return _propertyDefinitions;
      }
    }

    public RelationEndPointDefinitionCollection MyRelationEndPointDefinitions
    {
      get 
      {
        if (_relationEndPoints == null)
          throw new InvalidOperationException (string.Format ("No relation end point definitions have been set for class '{0}'.", ID));

        return _relationEndPoints;
      }
    }

    public ClassDefinitionCollection DerivedClasses
    {
      get
      {
        if (_derivedClasses == null)
          throw new InvalidOperationException (string.Format ("No derived classes have been set for class '{0}'.", ID));

        return _derivedClasses;
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
      if (_storageEntityDefinition != null && _storageEntityDefinition.LegacyEntityName != null)
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

    private void CheckPropertyDefinitions (IEnumerable<PropertyDefinition> propertyDefinitions)
    {
      foreach (var propertyDefinition in propertyDefinitions)
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
          string definingClass = String.Format ("base class '{0}'", basePropertyDefinition.ClassDefinition.ID);

          throw CreateMappingException (
              "Property '{0}' cannot be added to class '{1}', because {2} already defines a property with the same name.",
              propertyDefinition.PropertyName,
              _id,
              definingClass);
        }
      }
    }

    private void CheckRelationEndPointDefinitions (IEnumerable<IRelationEndPointDefinition> relationEndPoints)
    {
      foreach (IRelationEndPointDefinition endPointDefinition in relationEndPoints)
      {
        if (!ReferenceEquals (endPointDefinition.ClassDefinition, this))
        {
          throw CreateMappingException (
              "Relation end point for property '{0}' cannot be added to class '{1}', because it was initialized for class '{2}'.",
              endPointDefinition.PropertyName,
              _id,
              endPointDefinition.ClassDefinition.ID);
        }

        var baseEndPointDefinition = BaseClass != null ? BaseClass.GetRelationEndPointDefinition (endPointDefinition.PropertyName) : null;
        if (baseEndPointDefinition != null)
        {
          string definingClass = String.Format ("base class '{0}'", baseEndPointDefinition.ClassDefinition.ID);

          throw CreateMappingException (
              "Relation end point for property '{0}' cannot be added to class '{1}', because {2} already defines a relation end point with the same property name.",
              endPointDefinition.PropertyName,
              _id,
              definingClass);
        }
      }
    }

    private void CheckDerivedClass (ClassDefinitionCollection derivedClasses)
    {
      foreach (ClassDefinition derivedClass in derivedClasses)
      {
        if (derivedClass.BaseClass == null)
        {
          throw CreateMappingException (
              "Derived class '{0}' cannot be added to class '{1}', because it has no base class definition defined.", derivedClass.ID, _id);
        }

        if (derivedClass.BaseClass != this)
        {
          throw CreateMappingException (
              "Derived class '{0}' cannot be added to class '{1}', because it has class '{2}' as its base class definition defined.",
              derivedClass.ID,
              _id,
              derivedClass.BaseClass.ID);
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