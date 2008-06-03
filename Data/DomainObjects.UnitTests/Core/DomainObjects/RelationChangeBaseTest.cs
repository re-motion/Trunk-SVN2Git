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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DomainObjects
{
  public class RelationChangeBaseTest : ClientTransactionBaseTest
  {
    protected void CheckTouching (Proc modification, TestDomainBase foreignKeyObject, string simpleForeignKeyPropertyName,
                                params RelationEndPointID[] endPointsInvolved)
    {
      // Ensure all end points are loaded into the RelationEndPointMap before trying to check them
      foreach (RelationEndPointID id in endPointsInvolved)
      {
        if (id.Definition.Cardinality == CardinalityType.One)
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelatedObject (id, true);
        else
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelatedObjects (id);
      }

      if (foreignKeyObject != null)
      {
        Assert.IsFalse (
            foreignKeyObject.InternalDataContainer.PropertyValues[foreignKeyObject.GetPublicDomainObjectType() + "." + simpleForeignKeyPropertyName].
                HasBeenTouched,
            "ObjectID before modification");
      }

      foreach (RelationEndPointID id in endPointsInvolved)
        Assert.IsFalse (ClientTransactionMock.DataManager.RelationEndPointMap[id].HasBeenTouched, id + " before modification");

      modification ();

      if (foreignKeyObject != null)
      {
        Assert.IsTrue (
            foreignKeyObject.InternalDataContainer.PropertyValues[foreignKeyObject.GetPublicDomainObjectType() + "." + simpleForeignKeyPropertyName].
                HasBeenTouched,
            "ObjectID after modification");
      }

      foreach (RelationEndPointID id in endPointsInvolved)
        Assert.IsTrue (ClientTransactionMock.DataManager.RelationEndPointMap[id].HasBeenTouched, id + " after modification");
    }
  }
}
