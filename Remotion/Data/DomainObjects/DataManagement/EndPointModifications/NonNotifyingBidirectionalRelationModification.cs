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
namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  /// <summary>
  /// Represents a bidirectional relation modification that only calls <see cref="RelationEndPointModification.Perform"/> on its 
  /// steps. Specifically, it does not notify the client transaction when its steps are executed, and it does not
  /// call <see cref="RelationEndPointModification.Begin"/> or <see cref="RelationEndPointModification.End"/>.
  /// This is used when <see cref="RelationEndPointModification.CreateBidirectionalModification"/> is called on a 
  /// <see cref="RelationEndPointModification"/> such as <see cref="ObjectEndPointSetSameModification"/> or 
  /// <see cref="CollectionEndPointSelfReplaceModification"/> that should not raise events but still perform the operation (in order to touch the 
  /// data stored by the end point, for example).
  /// </summary>
  public class NonNotifyingBidirectionalRelationModification : BidirectionalRelationModificationBase
  {
    public NonNotifyingBidirectionalRelationModification (params RelationEndPointModification[] modifications)
        : base(modifications)
    {
    }

    public void Perform ()
    {
      foreach (RelationEndPointModification modification in GetModificationSteps ())
        modification.Perform ();
    }

    public override void ExecuteAllSteps ()
    {
      Perform ();
    }
  }
}