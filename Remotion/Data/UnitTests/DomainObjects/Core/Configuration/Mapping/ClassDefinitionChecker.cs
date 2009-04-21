// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  public class ClassDefinitionChecker
  {
    public ClassDefinitionChecker()
    {
    }

    public void Check (ClassDefinition expectedDefinition, ClassDefinition actualDefinition)
    {
      ArgumentUtility.CheckNotNull ("expectedDefinition", expectedDefinition);
      ArgumentUtility.CheckNotNull ("actualDefinition", actualDefinition);

      Assert.AreEqual (expectedDefinition.ID, actualDefinition.ID, "IDs of class definitions do not match.");

      Assert.AreEqual (
          expectedDefinition.ClassType,
          actualDefinition.ClassType,
          "ClassType of class definition '{0}' does not match.",
          expectedDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.IsClassTypeResolved,
          actualDefinition.IsClassTypeResolved,
          "IsClassTypeResolved of class definition '{0}' does not match.",
          expectedDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.IsAbstract,
          actualDefinition.IsAbstract,
          "IsAbstract of class definition '{0}' does not match.",
          expectedDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.StorageProviderID,
          actualDefinition.StorageProviderID,
          "StorageProviderID of class definition '{0}' does not match. ",
          expectedDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.MyEntityName,
          actualDefinition.MyEntityName,
          "EntityName of class definition '{0}' does not match.",
          expectedDefinition.ID);

      if (expectedDefinition.BaseClass == null)
      {
        Assert.IsNull (actualDefinition.BaseClass, "actualDefinition.BaseClass of class definition '{0}' is not null.", expectedDefinition.ID);
      }
      else
      {
        Assert.IsNotNull (actualDefinition.BaseClass, "actualDefinition.BaseClass of class definition '{0}' is null.", expectedDefinition.ID);

        Assert.AreEqual (
            expectedDefinition.BaseClass.ID,
            actualDefinition.BaseClass.ID,
            "BaseClass of class definition '{0}' does not match.",
            expectedDefinition.ID);
      }

      CheckDerivedClasses (expectedDefinition.DerivedClasses, actualDefinition.DerivedClasses, expectedDefinition);
      CheckPropertyDefinitions (expectedDefinition.MyPropertyDefinitions, actualDefinition.MyPropertyDefinitions, expectedDefinition);
    }


    public void Check (ClassDefinitionCollection expectedDefinitions, ClassDefinitionCollection actualDefinitions)
    {
      Check (expectedDefinitions, actualDefinitions, true, false);
    }

    public void Check (ClassDefinitionCollection expectedDefinitions, ClassDefinitionCollection actualDefinitions, bool checkRelations)
    {
      Check (expectedDefinitions, actualDefinitions, checkRelations, false);
    }

    public void Check (
        ClassDefinitionCollection expectedDefinitions,
        ClassDefinitionCollection actualDefinitions,
        bool checkRelations,
        bool ignoreUnknown)
    {
      ArgumentUtility.CheckNotNull ("expectedDefinitions", expectedDefinitions);
      ArgumentUtility.CheckNotNull ("actualDefinitions", actualDefinitions);

      if (!ignoreUnknown)
        Assert.AreEqual (expectedDefinitions.Count, actualDefinitions.Count, "Number of class definitions does not match.");

      Assert.AreEqual (
          expectedDefinitions.AreResolvedTypesRequired,
          actualDefinitions.AreResolvedTypesRequired,
          "AreResolvedTypesRequired does not match.");

      foreach (ClassDefinition expectedDefinition in expectedDefinitions)
      {
        ClassDefinition actualDefinition = actualDefinitions[expectedDefinition.ClassType];
        Check (expectedDefinition, actualDefinition);

        if (checkRelations)
        {
          RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker();
          relationDefinitionChecker.Check (expectedDefinition.MyRelationDefinitions, actualDefinition.MyRelationDefinitions, ignoreUnknown);
        }
      }
    }

    private void CheckDerivedClasses (
        ClassDefinitionCollection expectedDerivedClasses,
        ClassDefinitionCollection actualDerivedClasses,
        ClassDefinition expectedClassDefinition)
    {
      Assert.AreEqual (
          expectedDerivedClasses.Count,
          actualDerivedClasses.Count,
          "Number of derived classes of class definition '{0}' does not match.",
          expectedClassDefinition.ID);

      Assert.AreEqual (
          expectedDerivedClasses.AreResolvedTypesRequired,
          actualDerivedClasses.AreResolvedTypesRequired,
          "AreResolvedTypeNamesRequired of DerivedClasses collection of class definition '{0}' does not match",
          expectedClassDefinition.ID);

      foreach (ClassDefinition expectedDerivedClass in expectedDerivedClasses)
      {
        Assert.IsNotNull (
            actualDerivedClasses[expectedDerivedClass.ID],
            "Actual class definition '{0}' does not contain expected derived class '{1}'.",
            expectedClassDefinition.ID,
            expectedDerivedClass.ID);
      }
    }

    private void CheckPropertyDefinitions (
        PropertyDefinitionCollection expectedDefinitions,
        PropertyDefinitionCollection actualDefinitions,
        ClassDefinition expectedClassDefinition)
    {
      Assert.AreEqual (
          expectedDefinitions.Count,
          actualDefinitions.Count,
          "Number of property definitions in class definition '{0}' does not match.",
          expectedClassDefinition.ID);

      foreach (PropertyDefinition expectedDefinition in expectedDefinitions)
      {
        PropertyDefinition actualDefinition = actualDefinitions[expectedDefinition.PropertyName];
        Assert.IsNotNull (actualDefinition, "Class '{0}' has no property '{1}'.", expectedClassDefinition.ID, expectedDefinition.PropertyName);
        CheckPropertyDefinition (expectedDefinition, actualDefinition, expectedClassDefinition);
      }
    }

    private void CheckPropertyDefinition (
        PropertyDefinition expectedDefinition,
        PropertyDefinition actualDefinition,
        ClassDefinition classDefinition)
    {
      Assert.AreEqual (
          expectedDefinition.PropertyName,
          actualDefinition.PropertyName,
          "PropertyNames of property definitions (class definition: '{0}') do not match.",
          classDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.ClassDefinition.ID,
          actualDefinition.ClassDefinition.ID,
          "ClassDefinitionID of property definition '{0}' does not match.",
          expectedDefinition.PropertyName);

      Assert.AreEqual (
          expectedDefinition.StorageClass,
          actualDefinition.StorageClass,
          "StorageClass of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName,
          classDefinition.ID);

      if (expectedDefinition.StorageClass == StorageClass.Persistent)
      {
        Assert.AreEqual (
            expectedDefinition.StorageSpecificName,
            actualDefinition.StorageSpecificName,
            "StorageSpecificName of property definition '{0}' (class definition: '{1}') does not match.",
            expectedDefinition.PropertyName,
            classDefinition.ID);
      }

      Assert.AreEqual (
          expectedDefinition.MaxLength,
          actualDefinition.MaxLength,
          "MaxLength of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName,
          classDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.PropertyType,
          actualDefinition.PropertyType,
          "PropertyType of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName,
          classDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.IsNullable,
          actualDefinition.IsNullable,
          "IsNullable of property definition '{0}' (class definition: '{1}') does not match",
          expectedDefinition.PropertyName,
          classDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.IsObjectID,
          actualDefinition.IsObjectID,
          "IsObjectID of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName,
          classDefinition.ID);
    }
  }
}
