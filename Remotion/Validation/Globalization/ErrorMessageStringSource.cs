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
using FluentValidation.Resources;
using FluentValidation.Validators;
using Remotion.Utilities;

namespace Remotion.Validation.Globalization
{
  public class ErrorMessageStringSource : IStringSource
  {
    private readonly IPropertyValidator _propertyValidator;
    private readonly IErrorMessageGlobalizationService _validatorGlobalizationService;
    private readonly string _resourceName;
    private readonly Type _resourceType;
    private readonly string _defaultErrorMessage;

    public ErrorMessageStringSource (IPropertyValidator propertyValidator, IErrorMessageGlobalizationService validatorGlobalizationService)
    {
      ArgumentUtility.CheckNotNull ("propertyValidator", propertyValidator);
      ArgumentUtility.CheckNotNull ("validatorGlobalizationService", validatorGlobalizationService);

      _propertyValidator = propertyValidator;
      _validatorGlobalizationService = validatorGlobalizationService;

      _defaultErrorMessage = propertyValidator.ErrorMessageSource.GetString();
      _resourceName = propertyValidator.GetType().FullName;
      _resourceType = validatorGlobalizationService.GetType();
    }

    public string GetString ()
    {
      var validatorErrorMessage = _propertyValidator.ErrorMessageSource;
      var errorMessage = _validatorGlobalizationService.GetErrorMessage (_propertyValidator) ?? _defaultErrorMessage;
      Assertion.IsTrue (ReferenceEquals (_propertyValidator.ErrorMessageSource, validatorErrorMessage));
      return errorMessage;
    }

    public string ResourceName
    {
      get { return _resourceName; }
    }

    public Type ResourceType
    {
      get { return _resourceType; }
    }
  }
}