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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Implements <see cref="IRelationEndPointRegistrationAgent"/> for virtual object-valued relation end-points.
  /// </summary>
  [Serializable]
  public class FetchedVirtualObjectRelationDataRegistrationAgent : FetchedRelationDataRegistrationAgentBase
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (FetchedVirtualObjectRelationDataRegistrationAgent));

    public override void GroupAndRegisterRelatedObjects (
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject[] originatingObjects,
        DomainObject[] relatedObjects,
        IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("originatingObjects", originatingObjects);
      ArgumentUtility.CheckNotNull ("relatedObjects", relatedObjects);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      if (relationEndPointDefinition.Cardinality != CardinalityType.One || !relationEndPointDefinition.IsVirtual)
      {
        throw new ArgumentException (
            "Only virtual object-valued relation end-points can be handled by this registration agent.", 
            "relationEndPointDefinition");
      }

      // TODO 3646: Register data

      CheckOriginatingObjects (relationEndPointDefinition, originatingObjects);
      CheckRelatedObjects (relationEndPointDefinition, relatedObjects);
    }
  }
}