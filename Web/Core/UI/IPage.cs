// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI
{

/// <summary>
///   This interface contains all public members of System.Web.UI.Page. It is used to derive interfaces that will be
///   implemented by deriving from System.Web.UI.Page.
/// </summary>
/// <remarks>
///   The reason for providing this interface is that derived interfaces do not need to be casted to System.Web.UI.Page.
/// </remarks>
public interface IPage: ITemplateControl, IHttpHandler
{
  void DesignerInitialize();
  string GetPostBackClientEvent(Control control, string argument);
  string GetPostBackClientHyperlink(Control control, string argument);
  string GetPostBackEventReference(Control control);
  string GetPostBackEventReference(Control control, string argument);
  int GetTypeHashCode();
  bool IsClientScriptBlockRegistered(string key);
  bool IsStartupScriptRegistered(string key);
  string MapPath(string virtualPath);
  void RegisterArrayDeclaration(string arrayName, string arrayValue);
  void RegisterClientScriptBlock(string key, string script);
  void RegisterHiddenField(string hiddenFieldName, string hiddenFieldInitialValue);
  void RegisterOnSubmitStatement(string key, string script);
  void RegisterRequiresPostBack(Control control);
  void RegisterRequiresRaiseEvent(IPostBackEventHandler control);
  void RegisterStartupScript(string key, string script);
  void RegisterViewStateHandler();
  void Validate();
  void VerifyRenderingInServerForm(Control control);

  HttpApplicationState Application { get; }
  Cache Cache { get; }
  string ClientTarget { get; set; }
  string ErrorPage { get; set; }
  bool IsPostBack { get; }
  bool IsValid { get; }
  HttpRequest Request { get; }
  HttpResponse Response { get; }
  HttpServerUtility Server { get; }
  HttpSessionState Session { get; }
  bool SmartNavigation { get; set; }
  TraceContext Trace { get; }
  IPrincipal User { get; }
  ValidatorCollection Validators { get; }
  string ViewStateUserKey { get; set; }
}
 
}
