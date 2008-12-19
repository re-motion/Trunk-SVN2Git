// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

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
    private ClassDefinition _baseClass;
    [NonSerialized]
    private ClassDefinitionCollection _derivedClasses;

    // construction and disposing

    protected ClassDefinition (string id, string entityName, string storageProviderID, bool areResolvedTypesRequired)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      if (entityName == string.Empty)
        throw new ArgumentEmptyException ("entityName");
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);

      _id = id;
      _entityName = entityName;
      _storageProviderID = storageProviderID;

      _derivedClasses = new ClassDefinitionCollection (new ClassDefinitionCollection (areResolvedTypesRequired), true);
      _propertyDefinitions = new PropertyDefinitionCollection (this);
      _relationDefinitions = new RelationDefinitionCollection();
    }

    // methods and properties

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
        return new string[] {GetEntityName()};

      List<string> allConcreteEntityNames = new List<string>();
      FillAllConcreteEntityNames (allConcreteEntityNames);

      return allConcreteEntityNames.ToArray();
    }

    public ClassDefinitionCollection GetAllDerivedClasses()
    {
      ClassDefinitionCollection allDerivedClasses = new ClassDefinitionCollection (IsClassTypeResolved);
      FillAllDerivedClasses (allDerivedClasses);
      return allDerivedClasses;
    }

    public ClassDefinition GetInheritanceRootClass()
    {
      if (_baseClass != null)
        return _baseClass.GetInheritanceRootClass();

      return this;
    }

    public string GetEntityName()
    {
      if (_entityName != null)
        return _entityName;

      if (_baseClass == null)
        return null;

      return _baseClass.GetEntityName();
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
      PropertyDefinitionCollection propertyDefinitions = new PropertyDefinitionCollection (
          _propertyDefinitions, false);

      if (_baseClass != null)
      {
        foreach (PropertyDefinition basePropertyDefinition in _baseClass.GetPropertyDefinitions())
          propertyDefinitions.Add (basePropertyDefinition);
      }

      return propertyDefinitions;
    }

    public RelationDefinitionCollection GetRelationDefinitions()
    {
      RelationDefinitionCollection relations = new RelationDefinitionCollection (_relationDefinitions, false);

      if (_baseClass != null)
      {
        foreach (RelationDefinition baseRelation in _baseClass.GetRelationDefinitions())
        {
          if (!relations.Contains (baseRelation))
            relations.Add (baseRelation);
        }
      }

      return relations;
    }

    public IRelationEndPointDefinition[] GetRelationEndPointDefinitions()
    {
      ArrayList relationEndPointDefinitions = new ArrayList();

      foreach (IRelationEndPointDefinition relationEndPointDefinition in GetMyRelationEndPointDefinitions())
        relationEndPointDefinitions.Add (relationEndPointDefinition);

      if (_baseClass != null)
      {
        foreach (IRelationEndPointDefinition baseRelationEndPointDefinition in _baseClass.GetRelationEndPointDefinitions())
          relationEndPointDefinitions.Add (baseRelationEndPointDefinition);
      }

      return (IRelationEndPointDefinition[]) relationEndPointDefinitions.ToArray (typeof (IRelationEndPointDefinition));
    }

    public IRelationEndPointDefinition[] GetMyRelationEndPointDefinitions()
    {
      ArrayList relationEndPointDefinitions = new ArrayList();

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

      if (_baseClass != null)
        return _baseClass.GetRelationDefinition (propertyName);

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

      if (_baseClass != null)
        return _baseClass.GetOppositeClassDefinition (propertyName);

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

      if (_baseClass != null)
        return _baseClass.GetRelationEndPointDefinition (propertyName);

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

      return (relationEndPointDefinition.ClassDefinition == this && !relationEndPointDefinition.IsNull);
    }

    public bool IsRelationEndPoint (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      if (IsMyRelationEndPoint (relationEndPointDefinition))
        return true;

      if (_baseClass != null)
        return _baseClass.IsRelationEndPoint (relationEndPointDefinition);

      return false;
    }

    public PropertyDefinition GetPropertyDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      PropertyDefinition propertyDefinition = _propertyDefinitions[propertyName];

      if (propertyDefinition == null && _baseClass != null)
        return _baseClass.GetPropertyDefinition (propertyName);

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

    public string StorageProviderID
    {
      get { return _storageProviderID; }
    }

    public ClassDefinition BaseClass
    {
      get { return _baseClass; }
    }

    public ClassDefinitionCollection DerivedClasses
    {
      get { return _derivedClasses; }
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
      get { return (_baseClass != null || _derivedClasses.Count > 0); }
    }

    public abstract bool IsAbstract { get; }

    public override string ToString ()
    {
      return GetType().FullName + ": " + _id;
    }

    [Obsolete ("Check after Refactoring. (Version 1.7.42")]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public void SetBaseClass (ClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);

      CheckBaseClass (baseClass, _id, _storageProviderID, ClassType);
      PerformSetBaseClass (baseClass);
    }

    public virtual ClassDefinitionValidator GetValidator ()
    {
      return new ClassDefinitionValidator (this);
    }

    internal static void SetClassDefinition (ClassDefinition classDefinition, PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

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

      PropertyDefinitionCollection allPropertyDefinitions = GetPropertyDefinitions();
      if (allPropertyDefinitions.Contains (args.PropertyDefinition.PropertyName))
      {
        PropertyDefinition basePropertyDefinition = allPropertyDefinitions[args.PropertyDefinition.PropertyName];
        string definingClass;
        if (basePropertyDefinition.ClassDefinition == this)
          definingClass = "it";
        else
          definingClass = string.Format ("base class '{0}'", basePropertyDefinition.ClassDefinition.ID);

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

    private void PerformSetBaseClass (ClassDefinition baseClass)
    {
      _baseClass = baseClass;
      _baseClass.AddDerivedClass (this);
    }

    private void AddDerivedClass (ClassDefinition derivedClass)
    {
      ClassDefinitionCollection derivedClasses = new ClassDefinitionCollection (_derivedClasses, false);
      derivedClasses.Add (derivedClass);
      _derivedClasses = new ClassDefinitionCollection (derivedClasses, true);
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }

    private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
    {
      return new InvalidOperationException (string.Format (message, args));
    }

    private void CheckBaseClass (ClassDefinition baseClass, string id, string storageProviderID, Type classType)
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

    private void FillAllConcreteEntityNames (List<string> allConcreteEntityNames)
    {
      if (_entityName != null)
      {
        allConcreteEntityNames.Add (_entityName);
        return;
      }

      foreach (ClassDefinition derivedClass in _derivedClasses)
        derivedClass.FillAllConcreteEntityNames (allConcreteEntityNames);
    }

    private void FillAllDerivedClasses (ClassDefinitionCollection allDerivedClasses)
    {
      foreach (ClassDefinition derivedClass in _derivedClasses)
      {
        allDerivedClasses.Add (derivedClass);
        derivedClass.FillAllDerivedClasses (allDerivedClasses);
      }
    }

    protected internal abstract IDomainObjectCreator GetDomainObjectCreator ();

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
