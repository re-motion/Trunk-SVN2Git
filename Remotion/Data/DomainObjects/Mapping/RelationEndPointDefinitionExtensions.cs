// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Provides extension methods for <see cref="IRelationEndPointDefinition"/>.
  /// </summary>
  public static class RelationEndPointDefinitionExtensions
  {
    // TODO 3176: This should always return a value or throw an exception. Remove GetMandatoryOppositeEndPointDefinition.
    public static IRelationEndPointDefinition GetOppositeEndPointDefinition (this IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      // TODO 3176: This should be an ArgumentException instead.
      Assertion.IsNotNull (relationEndPointDefinition.RelationDefinition, "Only fully initialized end points can be used.");

      return relationEndPointDefinition.RelationDefinition.GetOppositeEndPointDefinition (relationEndPointDefinition);
    }

    public static IRelationEndPointDefinition GetMandatoryOppositeEndPointDefinition (this IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      Assertion.IsNotNull (relationEndPointDefinition.RelationDefinition, "Only fully initialized end points can be used.");

      return relationEndPointDefinition.RelationDefinition.GetMandatoryOppositeRelationEndPointDefinition (relationEndPointDefinition);
    }

    public static ClassDefinition GetOppositeClassDefinition (this IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      // TODO 3176: This should be an ArgumentException instead.
      Assertion.IsNotNull (relationEndPointDefinition.RelationDefinition, "Only fully initialized end points can be used.");

      return relationEndPointDefinition.GetOppositeEndPointDefinition().ClassDefinition;
    }
  }
}
