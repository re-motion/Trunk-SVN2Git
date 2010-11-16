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
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration
{
  public abstract class ViewBuilderBase
  {
    // types

    // static members and constants

    // member fields

    private StringBuilder _createViewStringBuilder;
    private StringBuilder _dropViewStringBuilder;

    // construction and disposing

    public ViewBuilderBase ()
    {
      _createViewStringBuilder = new StringBuilder ();
      _dropViewStringBuilder = new StringBuilder ();
    }

    // methods and properties

    public abstract void AddViewForConcreteClassToCreateViewScript (ClassDefinition classDefinition, StringBuilder createViewStringBuilder);
    public abstract void AddViewForAbstractClassToCreateViewScript (ClassDefinition classDefinition, ClassDefinitionCollection concreteClasses, StringBuilder createViewStringBuilder);
    public abstract void AddToDropViewScript (ClassDefinition classDefinition, StringBuilder dropViewStringBuilder);
    public abstract string CreateViewSeparator { get; }

    public string GetCreateViewScript ()
    {
      return _createViewStringBuilder.ToString ();
    }

    public string GetDropViewScript ()
    {
      return _dropViewStringBuilder.ToString ();
    }

    public void AddViews (ClassDefinitionCollection classDefinitions)
    {
      foreach (ClassDefinition classDefinition in classDefinitions)
        AddView (classDefinition);
    }

    public void AddView (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.GetEntityName () != null)
      {
        AddViewForConcreteClassToCreateViewScript (classDefinition);
        AddToDropViewScript (classDefinition);
      }
      else
      {
        ClassDefinitionCollection concreteClasses = GetConcreteClasses (classDefinition);
        if (concreteClasses.Count != 0)
        {
          AddViewForAbstractClassToCreateViewScript (classDefinition, concreteClasses);
          AddToDropViewScript (classDefinition);
        }
      }
    }

    private void AddViewForConcreteClassToCreateViewScript (ClassDefinition classDefinition)
    {
      AppendCreateViewSeparator ();
      AddViewForConcreteClassToCreateViewScript (classDefinition, _createViewStringBuilder);
    }

    private void AddViewForAbstractClassToCreateViewScript (ClassDefinition classDefinition, ClassDefinitionCollection concreteClasses)
    {
      AppendCreateViewSeparator ();
      AddViewForAbstractClassToCreateViewScript (classDefinition, concreteClasses, _createViewStringBuilder);
    }

    private void AppendCreateViewSeparator ()
    {
      if (_createViewStringBuilder.Length != 0)
        _createViewStringBuilder.Append (CreateViewSeparator);
    }

    private void AddToDropViewScript (ClassDefinition classDefinition)
    {
      if (_dropViewStringBuilder.Length != 0)
        _dropViewStringBuilder.Append ("\r\n");

      AddToDropViewScript (classDefinition, _dropViewStringBuilder);
    }

    protected ClassDefinitionCollection GetConcreteClasses (ClassDefinition abstractClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("abstractClassDefinition", abstractClassDefinition);

      ClassDefinitionCollection concreteClasses = new ClassDefinitionCollection (false);
      FillConcreteClasses (abstractClassDefinition, concreteClasses);
      return concreteClasses;
    }

    private void FillConcreteClasses (ClassDefinition classDefinition, ClassDefinitionCollection concreteClasses)
    {
      if (classDefinition.GetEntityName () != null)
      {
        concreteClasses.Add (classDefinition);
        return;
      }

      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
        FillConcreteClasses (derivedClass, concreteClasses);
    }

    protected ClassDefinitionCollection GetClassDefinitionsForWhereClause (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      ClassDefinitionCollection classDefinitionsForWhereClause = new ClassDefinitionCollection (false);

      if (classDefinition.GetEntityName () != null)
        classDefinitionsForWhereClause.Add (classDefinition);

      FillClassDefinitionsForWhereClauseWithDerivedClasses (classDefinition, classDefinitionsForWhereClause);

      return classDefinitionsForWhereClause;
    }

    private void FillClassDefinitionsForWhereClauseWithDerivedClasses (
        ClassDefinition classDefinition,
        ClassDefinitionCollection classDefinitionsForWhereClause)
    {
      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
      {
        if (derivedClass.GetEntityName () != null)
          classDefinitionsForWhereClause.Add (derivedClass);

        FillClassDefinitionsForWhereClauseWithDerivedClasses (derivedClass, classDefinitionsForWhereClause);
      }
    }

    protected bool IsPartOfInheritanceBranch (ClassDefinition classDefinitionOfBranch, ClassDefinition classDefinitionToEvaluate)
    {
      ArgumentUtility.CheckNotNull ("classDefinitionOfBranch", classDefinitionOfBranch);
      ArgumentUtility.CheckNotNull ("classDefinitionToEvaluate", classDefinitionToEvaluate);

      if (classDefinitionOfBranch == classDefinitionToEvaluate)
        return true;

      ClassDefinition baseClass = classDefinitionOfBranch.BaseClass;
      while (baseClass != null)
      {
        if (baseClass == classDefinitionToEvaluate)
          return true;
        baseClass = baseClass.BaseClass;
      }

      return IsDerivedClass (classDefinitionOfBranch, classDefinitionToEvaluate);
    }

    private bool IsDerivedClass (ClassDefinition classDefinition, ClassDefinition classDefinitionToEvaluate)
    {
      if (classDefinition.DerivedClasses.Contains (classDefinitionToEvaluate))
        return true;

      foreach (ClassDefinition derivedClasses in classDefinition.DerivedClasses)
      {
        if (IsDerivedClass (derivedClasses, classDefinitionToEvaluate))
          return true;
      }

      return false;
    }

    protected IEnumerable<IGrouping<string , PropertyDefinition>> GetGroupedPropertyDefinitions (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      List<PropertyDefinition> allPropertyDefinitions = new List<PropertyDefinition> ();
      FillAllPropertyDefinitionsFromBaseClasses (classDefinition, allPropertyDefinitions);

      foreach (PropertyDefinition propertyDefinitionInDerivedClass in classDefinition.MyPropertyDefinitions.GetAllPersistent ())
        allPropertyDefinitions.Add (propertyDefinitionInDerivedClass);

      FillAllPropertyDefinitionsFromDerivedClasses (classDefinition, allPropertyDefinitions);

      return allPropertyDefinitions.GroupBy (propertyDefinition => propertyDefinition.StoragePropertyDefinition.Name);
    }

    private void FillAllPropertyDefinitionsFromBaseClasses (ClassDefinition classDefinition, List<PropertyDefinition> allPropertyDefinitions)
    {
      if (classDefinition.BaseClass == null)
        return;

      FillAllPropertyDefinitionsFromBaseClasses (classDefinition.BaseClass, allPropertyDefinitions);

      foreach (PropertyDefinition propertyDefinitionInDerivedClass in classDefinition.BaseClass.MyPropertyDefinitions.GetAllPersistent ())
        allPropertyDefinitions.Add (propertyDefinitionInDerivedClass);
    }

    private void FillAllPropertyDefinitionsFromDerivedClasses (ClassDefinition classDefinition, List<PropertyDefinition> allPropertyDefinitions)
    {
      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
      {
        foreach (PropertyDefinition propertyDefinitionInDerivedClass in derivedClass.MyPropertyDefinitions.GetAllPersistent ())
          allPropertyDefinitions.Add (propertyDefinitionInDerivedClass);

        FillAllPropertyDefinitionsFromDerivedClasses (derivedClass, allPropertyDefinitions);
      }
    }
  }
}
