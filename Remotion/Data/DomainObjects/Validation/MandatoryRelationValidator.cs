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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Validates the mandatory relations of a <see cref="DomainObject"/>, throwing a <see cref="MandatoryRelationNotSetException"/> when a mandatory
  /// relation is not set. Only complete relations are validated, no data is loaded by the validation.
  /// </summary>
  public class MandatoryRelationValidator : IDomainObjectValidator
  {
    public static MandatoryRelationValidator CreateForClientTransaction (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      return new MandatoryRelationValidator (clientTransaction.DataManager);
    }

    private readonly IDataManager _dataManager;

    public MandatoryRelationValidator (IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      _dataManager = dataManager;
    }

    public IDataManager DataManager
    {
      get { return _dataManager; }
    }

    public void Validate (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      foreach (RelationEndPointID endPointID in RelationEndPointID.GetAllRelationEndPointIDs (domainObject.ID))
      {
        if (endPointID.Definition.IsMandatory)
        {
          var endPoint = _dataManager.GetRelationEndPointWithoutLoading (endPointID);
          if (endPoint != null && endPoint.IsDataComplete)
            endPoint.ValidateMandatory ();
        }
      }
    }
  }
}