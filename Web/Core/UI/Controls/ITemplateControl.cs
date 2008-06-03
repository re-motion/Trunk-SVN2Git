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

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   This interface contains all public members of System.Web.UI.TemplateControl. It is used to 
///   derive interfaces that will be implemented by deriving from System.Web.UI.TemplateControl.
/// </summary>
/// <remarks>
///   The reason for providing this interface is that derived interfaces do not need to be casted 
///   to System.Web.UI.TemplateControl.
/// </remarks>
public interface ITemplateControl: IControl, INamingContainer
{
  event EventHandler AbortTransaction;
  event EventHandler CommitTransaction;
  event EventHandler Error;

  Control LoadControl(string virtualPath);
  ITemplate LoadTemplate(string virtualPath);
  Control ParseControl(string content);
}

}
