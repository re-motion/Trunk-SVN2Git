/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   This interface defines essential validator features that are not included in IValidator.
///   <seealso cref="IValidator"/>
/// </summary>
/// <remarks>
///   This interface can be used for implementing validators that are not derived from <see cref="BaseValidator"/>. Code that uses <see cref="IValidator"/>
///   references should try to cast to <c>BaseValidator</c> AND <c>IBaseValidator</c>, and use either type's methods and properties.
/// </remarks>
/// <example>
///   void InitializeValidator (IValidator validator)
///   {
///     validator.ErrorMessage = "...";
///     
///     BaseValidator baseValidator = validator as BaseValidator;
///     if (baseValidator != null)
///       baseValidator.Display = ValidatorDisplay.None;
///      
///     IBaseValidator ibaseValidator = validator as IBaseValidator;
///     if (ibaseValidator != null)
///       ibaseValidator.Display = ValidatorDisplay.None;
///   }
/// </example>
public interface IBaseValidator: IValidator
{
  bool EnableClientScript { get; set; }

  string ControlToValidate { get; set; }
  ValidatorDisplay Display { get; set; }
}

}
