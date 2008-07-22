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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  public class RelationDefinitionChecker
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public RelationDefinitionChecker ()
    {
    }

    // methods and properties

    public void Check (RelationDefinitionCollection expectedDefinitions, RelationDefinitionCollection actualDefinitions) 
    {
      Check (expectedDefinitions, actualDefinitions, false);
    }

    public void Check (RelationDefinitionCollection expectedDefinitions, RelationDefinitionCollection actualDefinitions, bool ignoreUnknown)
    {
      ArgumentUtility.CheckNotNull ("expectedDefinitions", expectedDefinitions);
      ArgumentUtility.CheckNotNull ("actualDefinitions", actualDefinitions);

      if (!ignoreUnknown)
        Assert.AreEqual (expectedDefinitions.Count, actualDefinitions.Count, "Number of relation definitions does not match.");

      foreach (RelationDefinition expectedDefinition in expectedDefinitions)
      {
        RelationDefinition actualDefinition = actualDefinitions[expectedDefinition.ID];
        Assert.IsNotNull (actualDefinition, "Relation '{0}' was not found.", expectedDefinition.ID);
        Check (expectedDefinition, actualDefinition);
      }
    }

    public void Check (RelationDefinition expectedDefinition, RelationDefinition actualDefinition)
    {
      ArgumentUtility.CheckNotNull ("expectedDefinition", expectedDefinition);
      ArgumentUtility.CheckNotNull ("actualDefinition", actualDefinition);

      Assert.AreEqual (expectedDefinition.ID, actualDefinition.ID, "IDs of relation definitions do not match.");

      CheckEndPointDefinitions (expectedDefinition, actualDefinition);
    }

    private void CheckEndPointDefinitions (RelationDefinition expectedRelationDefinition, RelationDefinition actualRelationDefinition)
    {
      foreach (IRelationEndPointDefinition expectedEndPointDefinition in expectedRelationDefinition.EndPointDefinitions)
      {
        IRelationEndPointDefinition actualEndPointDefinition = actualRelationDefinition.GetEndPointDefinition (
          expectedEndPointDefinition.ClassDefinition.ID, expectedEndPointDefinition.PropertyName);

        Assert.IsNotNull (
            actualEndPointDefinition,
            "End point definition was not found (relation definition: '{0}', class: '{1}', property name: '{2}'.",
            expectedRelationDefinition.ID,
            expectedEndPointDefinition.ClassDefinition.ID,
            expectedEndPointDefinition.PropertyName);

        CheckEndPointDefinition (expectedEndPointDefinition, actualEndPointDefinition, expectedRelationDefinition);
      }
    }

    private void CheckEndPointDefinition (
      IRelationEndPointDefinition expectedEndPointDefinition, 
      IRelationEndPointDefinition actualEndPointDefinition, 
      RelationDefinition relationDefinition)
    {
      Assert.AreEqual (expectedEndPointDefinition.GetType (), actualEndPointDefinition.GetType (), 
        "End point definitions (relation definition: '{0}', property name: '{1}') are not of same type.", 
        relationDefinition.ID,  
        expectedEndPointDefinition.PropertyName);

      Assert.AreEqual (expectedEndPointDefinition.ClassDefinition.ID, actualEndPointDefinition.ClassDefinition.ID, 
        "ClassDefinition of end point definitions (relation definition: '{0}', property name: '{1}') does not match.", 
        relationDefinition.ID,  
        expectedEndPointDefinition.PropertyName);

      Assert.AreEqual (expectedEndPointDefinition.RelationDefinition.ID, actualEndPointDefinition.RelationDefinition.ID, 
        "RelationDefinition of end point definitions (property name: '{0}') does not match.", 
        expectedEndPointDefinition.PropertyName);
    
      Assert.AreEqual (expectedEndPointDefinition.PropertyName, actualEndPointDefinition.PropertyName, 
        "PropertyName of end point definitions (relation definition: '{0}', property name: '{1}') does not match.", 
        relationDefinition.ID,  
        expectedEndPointDefinition.PropertyName);

      Assert.AreEqual (expectedEndPointDefinition.PropertyType, actualEndPointDefinition.PropertyType, 
        "PropertyType of end point definitions (relation definition: '{0}', property name: '{1}') does not match.", 
        relationDefinition.ID,  
        expectedEndPointDefinition.PropertyName);

      Assert.AreEqual (expectedEndPointDefinition.IsMandatory, actualEndPointDefinition.IsMandatory, 
        "IsMandatory of end point definitions (relation definition: '{0}', property name: '{1}') does not match. ", 
        relationDefinition.ID,  
        expectedEndPointDefinition.PropertyName);

      Assert.AreEqual (expectedEndPointDefinition.Cardinality, actualEndPointDefinition.Cardinality, 
        "Cardinality of end point definitions (relation definition: '{0}', property name: '{1}') does not match.", 
        relationDefinition.ID,  
        expectedEndPointDefinition.PropertyName);


      if (expectedEndPointDefinition is VirtualRelationEndPointDefinition)
      {
        VirtualRelationEndPointDefinition expectedVirtualEndPointDefinition = (VirtualRelationEndPointDefinition) expectedEndPointDefinition;
        VirtualRelationEndPointDefinition actualVirtualEndPointDefinition = (VirtualRelationEndPointDefinition) actualEndPointDefinition;

        Assert.AreEqual (expectedVirtualEndPointDefinition.SortExpression, actualVirtualEndPointDefinition.SortExpression, 
          "SortExpression of end point definitions (relation definition: '{0}', property name: '{1}') does not match.", 
          relationDefinition.ID,  
          expectedVirtualEndPointDefinition.PropertyName);
      }
    }      
  }
}
