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
using System.Linq;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Results;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Rules
{
  /// <summary>Associates a set of <see cref="IPropertyValidator"/> implementations with an <see cref="IPropertyInformation"/> object.</summary>
  public class PropertyValidationRule : IValidationRule
  {
    public IPropertyInformation Property { get; }
    public IReadOnlyCollection<IPropertyValidator> Validators { get; }

    public PropertyValidationRule (IPropertyInformation property, IReadOnlyCollection<IPropertyValidator> validators)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      ArgumentUtility.CheckNotNull ("validators", validators);

      Property = property;
      Validators = validators;
    }

    public IEnumerable<ValidationFailure> Validate (ValidationContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var propertyName = context.PropertyChain.BuildPropertyName (Property.Name);
      if (context.Selector.CanExecute (this))
        return Validators.SelectMany (validator => ValidatePropertyValidator (context, propertyName, validator));
      else
        return Enumerable.Empty<ValidationFailure>();
    }

    private IEnumerable<ValidationFailure> ValidatePropertyValidator (
        ValidationContext context,
        string propertyName,
        IPropertyValidator validator)
    {
      var propertyValidatorContext = new PropertyValidatorContext (context, this, propertyName);
      return validator.Validate (propertyValidatorContext);
    }
  }
}