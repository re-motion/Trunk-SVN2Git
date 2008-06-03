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
using System.ComponentModel;
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   This interface contains all public members of System.Web.UI.Control. It is used to derive interfaces that will be
///   implemented by deriving from System.Web.UI.Control.
/// </summary>
/// <remarks>
///   The reason for providing this interface is that derived interfaces do not need to be casted to System.Web.UI.Control.
/// </remarks>
public interface IControl: IComponent, IDataBindingsAccessor, IParserAccessor
{
  event EventHandler DataBinding;
  event EventHandler Init;
  event EventHandler Load;
  event EventHandler PreRender;
  event EventHandler Unload;

  void DataBind();
  Control FindControl(string id);
  bool HasControls();

  void RenderControl(HtmlTextWriter writer);
  void SetRenderMethodDelegate(RenderMethod renderMethod);

  Control BindingContainer { get; }
  string ClientID { get; }
  ControlCollection Controls { get; }
  bool EnableViewState { get; set; }
  string ID { get; set; }
  Control NamingContainer { get; }
  Page Page { get; set; }
  Control Parent { get; }
  string TemplateSourceDirectory { get; }
  string UniqueID { get; }
  bool Visible { get; set; }
}

}
