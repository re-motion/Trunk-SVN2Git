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
using System.Collections.Specialized;
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   This interface declares the methods required by controls used for navigating between individual pages of a
///   web application.
/// </summary>
/// <remarks>
///   A <see cref="Control"/> implementing <see cref="INavigationControl"/> should check whether it is located on an
///   <see cref="ISmartNavigablePage"/> and if so, register itself using the 
///   <see cref="ISmartNavigablePage.RegisterNavigationControl"/> method during the <b>OnInit</b> phase of the page 
///   life cycle.
/// </remarks>
/// <seealso cref="ISmartNavigablePage"/>
public interface INavigationControl: IControl
{
  /// <summary> 
  ///   Provides the URL parameters containing the navigation information for this control (e.g. the selected tab).
  /// </summary>
  /// <returns> 
  ///   A <see cref="NameValueCollection"/> containing the URL parameters required by this 
  ///   <see cref="INavigationControl"/> to restore its navigation state when using hyperlinks.
  /// </returns>
  NameValueCollection GetNavigationUrlParameters();
}

}
