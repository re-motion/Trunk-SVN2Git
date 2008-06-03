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
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{

public interface IUserControl: ITemplateControl, IAttributeAccessor, IUserControlDesignerAccessor
{
  void DesignerInitialize();
  void InitializeAsUserControl(Page page);
  string MapPath(string virtualPath);

  HttpApplicationState Application { get; }
  AttributeCollection Attributes { get; }
  Cache Cache { get; }
  bool IsPostBack { get; }
  HttpRequest Request { get; }
  HttpResponse Response { get; }
  HttpServerUtility Server { get; }
  HttpSessionState Session { get; }
  TraceContext Trace { get; }
}

}
