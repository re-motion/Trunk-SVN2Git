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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  public class RelationChangeBaseTest : ClientTransactionBaseTest
  {
    protected void CheckTouching (Action modification, TestDomainBase foreignKeyObject, string simpleForeignKeyPropertyName,
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
