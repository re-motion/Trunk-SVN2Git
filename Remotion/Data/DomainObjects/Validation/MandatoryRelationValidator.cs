// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Validates the mandatory relations of a <see cref="DomainObject"/>, throwing a <see cref="MandatoryRelationNotSetException"/> when a mandatory
  /// relation is not set. Only complete relations are validated, no data is loaded by the validation.
  /// </summary>
  public class MandatoryRelationValidator : IPersistableDataValidator
  {
    public void Validate (PersistableData data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      if (data.DomainObjectState == StateType.Deleted)
        return;

      Assertion.IsTrue (
          data.DomainObjectState != StateType.NotLoadedYet && data.DomainObjectState != StateType.Invalid, 
          "No unloaded or invalid objects get this far.");

      foreach (var endPoint in data.GetAssociatedEndPoints())
      {
        if (endPoint.Definition.IsMandatory && endPoint.IsDataComplete)
            endPoint.ValidateMandatory ();
      }
    }
  }
}