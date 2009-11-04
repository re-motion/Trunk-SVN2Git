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
using Remotion.Collections;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
[Serializable]
public class ClassDefinitionCollection : CommonCollection
{
  // types

  // static members and constants

  private static readonly ILog s_log = LogManager.GetLogger (typeof (ClassDefinitionCollection));

  // member fields

  private Hashtable _types = new Hashtable ();
  private bool _areResolvedTypesRequired;

  // construction and disposing

  public ClassDefinitionCollection () : this (true)
  {
  }

  public ClassDefinitionCollection (bool areResolvedTypesRequired)
  {
    _areResolvedTypesRequired = areResolvedTypesRequired;
  }

  // standard constructor for collections

  public ClassDefinitionCollection (ClassDefinitionCollection collection, bool makeCollectionReadOnly)  
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (ClassDefinition classDefinition in collection)
      Add (classDefinition);

    _areResolvedTypesRequired = collection.AreResolvedTypesRequired;
    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  // methods and properties

  public void Validate ()
  {
    s_log.InfoFormat ("Validating {0} class definitions...", Count);

    using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to validate mapping configuration: {elapsed}."))
    {
      foreach (ClassDefinition rootClass in GetInheritanceRootClasses())
      {
        ValidateRootClass (rootClass);
      }
    }
  }

  public ClassDefinitionCollection GetInheritanceRootClasses ()
  {
    ClassDefinitionCollection rootClasses = new ClassDefinitionCollection (this.AreResolvedTypesRequired);
    foreach (ClassDefinition classDefinition in this)
    {
      ClassDefinition rootClassDefinition = classDefinition.GetInheritanceRootClass ();
      if (!rootClasses.Contains (rootClassDefinition))
        rootClasses.Add (rootClassDefinition);
    }

    return rootClasses;
  }

  public ClassDefinition GetMandatory (Type classType)
  {
    ArgumentUtility.CheckNotNull ("classType", classType);

    if (!_areResolvedTypesRequired)
    {
      throw CreateInvalidOperationException (
          "Collection allows only ClassDefinitions with resolved types and therefore GetMandatory(Type) cannot be used.");
    }

    ClassDefinition classDefinition = this[classType];
    if (classDefinition == null)
      throw CreateMappingException ("Mapping does not contain class '{0}'.", classType);

    return classDefinition;
  }

  public ClassDefinition GetMandatory (string classID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

    ClassDefinition classDefinition = this[classID];
    if (classDefinition == null)
      throw CreateMappingException ("Mapping does not contain class '{0}'.", classID);

    return classDefinition;
  }

  public bool AreResolvedTypesRequired
  {
    get { return _areResolvedTypesRequired; }
  }

  #region Standard implementation for "add-only" collections

  public bool Contains (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    return BaseContains (classDefinition.ID, classDefinition);
  }

  public bool Contains (Type classType)
  {
    if (!_areResolvedTypesRequired)
    {
      throw CreateInvalidOperationException (
          "Collection allows only ClassDefinitions with resolved types and therefore Contains(Type) cannot be used.");
    }

    return _types.ContainsKey (classType);
  }

  public bool Contains (string classID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

    return BaseContainsKey (classID);
  }

  public ClassDefinition this [int index]  
  {
    get { return (ClassDefinition) BaseGetObject (index); }
  }

  public ClassDefinition this [Type classType]  
  {
    get 
    {
      if (!_areResolvedTypesRequired)
      {
        throw CreateInvalidOperationException (
            "Collection allows only ClassDefinitions with resolved types and therefore this overload of the indexer cannot be used.");
      }

      return (ClassDefinition) _types[classType]; 
    }
  }

  public ClassDefinition this [string classID]
  {
    get
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      return (ClassDefinition) BaseGetObject (classID);
    }
  }

  public int Add (ClassDefinition value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);
  
    if (_areResolvedTypesRequired)
    {
      if (value.ClassType == null)
      {
        throw CreateInvalidOperationException (
            "Collection allows only ClassDefinitions with resolved types and therefore ClassDefinition '{0}' cannot be added.", value.ID);
      }

      if (_types.Contains (value.ClassType))
        throw new ArgumentException (string.Format ("A ClassDefinition with Type '{0}' is already part of this collection.", value.ClassType), "value");
    }

    if (Contains (value.ID))
    {
      throw CreateMappingException ("Class '{0}' and '{1}' both have the same class ID '{2}'. Use the ClassIDAttribute to define unique IDs for these "
          + "classes. The assemblies involved are '{3}' and '{4}'.", this[value.ID].ClassType.FullName, value.ClassType.FullName, value.ID,
          this[value.ID].ClassType.Assembly.FullName, value.ClassType.Assembly.FullName);
    }

    int position = BaseAdd (value.ID, value);

    if (_areResolvedTypesRequired)
      _types.Add (value.ClassType, value);

    return position;
  }

  #endregion

  private void ValidateRootClass (ClassDefinition rootClass)
  {
    ValidateEntireInheritanceHierarchyIsPartOfCollection (rootClass);
    rootClass.GetValidator().ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    ValidateConcreteEntityNames (rootClass);
  }

  private void ValidateConcreteEntityNames (ClassDefinition rootClass)
  {
    Dictionary<string, object> allDistinctConcreteEntityNames = new Dictionary<string, object> ();
    foreach (string entityName in rootClass.GetAllConcreteEntityNames ())
    {
      if (allDistinctConcreteEntityNames.ContainsKey (entityName))
      {
        throw CreateMappingException (
            "At least two classes in different inheritance branches derived from abstract class '{0}'"
            + " specify the same entity name '{1}', which is not allowed.",
            rootClass.ID, entityName);
      }

      allDistinctConcreteEntityNames.Add (entityName, null);
    }
  }

  private void ValidateEntireInheritanceHierarchyIsPartOfCollection (ClassDefinition rootClass)
  {
    if (!Contains (rootClass))
    {
      throw CreateInvalidOperationException (
          "Validate cannot be invoked, because class '{0}' is a base class of a class in the collection,"
          + " but the base class is not part of the collection itself.",
          rootClass.ID);
    }

    foreach (ClassDefinition derivedClass in rootClass.GetAllDerivedClasses ())
    {
      if (!Contains (derivedClass))
      {
        throw CreateInvalidOperationException (
            "Validate cannot be invoked, because class '{0}' is a derived class of '{1}', but is not part of the collection itself.",
            derivedClass.ID, rootClass.ID);
      }
    }
  }

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }

  private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
  {
    return new InvalidOperationException (string.Format (message, args));
  }
}
}
