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
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration
{
  public abstract class ConstraintBuilderBase
  {
    // types

    // static members and constants

    // member fields

    private StringBuilder _createConstraintStringBuilder;
    private List<string> _entityNamesForDropConstraintScript;
    private Hashtable _constraintNamesUsed;

    // construction and disposing

    public ConstraintBuilderBase ()
    {
      _createConstraintStringBuilder = new StringBuilder();
      _constraintNamesUsed = new Hashtable();
      _entityNamesForDropConstraintScript = new List<string>();
    }

    // methods and properties

    public abstract void AddToDropConstraintScript (List<string> entityNamesForDropConstraintScript, StringBuilder dropConstraintStringBuilder);
    public abstract void AddToCreateConstraintScript (ClassDefinition classDefinition, StringBuilder createConstraintStringBuilder);

    public abstract string GetConstraint (
        ClassDefinition classDefinition,
        IRelationEndPointDefinition relationEndPoint,
        PropertyDefinition propertyDefinition,
        ClassDefinition oppositeClassDefinition);

    protected abstract string ConstraintSeparator { get; }

    public string GetAddConstraintScript ()
    {
      return _createConstraintStringBuilder.ToString();
    }

    public string GetDropConstraintScript ()
    {
      if (_entityNamesForDropConstraintScript.Count == 0)
        return string.Empty;

      StringBuilder dropConstraintStringBuilder = new StringBuilder();
      AddToDropConstraintScript (_entityNamesForDropConstraintScript, dropConstraintStringBuilder);
      return dropConstraintStringBuilder.ToString();
    }

    public void AddConstraints (ClassDefinitionCollection classes)
    {
      ArgumentUtility.CheckNotNull ("classes", classes);

      foreach (ClassDefinition currentClass in classes)
        AddConstraint (currentClass);
    }

    public void AddConstraint (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (TableBuilderBase.IsConcreteTable (classDefinition))
      {
        AddToCreateConstraintScript (classDefinition);
        _entityNamesForDropConstraintScript.Add (classDefinition.MyEntityName);
      }
    }

    private void AddToCreateConstraintScript (ClassDefinition classDefinition)
    {
      if (_createConstraintStringBuilder.Length != 0)
        _createConstraintStringBuilder.Append ("\r\n");
      int length = _createConstraintStringBuilder.Length;

      AddToCreateConstraintScript (classDefinition, _createConstraintStringBuilder);

      if (_createConstraintStringBuilder.Length == length && length > 1)
        _createConstraintStringBuilder.Remove (length - 2, 2);
    }

    protected List<IRelationEndPointDefinition> GetAllRelationEndPoints (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      List<IRelationEndPointDefinition> allRelationEndPointDefinitions = new List<IRelationEndPointDefinition>();
      if (classDefinition.BaseClass != null)
        allRelationEndPointDefinitions.AddRange (classDefinition.BaseClass.GetRelationEndPointDefinitions());

      FillAllRelationEndPointDefinitionsWithParticularAndDerivedClass (classDefinition, allRelationEndPointDefinitions);

      return allRelationEndPointDefinitions;
    }

    protected List<string> GetConstraints (ClassDefinition tableRootClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("tableRootClassDefinition", tableRootClassDefinition);

      List<string> constraints = new List<string>();
      foreach (IRelationEndPointDefinition relationEndPoint in GetAllRelationEndPoints (tableRootClassDefinition))
      {
        string constraint = GetConstraint (tableRootClassDefinition, relationEndPoint);
        if (constraint != null)
          constraints.Add (constraint);
      }

      return constraints;
    }

    private string GetConstraint (ClassDefinition tableRootClassDefinition, IRelationEndPointDefinition relationEndPoint)
    {
      if (relationEndPoint.IsAnonymous)
        return null;

      ClassDefinition oppositeClassDefinition = relationEndPoint.ClassDefinition.GetMandatoryOppositeClassDefinition (relationEndPoint.PropertyName);

      if (!HasConstraint (relationEndPoint, oppositeClassDefinition))
        return null;

      PropertyDefinition propertyDefinition = relationEndPoint.ClassDefinition.GetMandatoryPropertyDefinition (relationEndPoint.PropertyName);
      if (propertyDefinition.StorageClass != StorageClass.Persistent)
        return null;

      return GetConstraint (tableRootClassDefinition, relationEndPoint, propertyDefinition, oppositeClassDefinition);
    }

    private void FillAllRelationEndPointDefinitionsWithParticularAndDerivedClass (
        ClassDefinition classDefinition, List<IRelationEndPointDefinition> allRelationEndPointDefinitions)
    {
      foreach (RelationDefinition relationDefinition in classDefinition.MyRelationDefinitions)
      {
        foreach (IRelationEndPointDefinition relationEndPointDefinition in relationDefinition.EndPointDefinitions)
        {
          if (relationEndPointDefinition.ClassDefinition == classDefinition)
            allRelationEndPointDefinitions.Add (relationEndPointDefinition);
        }
      }

      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
        FillAllRelationEndPointDefinitionsWithParticularAndDerivedClass (derivedClass, allRelationEndPointDefinitions);
    }

    private bool HasConstraint (IRelationEndPointDefinition relationEndPoint, ClassDefinition oppositeClassDefinition)
    {
      if (relationEndPoint.IsVirtual)
        return false;

      if (oppositeClassDefinition.StorageProviderID != relationEndPoint.ClassDefinition.StorageProviderID)
        return false;

      if (oppositeClassDefinition.GetEntityName() == null)
        return false;

      return true;
    }
  }
}
