// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  public class RelationEndPointDefinitionChecker
  {
    public void Check (RelationEndPointDefinitionCollection expectedDefinitions, RelationEndPointDefinitionCollection actualDefinitions)
    {
      Assert.AreEqual (expectedDefinitions.Count, actualDefinitions.Count, "Number of relation end points does not match.");

      foreach (var expectedDefinition in expectedDefinitions)
      {
        var actualDefinition = actualDefinitions[expectedDefinition.PropertyName];
        Assert.IsNotNull (actualDefinition, "Relation end point '{0}' was not found.", expectedDefinition.PropertyName);
        Check ((RelationEndPointDefinitionCollection) expectedDefinition, (RelationEndPointDefinitionCollection) actualDefinition);
      }
    }

    public void Check (
        IRelationEndPointDefinition expectedEndPointDefinition, 
        IRelationEndPointDefinition actualEndPointDefinition)
    {
      Assert.AreEqual (expectedEndPointDefinition.GetType (), actualEndPointDefinition.GetType (), 
                       "End point definitions (property name: '{0}') are not of same type.", 
                       expectedEndPointDefinition.PropertyName);

      Assert.AreEqual (expectedEndPointDefinition.ClassDefinition.ID, actualEndPointDefinition.ClassDefinition.ID, 
                       "ClassDefinition of end point definitions (property name: '{0}') does not match.", 
                       expectedEndPointDefinition.PropertyName);

      Assert.AreEqual (expectedEndPointDefinition.RelationDefinition.ID, actualEndPointDefinition.RelationDefinition.ID, 
                       "RelationDefinition of end point definitions (property name: '{0}') does not match.", 
                       expectedEndPointDefinition.PropertyName);
    
      Assert.AreEqual (expectedEndPointDefinition.PropertyName, actualEndPointDefinition.PropertyName, 
                       "PropertyName of end point definitions (property name: '{0}') does not match.", 
                       expectedEndPointDefinition.PropertyName);

      Assert.AreEqual (expectedEndPointDefinition.PropertyType, actualEndPointDefinition.PropertyType, 
                       "PropertyType of end point definitions (property name: '{0}') does not match.", 
                       expectedEndPointDefinition.PropertyName);

      Assert.AreEqual (expectedEndPointDefinition.IsMandatory, actualEndPointDefinition.IsMandatory, 
                       "IsMandatory of end point definitions (property name: '{0}') does not match. ", 
                       expectedEndPointDefinition.PropertyName);

      Assert.AreEqual (expectedEndPointDefinition.Cardinality, actualEndPointDefinition.Cardinality, 
                       "Cardinality of end point definitions (property name: '{0}') does not match.", 
                       expectedEndPointDefinition.PropertyName);


      if (expectedEndPointDefinition is VirtualRelationEndPointDefinition)
      {
        var expectedVirtualEndPointDefinition = (VirtualRelationEndPointDefinition) expectedEndPointDefinition;
        var actualVirtualEndPointDefinition = (VirtualRelationEndPointDefinition) actualEndPointDefinition;

        Assert.AreEqual (expectedVirtualEndPointDefinition.SortExpressionText, actualVirtualEndPointDefinition.SortExpressionText, 
                         "SortExpression of end point definitions (property name: '{0}') does not match.", 
                         expectedVirtualEndPointDefinition.PropertyName);
      }
    }
  }
}