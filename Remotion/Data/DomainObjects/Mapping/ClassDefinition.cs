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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;
using System.Linq;
using System.Reflection;

namespace Remotion.Data.DomainObjects.Mapping
{
  [Serializable]
  [DebuggerDisplay ("{GetType().Name} for {ClassType.FullName}")]
  public abstract class ClassDefinition: SerializableMappingObject
  {
    // types

    // static members and constants

    // serialized member fields
    // Note: ClassDefinitions can only be serialized if they are part of the current mapping configuration. Only the fields listed below
    // will be serialized; these are used to retrieve the "real" object at deserialization time.

    private readonly string _id;
    
    private bool _isReadOnly;

    // nonserialized member fields
    [NonSerialized]
    private readonly string _entityName;
    [NonSerialized]
    private readonly string _storageProviderID;
    [NonSerialized]
    private readonly PropertyDefinitionCollection _propertyDefinitions;
    [NonSerialized]
    private readonly RelationDefinitionCollection _relationDefinitions;

    [NonSerialized]
    private readonly DoubleCheckedLockingContainer<Dictionary<string, PropertyAccessorData>>  _cachedAccessorData;
    [NonSerialized]
    private readonly DoubleCheckedLockingContainer<RelationDefinitionCollection> _cachedRelationDefinitions;
    [NonSerialized]
    private readonly DoubleCheckedLockingContainer<ReadOnlyCollection<IRelationEndPointDefinition>> _cachedRelationEndPointDefinitions;
    [NonSerialized]
    private readonly DoubleCheckedLockingContainer<PropertyDefinitionCollection> _cachedPropertyDefinitions;

    // construction and disposing

    protected ClassDefinition (string id, string entityName, string storageProviderID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      if (entityName == string.Empty)
        throw new ArgumentEmptyException ("entityName");
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);

      _id = id;
      _entityName = entityName;
      _storageProviderID = storageProviderID;

      _propertyDefinitions = new PropertyDefinitionCollection (this);
      _relationDefinitions = new RelationDefinitionCollection();

      _cachedAccessorData = new DoubleCheckedLockingContainer<Dictionary<string, PropertyAccessorData>> (BuildAccessorDataDictionary);
      _cachedRelationDefinitions = new DoubleCheckedLockingContainer<RelationDefinitionCollection> (FindAllRelationDefinitions);
      _cachedRelationEndPointDefinitions = new DoubleCheckedLockingContainer<ReadOnlyCollection<IRelationEndPointDefinition>> (FindAllRelationEndPointDefinitions);
      _cachedPropertyDefinitions = new DoubleCheckedLockingContainer<PropertyDefinitionCollection> (FindAllPropertyDefinitions);
    }

    // methods and properties

    public bool IsReadOnly
    {
      get { return _isReadOnly; }
    }

    public void SetReadOnly ()
    {
      _propertyDefinitions.SetReadOnly ();
      _relationDefinitions.SetReadOnly ();

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

    public string[] GetAllConcreteEntityNames()
    {
      if (GetEntityName() != null)
        return new[] {GetEntityName()};

      var allConcreteEntityNames = new List<string>();
      FillAllConcreteEntityNames (allConcreteEntityNames);

      return allConcreteEntityNames.ToArray();
    }

    public ClassDefinitionCollection GetAllDerivedClasses()
    {
      var allDerivedClasses = new ClassDefinitionCollection (IsClassTypeResolved);
      FillAllDerivedClasses (allDerivedClasses);
      return allDerivedClasses;
    }

    public ClassDefinition GetInheritanceRootClass()
    {
      if (BaseClass != null)
        return BaseClass.GetInheritanceRootClass();

      return this;
    }

    public string GetEntityName()
    {
      if (_entityName != null)
        return _entityName;

      if (BaseClass == null)
        return null;

      return BaseClass.GetEntityName();
    }

    public string GetViewName ()
    {
      return ID + "View";
    }

    public bool Contains (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      return _propertyDefinitions.Contains (propertyDefinition);
    }

    public IRelationEndPointDefinition GetOppositeEndPointDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      IRelationEndPointDefinition relationEndPointDefinition = GetRelationEndPointDefinition (propertyName);
      RelationDefinition relationDefinition = GetRelationDefinition (propertyName);

      if (relationDefinition != null && relationEndPointDefinition != null)
        return relationDefinition.GetOppositeEndPointDefinition (relationEndPointDefinition);

      return null;
    }

    public IRelationEndPointDefinition GetMandatoryOppositeEndPointDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      IRelationEndPointDefinition relationEndPointDefinition = GetMandatoryRelationEndPointDefinition (propertyName);
      RelationDefinition relationDefinition = GetRelationDefinition (propertyName);
      return relationDefinition.GetMandatoryOppositeRelationEndPointDefinition (relationEndPointDefinition);
    }

    public PropertyDefinitionCollection GetPropertyDefinitions()
    {
      CheckIsReadOnlyForCachedData ();

      return _cachedPropertyDefinitions.Value;
    }

    public RelationDefinitionCollection GetRelationDefinitions()
    {
      CheckIsReadOnlyForCachedData();

      return _cachedRelationDefinitions.Value;
    }

    public ReadOnlyCollection<IRelationEndPointDefinition> GetRelationEndPointDefinitions ()
    {
      CheckIsReadOnlyForCachedData ();

      return _cachedRelationEndPointDefinitions.Value;
    }

    public IRelationEndPointDefinition[] GetMyRelationEndPointDefinitions()
    {
      var relationEndPointDefinitions = new ArrayList();

      foreach (RelationDefinition relationDefinition in _relationDefinitions)
      {
        foreach (IRelationEndPointDefinition endPointDefinition in relationDefinition.EndPointDefinitions)
        {
          if (IsMyRelationEndPoint (endPointDefinition))
            relationEndPointDefinitions.Add (endPointDefinition);
        }
      }

      return (IRelationEndPointDefinition[]) relationEndPointDefinitions.ToArray (typeof (IRelationEndPointDefinition));
    }

    public RelationDefinition GetRelationDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      foreach (RelationDefinition relationDefinition in _relationDefinitions)
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

      RelationDefinition relationDefinition = GetRelationDefinition (propertyName);
      if (relationDefinition == null)
        return null;

      ClassDefinition oppositeClass = relationDefinition.GetOppositeClassDefinition (_id, propertyName);

      if (oppositeClass != null)
        return oppositeClass;

      if (BaseClass != null)
        return BaseClass.GetOppositeClassDefinition (propertyName);

      return null;
    }

    public ClassDefinition GetMandatoryOppositeClassDefinition (string propertyName)
    {
      ClassDefinition oppositeClassDefinition = GetOppositeClassDefinition (propertyName);

      if (oppositeClassDefinition == null)
        throw CreateMappingException ("No relation found for class '{0}' and property '{1}'.", ID, propertyName);

      return oppositeClassDefinition;
    }

    public IRelationEndPointDefinition GetRelationEndPointDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      foreach (RelationDefinition relationDefinition in _relationDefinitions)
      {
        IRelationEndPointDefinition relationEndPointDefinition = relationDefinition.GetEndPointDefinition (_id, propertyName);
        if (relationEndPointDefinition != null)
          return relationEndPointDefinition;
      }

      if (BaseClass != null)
        return BaseClass.GetRelationEndPointDefinition (propertyName);

      return null;
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

      PropertyDefinition propertyDefinition = _propertyDefinitions[propertyName];

      if (propertyDefinition == null && BaseClass != null)
        return BaseClass.GetPropertyDefinition (propertyName);

      return propertyDefinition;
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
        return _propertyDefinitions[propertyName];
      }
    }

    public string ID
    {
      get { return _id; }
    }

    public string MyEntityName
    {
      get { return _entityName; }
    }

    public abstract Type ClassType { get; }

    public abstract bool IsClassTypeResolved { get; }
    
    public abstract ClassDefinition BaseClass { get; }
    public abstract ClassDefinitionCollection DerivedClasses { get; }
    public abstract IDomainObjectCreator GetDomainObjectCreator ();
    public abstract PropertyDefinition ResolveProperty (PropertyInfo property);

    public string StorageProviderID
    {
      get { return _storageProviderID; }
    }

    public PropertyDefinitionCollection MyPropertyDefinitions
    {
      get { return _propertyDefinitions; }
    }

    public RelationDefinitionCollection MyRelationDefinitions
    {
      get { return _relationDefinitions; }
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

    public virtual ClassDefinitionValidator GetValidator ()
    {
      return new ClassDefinitionValidator (this);
    }

    public PropertyAccessorData GetPropertyAccessorData (string propertyIdentifier)
    {
      PropertyAccessorData result;
      _cachedAccessorData.Value.TryGetValue (propertyIdentifier, out result);
      return result;
    }

    internal void PropertyDefinitions_Adding (object sender, PropertyDefinitionAddingEventArgs args)
    {
      if (!ReferenceEquals (args.PropertyDefinition.ClassDefinition, this))
      {
        throw CreateMappingException (
            "Property '{0}' cannot be added to class '{1}', because it was initialized for class '{2}'.",
            args.PropertyDefinition.PropertyName,
            _id,
            args.PropertyDefinition.ClassDefinition.ID);
      }

      if (IsClassTypeResolved != args.PropertyDefinition.IsPropertyTypeResolved)
      {
        if (IsClassTypeResolved)
        {
          throw CreateInvalidOperationException (
              "The PropertyDefinition '{0}' cannot be added to ClassDefinition '{1}', "
              + "because the ClassDefinition's type is resolved and the PropertyDefinition's type is not.",
              args.PropertyDefinition.PropertyName,
              _id);
        }
        else
        {
          throw CreateInvalidOperationException (
              "The PropertyDefinition '{0}' cannot be added to ClassDefinition '{1}', "
              + "because the PropertyDefinition's type is resolved and the ClassDefinition's type is not.",
              args.PropertyDefinition.PropertyName,
              _id);
        }
      }

      var basePropertyDefinition = GetPropertyDefinition (args.PropertyDefinition.PropertyName);
      if (basePropertyDefinition != null)
      {
        string definingClass = 
            basePropertyDefinition.ClassDefinition == this 
            ? "it" 
            : string.Format ("base class '{0}'", basePropertyDefinition.ClassDefinition.ID);

        throw CreateMappingException (
            "Property '{0}' cannot be added to class '{1}', because {2} already defines a property with the same name.",
            args.PropertyDefinition.PropertyName,
            _id,
            definingClass);
      }
    }

    internal void PropertyDefinitions_Added (object sender, PropertyDefinitionAddedEventArgs args)
    {
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }

    private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
    {
      return new InvalidOperationException (string.Format (message, args));
    }

    private void FillAllConcreteEntityNames (List<string> allConcreteEntityNames)
    {
      if (_entityName != null)
      {
        allConcreteEntityNames.Add (_entityName);
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
      var relations = new RelationDefinitionCollection (_relationDefinitions, false);

      if (BaseClass != null)
      {
        foreach (RelationDefinition baseRelation in BaseClass.GetRelationDefinitions ())
        {
          if (!relations.Contains (baseRelation))
            relations.Add (baseRelation);
        }
      }

      return new RelationDefinitionCollection (relations, true);
    }

    private PropertyDefinitionCollection FindAllPropertyDefinitions ()
    {
      var propertyDefinitions = new PropertyDefinitionCollection (_propertyDefinitions, false);

      if (BaseClass != null)
      {
        foreach (PropertyDefinition basePropertyDefinition in BaseClass.GetPropertyDefinitions ())
          propertyDefinitions.Add (basePropertyDefinition);
      }

      return new PropertyDefinitionCollection (propertyDefinitions, true);
    }


    private ReadOnlyCollection<IRelationEndPointDefinition> FindAllRelationEndPointDefinitions ()
    {
      var relationEndPointDefinitions = new ArrayList ();

      foreach (IRelationEndPointDefinition relationEndPointDefinition in GetMyRelationEndPointDefinitions ())
        relationEndPointDefinitions.Add (relationEndPointDefinition);

      if (BaseClass != null)
      {
        foreach (IRelationEndPointDefinition baseRelationEndPointDefinition in BaseClass.GetRelationEndPointDefinitions ())
          relationEndPointDefinitions.Add (baseRelationEndPointDefinition);
      }

      return Array.AsReadOnly ((IRelationEndPointDefinition[]) relationEndPointDefinitions.ToArray (typeof (IRelationEndPointDefinition)));
    }

    private void CheckIsReadOnlyForCachedData ()
    {
      if (!IsReadOnly)
        throw new InvalidOperationException ("ClassDefinition must be read-only when retrieving data that spans the inheritance hierarchy.");
    }
    
    private Dictionary<string, PropertyAccessorData> BuildAccessorDataDictionary ()
    {
      var propertyDefinitions = GetPropertyDefinitions ();
      var relationEndPointDefinitions = GetRelationEndPointDefinitions ();

      var propertyDefinitionNames = from PropertyDefinition pd in propertyDefinitions
                                    select pd.PropertyName;
      var virtualRelationEndPointNames =
          from IRelationEndPointDefinition repd in relationEndPointDefinitions
          where repd.IsVirtual
          select repd.PropertyName;

      var allPropertyNames = propertyDefinitionNames.Concat (virtualRelationEndPointNames);
      var allPropertyAccessorData = allPropertyNames.Select (name => new PropertyAccessorData (this, name));
      return allPropertyAccessorData.ToDictionary (data => data.PropertyIdentifier);
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
