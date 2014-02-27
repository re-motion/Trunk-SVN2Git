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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FluentValidation;
using FluentValidation.Results;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Utilities;
using Remotion.Validation;
using Remotion.Validation.Implementation;

namespace Remotion.Data.DomainObjects.Validation
{
  public class ValidationClientTransactionExtension : ClientTransactionExtensionBase
  {
    private readonly IValidatorBuilder _validatorBuilder;

    public static string DefaultKey
    {
      get { return typeof (ValidationClientTransactionExtension).FullName; }
    }

    public ValidationClientTransactionExtension (IValidatorBuilder validatorBuilder)
        : this (DefaultKey, validatorBuilder)
    {
    }

    public ValidationClientTransactionExtension (string key, IValidatorBuilder validatorBuilder)
        : base (key)
    {
      ArgumentUtility.CheckNotNull ("validatorBuilder", validatorBuilder);

      _validatorBuilder = validatorBuilder;
    }

    public IValidatorBuilder ValidatorBuilder
    {
      get { return _validatorBuilder; }
    }

    public override void CommitValidate (ClientTransaction clientTransaction, ReadOnlyCollection<PersistableData> committedData)
    {
      var invalidValidationResults = new List<ValidationResult>();
      foreach (var item in committedData)
      {
        if (item.DomainObjectState == StateType.Deleted)
          continue;

        Assertion.IsTrue (
            item.DomainObjectState != StateType.NotLoadedYet && item.DomainObjectState != StateType.Invalid,
            "No unloaded or invalid objects get this far.");

        var validator = _validatorBuilder.BuildValidator (item.DomainObject.GetPublicDomainObjectType());
        var validationResult = validator.Validate (item.DomainObject);
        if(!validationResult.IsValid)
          invalidValidationResults.Add(validationResult);
      }

      if (invalidValidationResults.Any())
        throw new ValidationException (invalidValidationResults.SelectMany (vr => vr.Errors));
    }
  }
}