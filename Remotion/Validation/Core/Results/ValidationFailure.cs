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
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Validation.Results
{
  public class ValidationFailure
  {
    public IPropertyInformation Property { get; }

    /// <summary>The technical representation of this validation error. Intended for logging.</summary>
    [NotNull]
    public string ErrorMessage { get; }

    /// <summary>The localized representation of this validation error. Intended for display in the user interface.</summary>
    [NotNull]
    public string LocalizedValidationMessage { get; }

    /// <summary>Custom state associated with the failure.</summary>
    public object CustomState { get; set; }

    public ValidationFailure (
        [NotNull] IPropertyInformation property,
        [NotNull] string errorMessage)
        : this (property, errorMessage, errorMessage)
    {
      // TODO RM-5906 remove overload
    }

    public ValidationFailure (
        [NotNull] IPropertyInformation property,
        [NotNull] string errorMessage,
        [NotNull] string localizedValidationMessage)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      ArgumentUtility.CheckNotNullOrEmpty ("errorMessage", errorMessage);
      ArgumentUtility.CheckNotNullOrEmpty ("localizedValidationMessage", localizedValidationMessage);

      Property = property;
      ErrorMessage = errorMessage;
      LocalizedValidationMessage = localizedValidationMessage;
    }
  }
}