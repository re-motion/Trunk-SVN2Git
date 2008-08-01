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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web
{
  /// <summary>
  ///   Provides services for business object bound web applications
  /// </summary>
  public interface IBusinessObjectWebUIService : IBusinessObjectService
  {
    IconInfo GetIcon (IBusinessObject obj);
    string GetToolTip (IBusinessObject obj);

    /// <summary>
    /// Returns a <see cref="HelpInfo"/> object for the specified parameters.
    /// </summary>
    /// <param name="control">The <see cref="IBusinessObjectBoundWebControl"/> for which the help-link is to be generated.</param>
    /// <param name="businessObjectClass">The <see cref="IBusinessObjectClass"/> bound to the <paramref name="control"/>'s datasource.</param>
    /// <param name="businessObjectProperty">
    /// The <see cref="IBusinessObjectProperty"/> displayed by the <paramref name="control"/>. Can be <see langword="null" />.
    /// </param>
    /// <param name="businessObject">
    /// The <see cref="IBusinessObject"/> bound to the <paramref name="control"/>. 
    /// Can be <see langword="null" /> if the control's datasource has no value.
    /// </param>
    /// <returns>An instance of the <see cref="HelpInfo"/> type or <see langword="null" /> to not generate a help-link.</returns>
    HelpInfo GetHelpInfo (
        IBusinessObjectBoundWebControl control,
        IBusinessObjectClass businessObjectClass,
        IBusinessObjectProperty businessObjectProperty,
        IBusinessObject businessObject);
  }
}