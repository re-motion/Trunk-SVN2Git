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
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls
{

//  TODO: Command: Move long comment blocks to xml-file
/// <summary> A <see cref="NavigationCommand"/> defines an action the user can invoke when navigating between pages. </summary>
public class NavigationCommand: Command
{
  public NavigationCommand ()
    : this (CommandType.Href)
  {
  }

  public NavigationCommand (CommandType defaultType)
    : base (defaultType)
  {
  }

  /// <summary> Adds the attributes for the Wxe Function command to the anchor tag. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
  /// <param name="postBackEvent">
  ///   The string executed upon the click on a command of types
  ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
  ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
  ///   to force a post back. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="onClick"> 
  ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
  /// </param>
  /// <param name="additionalUrlParameters">
  ///   The <see cref="NameValueCollection"/> containing additional url parameters.
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <param name="includeNavigationUrlParameters"> 
  ///   <see langword="true"/> to include URL parameters provided by <see cref="ISmartNavigablePage"/>.
  /// </param>
  /// <exception cref="InvalidOperationException">
  ///   <para>
  ///     Thrown if called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  ///   </para><para>
  ///     Thrown if neither the <see cref="Command.WxeFunctionCommandInfo.MappingID"/> nor the 
  ///     <see cref="Command.WxeFunctionCommandInfo.TypeName"/> are set.
  ///   </para><para>
  ///     Thrown if the <see cref="Command.WxeFunctionCommandInfo.MappingID"/> and 
  ///     <see cref="Command.WxeFunctionCommandInfo.TypeName"/> specify different functions.
  ///   </para>
  /// </exception> 
  protected override void AddAttributesToRenderForWxeFunctionCommand (
      HtmlTextWriter writer, 
      string postBackEvent,
      string onClick,
      NameValueCollection additionalUrlParameters,
      bool includeNavigationUrlParameters)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    ArgumentUtility.CheckNotNull ("postBackEvent", postBackEvent); 
    ArgumentUtility.CheckNotNull ("additionalUrlParameters", additionalUrlParameters); 
    if (Type != CommandType.WxeFunction)
      throw new InvalidOperationException ("Call to AddAttributesToRenderForWxeFunctionCommand not allowed unless Type is set to CommandType.WxeFunction.");
       
    string href = "#";
    if (System.Web.HttpContext.Current != null)
      href = GetWxeFunctionPermanentUrl (additionalUrlParameters);
    writer.AddAttribute (HtmlTextWriterAttribute.Href, href);
    if (! StringUtility.IsNullOrEmpty (WxeFunctionCommand.Target))
      writer.AddAttribute (HtmlTextWriterAttribute.Target, WxeFunctionCommand.Target);
    if (! StringUtility.IsNullOrEmpty (onClick))
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClick);
    if (! StringUtility.IsNullOrEmpty (ToolTip))
      writer.AddAttribute (HtmlTextWriterAttribute.Title, ToolTip);
  }

  /// <summary> 
  ///   Gets the permanent URL for the <see cref="WxeFunction"/> defined by the 
  ///   <see cref="Command.WxeFunctionCommandInfo"/>.
  /// </summary>
  /// <param name="additionalUrlParameters">
  ///   The <see cref="NameValueCollection"/> containing additional url parameters. 
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <exception cref="InvalidOperationException">
  ///   <para>
  ///     Thrown if called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  ///   </para><para>
  ///     Thrown if neither the <see cref="Command.WxeFunctionCommandInfo.MappingID"/> nor the 
  ///     <see cref="Command.WxeFunctionCommandInfo.TypeName"/> are set.
  ///   </para><para>
  ///     Thrown if the <see cref="Command.WxeFunctionCommandInfo.MappingID"/> and 
  ///     <see cref="Command.WxeFunctionCommandInfo.TypeName"/> specify different functions.
  ///   </para>
  /// </exception> 
  public virtual string GetWxeFunctionPermanentUrl (NameValueCollection additionalUrlParameters)
  {
    ArgumentUtility.CheckNotNull ("additionalUrlParameters", additionalUrlParameters);

    if (Type != CommandType.WxeFunction)
      throw new InvalidOperationException ("Call to ExecuteWxeFunction not allowed unless Type is set to CommandType.WxeFunction.");

    Type functionType = WxeFunctionCommand.ResolveFunctionType();
    WxeParameterDeclaration[] parameterDeclarations = WxeVariablesContainer.GetParameterDeclarations (functionType);
    object[] parameterValues = WxeVariablesContainer.ParseActualParameters (parameterDeclarations, WxeFunctionCommand.Parameters, CultureInfo.InvariantCulture);

    NameValueCollection queryString = WxeVariablesContainer.SerializeParametersForQueryString (parameterDeclarations, parameterValues);
    queryString.Set (WxeHandler.Parameters.WxeReturnToSelf, true.ToString());
    NameValueCollectionUtility.Append (queryString, additionalUrlParameters);
    
    return WxeContext.GetPermanentUrl (new HttpContextWrapper(HttpContext.Current), functionType, queryString);
  }

  /// <summary> 
  ///   Gets the permanent URL for the <see cref="WxeFunction"/> defined by the 
  ///   <see cref="Command.WxeFunctionCommandInfo"/>.
  /// </summary>
  /// <exception cref="InvalidOperationException">
  ///   <para>
  ///     Thrown if called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  ///   </para><para>
  ///     Thrown if neither the <see cref="Command.WxeFunctionCommandInfo.MappingID"/> nor the 
  ///     <see cref="Command.WxeFunctionCommandInfo.TypeName"/> are set.
  ///   </para><para>
  ///     Thrown if the <see cref="Command.WxeFunctionCommandInfo.MappingID"/> and 
  ///     <see cref="Command.WxeFunctionCommandInfo.TypeName"/> specify different functions.
  ///   </para>
  /// </exception> 
  public string GetWxeFunctionPermanentUrl ()
  {
    return GetWxeFunctionPermanentUrl (new NameValueCollection (0));
  }
}
}
