﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Reflection;
using System.Linq;
using Remotion.Utilities;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Attributes.Validation
{
  /// <summary>
  /// Base class for validation attributes used to substitute the API-based <see cref="ComponentValidationCollector{TValidatedType}"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property)]
  public abstract class AddingValidationAttributeBase : Attribute
  {
    /// <summary>
    /// Gets or sets a flag whether the constraint can be removed by an other component.
    /// </summary>
    public bool IsHardConstraint { get; set; }

    /// <summary>
    /// Gets or sets the error message displayed when the validation fails.
    /// </summary>
    public string ErrorMessage { get; set; }

    protected abstract IEnumerable<IPropertyValidator> GetValidators (PropertyInfo property); 

    public IEnumerable<IPropertyValidator> GetPropertyValidators (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      
      var validators = GetValidators(property).ToArray();

      //TODO RM-5906: Set message from attribute
      //if (!string.IsNullOrEmpty (ErrorMessage))
      //{
      //  foreach (var validator in validators)
      //    validator.ErrorMessageSource = new StaticStringSource (ErrorMessage); //Note: currently only static error messages are supported!
      //}

      return validators;
    }
  }
}