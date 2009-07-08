// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using System.ComponentModel;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.Adapters;
using System.Web.UI.HtmlControls;
using Remotion.Utilities;
using Remotion.Web.UI;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// The <see cref="PageWrapper"/> type is the default implementation of the <see cref="IPage"/> interface.
  /// </summary>
  public class PageWrapper : IPage
  {
    public static IPage CastOrCreate (Page page)
    {
      if (page == null)
        return null;
      else if (page is IPage)
        return (IPage) page;
      else
        return new PageWrapper (page);
    }

    private readonly Page _page;

    public PageWrapper (Page page)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      _page = page;
    }

    public Page WrappedInstance
    {
      get { return _page; }
    }

    /// <summary>
    /// When implemented by an ASP.NET server control, notifies the server control that an element, either XML or HTML, was parsed.
    /// </summary>
    /// <param name="obj">The <see cref="T:System.Object"/> that was parsed. 
    /// </param>
    public void AddParsedSubObject (object obj)
    {
      ((IParserAccessor) _page).AddParsedSubObject (obj);
    }

    public DataBindingCollection DataBindings
    {
      get { return ((IDataBindingsAccessor) _page).DataBindings; }
    }

    /// <summary>
    /// Gets a value indicating whether the control contains any data-binding logic.
    /// </summary>
    /// <returns>
    /// true if the control contains data binding logic.
    /// </returns>
    public bool HasDataBindings
    {
      get { return ((IDataBindingsAccessor)_page).HasDataBindings; }
    }

    /// <summary>
    /// Gets the control builder for this control.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.ControlBuilder"/> that built the control; otherwise, null if no builder was used.
    /// </returns>
    public ControlBuilder ControlBuilder
    {
      get { return ((IControlBuilderAccessor)_page).ControlBuilder; }
    }

    /// <summary>
    /// When implemented, gets the state from the control during use on the design surface.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IDictionary"/> of the control state.
    /// </returns>
    public IDictionary GetDesignModeState ()
    {
      return ((IControlDesignerAccessor) _page).GetDesignModeState();
    }

    /// <summary>
    /// When implemented, sets control state before rendering it on the design surface.
    /// </summary>
    /// <param name="data">The <see cref="T:System.Collections.IDictionary"/> containing the control state.
    /// </param>
    public void SetDesignModeState (IDictionary data)
    {
      ((IControlDesignerAccessor) _page).SetDesignModeState (data);
    }

    /// <summary>
    /// When implemented, specifies the control that acts as the owner to the control implementing this method.
    /// </summary>
    /// <param name="owner">The control to act as owner.
    /// </param>
    public void SetOwnerControl (Control owner)
    {
      ((IControlDesignerAccessor) _page).SetOwnerControl (owner);
    }

    /// <summary>
    /// When implemented, gets a collection of information that can be accessed by a control designer.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IDictionary"/> containing information about the control.
    /// </returns>
    public IDictionary UserData
    {
      get { return ((IControlDesignerAccessor) _page).UserData; }
    }

    /// <summary>
    /// Gets a value indicating whether the instance of the class that implements this interface has any properties bound by an expression.
    /// </summary>
    /// <returns>
    /// true if the control has properties set through expressions; otherwise, false. 
    /// </returns>
    public bool HasExpressions
    {
      get { return ((IExpressionsAccessor) _page).HasExpressions; }
    }

    /// <summary>
    /// Gets a collection of <see cref="T:System.Web.UI.ExpressionBinding"/> objects.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.UI.ExpressionBindingCollection"/> containing 
    /// <see cref="T:System.Web.UI.ExpressionBinding"/> objects that represent the properties and expressions for a control.
    /// </returns>
    public ExpressionBindingCollection Expressions
    {
      get { return ((IExpressionsAccessor) _page).Expressions; }
    }

    /// <summary>
    /// Applies the style properties defined in the page style sheet to the control.
    /// </summary>
    /// <param name="page">The <see cref="T:System.Web.UI.Page"/> containing the control.
    /// </param><exception cref="T:System.InvalidOperationException">The style sheet is already applied.
    /// </exception>
    public void ApplyStyleSheetSkin (Page page)
    {
      _page.ApplyStyleSheetSkin (page);
    }

    /// <summary>
    /// Binds a data source to the invoked server control and all its child controls.
    /// </summary>
    public void DataBind ()
    {
      _page.DataBind();
    }

    /// <summary>
    /// Sets input focus to a control.
    /// </summary>
    public void Focus ()
    {
      _page.Focus();
    }

    /// <summary>
    /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object 
    /// and stores tracing information about the control if tracing is enabled.
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content. 
    /// </param>
    public void RenderControl (HtmlTextWriter writer)
    {
      _page.RenderControl (writer);
    }

    /// <summary>
    /// Enables a server control to perform final clean up before it is released from memory.
    /// </summary>
    public void Dispose ()
    {
      _page.Dispose();
    }

    /// <summary>
    /// Converts a URL into one that is usable on the requesting client.
    /// </summary>
    /// <returns>
    /// The converted URL.
    /// </returns>
    /// <param name="relativeUrl">The URL associated with the <see cref="P:System.Web.UI.Control.TemplateSourceDirectory"/> property. 
    /// </param><exception cref="T:System.ArgumentNullException">Occurs if the <paramref name="relativeUrl"/> parameter contains null. 
    /// </exception>
    public string ResolveUrl (string relativeUrl)
    {
      return _page.ResolveUrl (relativeUrl);
    }

    /// <summary>
    /// Gets a URL that can be used by the browser.
    /// </summary>
    /// <returns>
    /// A fully qualified URL to the specified resource suitable for use on the browser.
    /// </returns>
    /// <param name="relativeUrl">A URL relative to the current page.
    /// </param><exception cref="T:System.ArgumentNullException"><paramref name="relativeUrl"/> is null.
    /// </exception>
    public string ResolveClientUrl (string relativeUrl)
    {
      return _page.ResolveClientUrl (relativeUrl);
    }

    /// <summary>
    /// Determines if the server control contains any child controls.
    /// </summary>
    /// <returns>
    /// true if the control contains other controls; otherwise, false.
    /// </returns>
    public bool HasControls ()
    {
      return _page.HasControls();
    }

    /// <summary>
    /// Assigns an event handler delegate to render the server control and its content into its parent control.
    /// </summary>
    /// <param name="renderMethod">The information necessary to pass to the delegate so that it can render the server control. 
    /// </param>
    public void SetRenderMethodDelegate (RenderMethod renderMethod)
    {
      _page.SetRenderMethodDelegate (renderMethod);
    }

    /// <summary>
    /// Gets the server control identifier generated by ASP.NET.
    /// </summary>
    /// <returns>
    /// The server control identifier generated by ASP.NET.
    /// </returns>
    public string ClientID
    {
      get { return _page.ClientID; }
    }

    /// <summary>
    /// Gets or sets the skin to apply to the control.
    /// </summary>
    /// <returns>
    /// The name of the skin to apply to the control. The default is <see cref="F:System.String.Empty"/>.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The style sheet has already been applied.
    ///     - or -
    ///     The Page_PreInit event has already occurred.
    ///     - or -
    ///     The control was already added to the Controls collection.
    /// </exception>
    public string SkinID
    {
      get { return _page.SkinID; }
      set { _page.SkinID = value; }
    }

    /// <summary>
    /// Gets a reference to the server control's naming container, which creates a unique namespace 
    /// for differentiating between server controls with the same <see cref="P:System.Web.UI.Control.ID"/> property value.
    /// </summary>
    /// <returns>
    /// The server control's naming container.
    /// </returns>
    public Control NamingContainer
    {
      get { return _page.NamingContainer; }
    }

    /// <summary>
    /// Gets the control that contains this control's data binding.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.Control"/> that contains this control's data binding.
    /// </returns>
    public Control BindingContainer
    {
      get { return _page.BindingContainer; }
    }

    /// <summary>
    /// Gets a reference to the <see cref="T:System.Web.UI.Page"/> instance that contains the server control.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.Page"/> instance that contains the server control.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The control is a <see cref="T:System.Web.UI.WebControls.Substitution"/> control.
    /// </exception>
    public IPage Page
    {
      get { return this; }
    }

    /// <summary>
    /// Gets or sets a reference to the template that contains this control. 
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.TemplateControl"/> instance that contains this control. 
    /// </returns>
    public TemplateControl TemplateControl
    {
      get { return _page.TemplateControl; }
      set { _page.TemplateControl = value; }
    }

    /// <summary>
    /// Gets a reference to the server control's parent control in the page control hierarchy.
    /// </summary>
    /// <returns>
    /// A reference to the server control's parent control.
    /// </returns>
    public Control Parent
    {
      get { return _page.Parent; }
    }

    /// <summary>
    /// Gets the virtual directory of the <see cref="T:System.Web.UI.Page"/> or 
    /// <see cref="T:System.Web.UI.UserControl"/> that contains the current server control.
    /// </summary>
    /// <returns>
    /// The virtual directory of the page or user control that contains the server control.
    /// </returns>
    public string TemplateSourceDirectory
    {
      get { return _page.TemplateSourceDirectory; }
    }

    /// <summary>
    /// Gets or sets the application-relative virtual directory of the <see cref="T:System.Web.UI.Page"/> or 
    /// <see cref="T:System.Web.UI.UserControl"/> object that contains this control.
    /// </summary>
    /// <returns>
    /// The application-relative virtual directory of the page or user control that contains this control.
    /// </returns>
    public string AppRelativeTemplateSourceDirectory
    {
      get { return _page.AppRelativeTemplateSourceDirectory; }
      set { _page.AppRelativeTemplateSourceDirectory = value; }
    }

    /// <summary>
    /// Gets information about the container that hosts the current control when rendered on a design surface.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.ComponentModel.ISite"/> that contains information about the container that the control is hosted in.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The control is a <see cref="T:System.Web.UI.WebControls.Substitution"/> control.
    /// </exception>
    public ISite Site
    {
      get { return _page.Site; }
      set { _page.Site = value; }
    }

    /// <summary>
    /// Gets the unique, hierarchically qualified identifier for the server control.
    /// </summary>
    /// <returns>
    /// The fully qualified identifier for the server control.
    /// </returns>
    public string UniqueID
    {
      get { return _page.UniqueID; }
    }

    /// <summary>
    /// Gets a <see cref="T:System.Web.UI.ControlCollection"/> object that represents the child controls 
    /// for a specified server control in the UI hierarchy.
    /// </summary>
    /// <returns>
    /// The collection of child controls for the specified server control.
    /// </returns>
    public ControlCollection Controls
    {
      get { return _page.Controls; }
    }

    public event EventHandler Disposed
    {
      add { _page.Disposed += value; }
      remove { _page.Disposed -= value; }
    }

    public event EventHandler DataBinding
    {
      add { _page.DataBinding += value; }
      remove { _page.DataBinding -= value; }
    }

    public event EventHandler Init
    {
      add { _page.Init += value; }
      remove { _page.Init -= value; }
    }

    public event EventHandler Load
    {
      add { _page.Load += value; }
      remove { _page.Load -= value; }
    }

    public event EventHandler PreRender
    {
      add { _page.PreRender += value; }
      remove { _page.PreRender -= value; }
    }

    public event EventHandler Unload
    {
      add { _page.Unload += value; }
      remove { _page.Unload -= value; }
    }

    /// <summary>
    /// Returns a value indicating whether the specified filter is a type of the current filter object.
    /// </summary>
    /// <returns>
    /// true if the specified filter is a type applicable to the current filter object; otherwise, false.
    /// </returns>
    /// <param name="filterName">The name of a device filter.
    /// </param>
    public bool EvaluateFilter (string filterName)
    {
      return ((IFilterResolutionService) _page).EvaluateFilter (filterName);
    }

    /// <summary>
    /// Returns a value indicating whether a parent-child relationship exists between two specified device filters. 
    /// </summary>
    /// <returns>
    /// 1 if the device filter identified by <paramref name="filter1"/> is a parent of the filter identified by <paramref name="filter2"/>, 
    /// -1 if the device filter identified by <paramref name="filter2"/> is a parent of the filter identified by <paramref name="filter1"/>, 
    /// and 0 if there is no parent-child relationship between the two filters.
    /// </returns>
    /// <param name="filter1">A device filter name.
    /// </param><param name="filter2">A device filter name
    /// </param>
    public int CompareFilters (string filter1, string filter2)
    {
      return ((IFilterResolutionService) _page).CompareFilters (filter1, filter2);
    }

    /// <summary>
    /// Reads a string resource. The <see cref="M:System.Web.UI.TemplateControl.ReadStringResource"/> 
    /// method is not intended for use from within your code.
    /// </summary>
    /// <returns>
    /// An object representing the resource.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">The <see cref="M:System.Web.UI.TemplateControl.ReadStringResource"/> is no longer supported.
    /// </exception>
    public object ReadStringResource ()
    {
      return _page.ReadStringResource();
    }

    /// <summary>
    /// Returns a Boolean value indicating whether a device filter applies to the HTTP request.
    /// </summary>
    /// <returns>
    /// true if the client browser specified in <paramref name="filterName"/> is the same as the specified browser; otherwise, false. 
    /// The default is false.
    /// </returns>
    /// <param name="filterName">The browser name to test. 
    /// </param>
    public bool TestDeviceFilter (string filterName)
    {
      return _page.TestDeviceFilter (filterName);
    }

    /// <summary>
    /// Loads a <see cref="T:System.Web.UI.Control"/> object from a file based on a specified virtual path.
    /// </summary>
    /// <returns>
    /// Returns the specified <see cref="T:System.Web.UI.Control"/>.
    /// </returns>
    /// <param name="virtualPath">The virtual path to a control file. 
    /// </param><exception cref="T:System.ArgumentNullException">The virtual path is null or empty.
    /// </exception>
    public Control LoadControl (string virtualPath)
    {
      return _page.LoadControl (virtualPath);
    }

    /// <summary>
    /// Loads a <see cref="T:System.Web.UI.Control"/> object based on a specified type and constructor parameters.
    /// </summary>
    /// <returns>
    /// Returns the specified <see cref="T:System.Web.UI.UserControl"/>.
    /// </returns>
    /// <param name="t">The type of the control.
    /// </param><param name="parameters">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. 
    /// If <paramref name="parameters"/> is an empty array or null, the constructor that takes no parameters (the default constructor) is invoked.
    /// </param>
    public Control LoadControl (Type t, object[] parameters)
    {
      return _page.LoadControl (t, parameters);
    }

    /// <summary>
    /// Obtains an instance of the <see cref="T:System.Web.UI.ITemplate"/> interface from an external file.
    /// </summary>
    /// <returns>
    /// An instance of the specified template.
    /// </returns>
    /// <param name="virtualPath">The virtual path to a user control file. 
    /// </param>
    public ITemplate LoadTemplate (string virtualPath)
    {
      return _page.LoadTemplate (virtualPath);
    }

    /// <summary>
    /// Parses an input string into a <see cref="T:System.Web.UI.Control"/> object on the Web Forms page or user control.
    /// </summary>
    /// <returns>
    /// The parsed <see cref="T:System.Web.UI.Control"/>.
    /// </returns>
    /// <param name="content">A string that contains a user control. 
    /// </param>
    public Control ParseControl (string content)
    {
      return _page.ParseControl (content);
    }

    /// <summary>
    /// Parses an input string into a <see cref="T:System.Web.UI.Control"/> object on the ASP.NET Web page or user control.
    /// </summary>
    /// <returns>
    /// The parsed control.
    /// </returns>
    /// <param name="content">A string that contains a user control.
    /// </param><param name="ignoreParserFilter">A value that specifies whether to ignore the parser filter.
    /// </param>
    public Control ParseControl (string content, bool ignoreParserFilter)
    {
      return _page.ParseControl (content, ignoreParserFilter);
    }

    /// <summary>
    /// Gets or sets a Boolean value indicating whether themes apply to the control that is derived from the 
    /// <see cref="T:System.Web.UI.TemplateControl"/> class. 
    /// </summary>
    /// <returns>
    /// true to use themes; otherwise, false. The default is true.
    /// </returns>
    public bool EnableTheming
    {
      get { return _page.EnableTheming; }
      set { _page.EnableTheming = value; }
    }

    /// <summary>
    /// Gets or sets the application-relative, virtual directory path to the file from which the control is parsed and compiled. 
    /// </summary>
    /// <returns>
    /// A string representing the path.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">The path that is set is null. 
    /// </exception><exception cref="T:System.ArgumentException">The path that is set is not rooted. 
    /// </exception>
    public string AppRelativeVirtualPath
    {
      get { return _page.AppRelativeVirtualPath; }
      set { _page.AppRelativeVirtualPath = value; }
    }

    public event EventHandler CommitTransaction
    {
      add { _page.CommitTransaction += value; }
      remove { _page.CommitTransaction -= value; }
    }

    public event EventHandler AbortTransaction
    {
      add { _page.AbortTransaction += value; }
      remove { _page.AbortTransaction -= value; }
    }

    public event EventHandler Error
    {
      add { _page.Error += value; }
      remove { _page.Error -= value; }
    }

    /// <summary>
    /// Searches the page naming container for a server control with the specified identifier.
    /// </summary>
    /// <returns>
    /// The specified control, or null if the specified control does not exist.
    /// </returns>
    /// <param name="id">The identifier for the control to be found. 
    /// </param>
    public Control FindControl (string id)
    {
      return _page.FindControl (id);
    }

    /// <summary>
    /// Retrieves a hash code that is generated by <see cref="T:System.Web.UI.Page"/> objects that are generated at run time. 
    /// This hash code is unique to the <see cref="T:System.Web.UI.Page"/> object's control hierarchy.
    /// </summary>
    /// <returns>
    /// The hash code generated at run time. The default is 0.
    /// </returns>
    public int GetTypeHashCode ()
    {
      return _page.GetTypeHashCode();
    }

    /// <summary>
    /// Performs any initialization of the instance of the <see cref="T:System.Web.UI.Page"/> class that is required by RAD designers. 
    /// This method is used only at design time.
    /// </summary>
    public void DesignerInitialize ()
    {
      _page.DesignerInitialize();
    }

    /// <summary>
    /// Sets the browser focus to the specified control. 
    /// </summary>
    /// <param name="control">The control to receive focus.
    /// </param><exception cref="T:System.ArgumentNullException"><paramref name="control"/> is null. 
    /// </exception><exception cref="T:System.InvalidOperationException">
    /// <see cref="M:System.Web.UI.Page.SetFocus(System.Web.UI.Control)"/> is called when the control is not part of a Web Forms page. 
    ///     - or -
    /// <see cref="M:System.Web.UI.Page.SetFocus(System.Web.UI.Control)"/> is called after the <see cref="E:System.Web.UI.Control.PreRender"/> event. 
    /// </exception>
    public void SetFocus (Control control)
    {
      _page.SetFocus (control);
    }

    /// <summary>
    /// Sets the browser focus to the control with the specified identifier. 
    /// </summary>
    /// <param name="clientID">The ID of the control to set focus to.
    /// </param><exception cref="T:System.ArgumentNullException"><paramref name="clientID"/> is null.
    /// </exception><exception cref="T:System.InvalidOperationException">
    /// <see cref="M:System.Web.UI.Page.SetFocus(System.String)"/> is called when the control is not part of a Web Forms page.
    ///     - or -
    /// <see cref="M:System.Web.UI.Page.SetFocus(System.String)"/> is called after the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </exception>
    public void SetFocus (string clientID)
    {
      _page.SetFocus (clientID);
    }

    /// <summary>
    /// Returns a string that can be used in a client event to cause postback to the server. 
    /// The reference string is defined by the specified <see cref="T:System.Web.UI.Control"/> object.
    /// </summary>
    /// <returns>
    /// A string that, when treated as script on the client, initiates the postback.
    /// </returns>
    /// <param name="control">The server control to process the postback on the server. 
    /// </param>
    [Obsolete]
    public string GetPostBackEventReference (Control control)
    {
      return _page.GetPostBackEventReference (control);
    }

    /// <summary>
    /// Returns a string that can be used in a client event to cause postback to the server. 
    /// The reference string is defined by the specified control that handles the postback and a string argument of additional event information. 
    /// </summary>
    /// <returns>
    /// A string that, when treated as script on the client, initiates the postback.
    /// </returns>
    /// <param name="control">The server control to process the postback. 
    /// </param><param name="argument">The parameter passed to the server control. 
    /// </param>
    [Obsolete]
    public string GetPostBackEventReference (Control control, string argument)
    {
      return _page.GetPostBackEventReference (control, argument);
    }

    /// <summary>
    /// Gets a reference that can be used in a client event to post back to the server for the specified control 
    /// and with the specified event arguments.
    /// </summary>
    /// <returns>
    /// The string that represents the client event.
    /// </returns>
    /// <param name="control">The server control that receives the client event postback. 
    /// </param>
    /// <param name="argument">A <see cref="T:System.String"/> that is passed to 
    /// <see cref="M:System.Web.UI.IPostBackEventHandler.RaisePostBackEvent(System.String)"/>. 
    /// </param>
    [Obsolete]
    public string GetPostBackClientEvent (Control control, string argument)
    {
      return _page.GetPostBackClientEvent (control, argument);
    }

    /// <summary>
    /// Gets a reference, with javascript: appended to the beginning of it, that can be used in a client event to post back 
    /// to the server for the specified control and with the specified event arguments.
    /// </summary>
    /// <returns>
    /// A string representing a JavaScript call to the postback function that includes the target control's ID and event arguments.
    /// </returns>
    /// <param name="control">The server control to process the postback. 
    /// </param><param name="argument">The parameter passed to the server control. 
    /// </param>
    [Obsolete]
    public string GetPostBackClientHyperlink (Control control, string argument)
    {
      return _page.GetPostBackClientHyperlink (control, argument);
    }

    /// <summary>
    /// Determines whether the client script block with the specified key is registered with the page.
    /// </summary>
    /// <returns>
    /// true if the script block is registered; otherwise, false.
    /// </returns>
    /// <param name="key">The string key of the client script to search for. 
    /// </param>
    [Obsolete]
    public bool IsClientScriptBlockRegistered (string key)
    {
      return _page.IsClientScriptBlockRegistered (key);
    }

    /// <summary>
    /// Determines whether the client startup script is registered with the <see cref="T:System.Web.UI.Page"/> object.
    /// </summary>
    /// <returns>
    /// true if the startup script is registered; otherwise, false.
    /// </returns>
    /// <param name="key">The string key of the startup script to search for. 
    /// </param>
    [Obsolete]
    public bool IsStartupScriptRegistered (string key)
    {
      return _page.IsStartupScriptRegistered (key);
    }

    /// <summary>
    /// Declares a value that is declared as an ECMAScript array declaration when the page is rendered.
    /// </summary>
    /// <param name="arrayName">The name of the array in which to declare the value. 
    /// </param><param name="arrayValue">The value to place in the array. 
    /// </param>
    [Obsolete]
    public void RegisterArrayDeclaration (string arrayName, string arrayValue)
    {
      _page.RegisterArrayDeclaration (arrayName, arrayValue);
    }

    /// <summary>
    /// Allows server controls to automatically register a hidden field on the form. 
    /// The field will be sent to the <see cref="T:System.Web.UI.Page"/> object when 
    /// the <see cref="T:System.Web.UI.HtmlControls.HtmlForm"/> server control is rendered.
    /// </summary>
    /// <param name="hiddenFieldName">The unique name of the hidden field to be rendered. 
    /// </param><param name="hiddenFieldInitialValue">The value to be emitted in the hidden form. 
    /// </param>
    [Obsolete]
    public void RegisterHiddenField (string hiddenFieldName, string hiddenFieldInitialValue)
    {
      _page.RegisterHiddenField (hiddenFieldName, hiddenFieldInitialValue);
    }

    /// <summary>
    /// Emits client-side script blocks to the response.
    /// </summary>
    /// <param name="key">Unique key that identifies a script block. 
    /// </param><param name="script">Content of script that is sent to the client. 
    /// </param>
    [Obsolete]
    public void RegisterClientScriptBlock (string key, string script)
    {
      _page.RegisterClientScriptBlock (key, script);
    }

    /// <summary>
    /// Emits a client-side script block in the page response. 
    /// </summary>
    /// <param name="key">Unique key that identifies a script block. 
    /// </param><param name="script">Content of script that will be sent to the client. 
    /// </param>
    [Obsolete]
    public void RegisterStartupScript (string key, string script)
    {
      _page.RegisterStartupScript (key, script);
    }

    /// <summary>
    /// Allows a page to access the client OnSubmit event. The script should be a function call to client code registered elsewhere.
    /// </summary>
    /// <param name="key">Unique key that identifies a script block. 
    /// </param><param name="script">The client-side script to be sent to the client. 
    /// </param>
    [Obsolete]
    public void RegisterOnSubmitStatement (string key, string script)
    {
      _page.RegisterOnSubmitStatement (key, script);
    }

    /// <summary>
    /// Registers a control as one whose control state must be persisted.
    /// </summary>
    /// <param name="control">The control to register.
    /// </param><exception cref="T:System.ArgumentException">The control to register is null.
    /// </exception><exception cref="T:System.InvalidOperationException">
    /// The <see cref="M:System.Web.UI.Page.RegisterRequiresControlState(System.Web.UI.Control)"/> 
    /// method can be called only before or during the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </exception>
    public void RegisterRequiresControlState (Control control)
    {
      _page.RegisterRequiresControlState (control);
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Web.UI.Control"/> object is registered to participate in control state management.
    /// </summary>
    /// <returns>
    /// true if the specified <see cref="T:System.Web.UI.Control"/> requires control state; otherwise, false
    /// </returns>
    /// <param name="control">The <see cref="T:System.Web.UI.Control"/> to check for a control state requirement.
    /// </param>
    public bool RequiresControlState (Control control)
    {
      return _page.RequiresControlState (control);
    }

    /// <summary>
    /// Stops persistence of control state for the specified control.
    /// </summary>
    /// <param name="control">The <see cref="T:System.Web.UI.Control"/> for which to stop persistence of control state.
    /// </param><exception cref="T:System.ArgumentException">The <see cref="T:System.Web.UI.Control"/> is null.
    /// </exception>
    public void UnregisterRequiresControlState (Control control)
    {
      _page.UnregisterRequiresControlState (control);
    }

    /// <summary>
    /// Registers a control as one that requires postback handling when the page is posted back to the server. 
    /// </summary>
    /// <param name="control">The control to be registered. 
    /// </param><exception cref="T:System.Web.HttpException">The control to register does not implement the 
    /// <see cref="T:System.Web.UI.IPostBackDataHandler"/> interface. 
    /// </exception>
    public void RegisterRequiresPostBack (Control control)
    {
      _page.RegisterRequiresPostBack (control);
    }

    /// <summary>
    /// Registers an ASP.NET server control as one requiring an event to be raised when the control is processed on the 
    /// <see cref="T:System.Web.UI.Page"/> object.
    /// </summary>
    /// <param name="control">The control to register. 
    /// </param>
    public void RegisterRequiresRaiseEvent (IPostBackEventHandler control)
    {
      _page.RegisterRequiresRaiseEvent (control);
    }

    /// <summary>
    /// Retrieves the physical path that a virtual path, either absolute or relative, or an application-relative path maps to.
    /// </summary>
    /// <returns>
    /// The physical path associated with the virtual path or application-relative path.
    /// </returns>
    /// <param name="virtualPath">A <see cref="T:System.String"/> that represents the virtual path. 
    /// </param>
    public string MapPath (string virtualPath)
    {
      return _page.MapPath (virtualPath);
    }

    /// <summary>
    /// Registers a control with the page as one requiring view-state encryption. 
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="M:System.Web.UI.Page.RegisterRequiresViewStateEncryption"/> method must be called before or during 
    /// the page PreRenderphase in the page life cycle. 
    /// </exception>
    public void RegisterRequiresViewStateEncryption ()
    {
      _page.RegisterRequiresViewStateEncryption();
    }

    /// <summary>
    /// Sets the intrinsics of the <see cref="T:System.Web.UI.Page"/>, such as the <see cref="P:System.Web.UI.Page.Context"/>, 
    /// <see cref="P:System.Web.UI.Page.Request"/>, <see cref="P:System.Web.UI.Page.Response"/>, 
    /// and <see cref="P:System.Web.UI.Page.Application"/> properties.
    /// </summary>
    /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects 
    /// (for example, <see cref="P:System.Web.HttpContext.Request"/>, <see cref="P:System.Web.HttpContext.Response"/>, 
    /// and <see cref="P:System.Web.HttpContext.Session"/>) used to service HTTP requests. 
    /// </param>
    public void ProcessRequest (HttpContext context)
    {
      _page.ProcessRequest (context);
    }

    /// <summary>
    /// Causes page view state to be persisted, if called.
    /// </summary>
    public void RegisterViewStateHandler ()
    {
      _page.RegisterViewStateHandler();
    }

    /// <summary>
    /// Starts the execution of an asynchronous task.
    /// </summary>
    /// <exception cref="T:System.Web.HttpException">There is an exception in the asynchronous task.
    /// </exception>
    public void ExecuteRegisteredAsyncTasks ()
    {
      _page.ExecuteRegisteredAsyncTasks();
    }

    /// <summary>
    /// Registers a new asynchronous task with the page.
    /// </summary>
    /// <param name="task">A <see cref="T:System.Web.UI.PageAsyncTask"/> that defines the asynchronous task.
    /// </param><exception cref="T:System.ArgumentNullException">The asynchronous task is null. 
    /// </exception>
    public void RegisterAsyncTask (PageAsyncTask task)
    {
      _page.RegisterAsyncTask (task);
    }

    /// <summary>
    /// Registers beginning and ending event handler delegates that do not require state information for an asynchronous page.
    /// </summary>
    /// <param name="beginHandler">The delegate for the <see cref="T:System.Web.BeginEventHandler"/> method.
    /// </param>
    /// <param name="endHandler">The delegate for the <see cref="T:System.Web.EndEventHandler"/> method.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// The &lt;async&gt; page directive is not set to true.
    ///     - or -
    ///     The <see cref="M:System.Web.UI.Page.AddOnPreRenderCompleteAsync(System.Web.BeginEventHandler,System.Web.EndEventHandler)"/> 
    ///     method is called after the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <see cref="P:System.Web.UI.PageAsyncTask.BeginHandler"/> or <see cref="P:System.Web.UI.PageAsyncTask.EndHandler"/> is null. 
    /// </exception>
    public void AddOnPreRenderCompleteAsync (BeginEventHandler beginHandler, EndEventHandler endHandler)
    {
      _page.AddOnPreRenderCompleteAsync (beginHandler, endHandler);
    }

    /// <summary>
    /// Registers beginning and ending  event handler delegates for an asynchronous page.
    /// </summary>
    /// <param name="beginHandler">The delegate for the <see cref="T:System.Web.BeginEventHandler"/> method.
    /// </param>
    /// <param name="endHandler">The delegate for the <see cref="T:System.Web.EndEventHandler"/> method.
    /// </param>
    /// <param name="state">An object containing state information for the event handlers.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    ///   The &lt;async&gt; page directive is not set to true.
    ///     - or -
    ///     The <see cref="M:System.Web.UI.Page.AddOnPreRenderCompleteAsync(System.Web.BeginEventHandler,System.Web.EndEventHandler)"/> 
    ///     method is called after the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    ///   The <see cref="P:System.Web.UI.PageAsyncTask.BeginHandler"/> or <see cref="P:System.Web.UI.PageAsyncTask.EndHandler"/> is null. 
    /// </exception>
    public void AddOnPreRenderCompleteAsync (BeginEventHandler beginHandler, EndEventHandler endHandler, object state)
    {
      _page.AddOnPreRenderCompleteAsync (beginHandler, endHandler, state);
    }

    /// <summary>
    /// Instructs any validation controls included on the page to validate their assigned information.
    /// </summary>
    public void Validate ()
    {
      _page.Validate();
    }

    /// <summary>
    /// Instructs the validation controls in the specified validation group to validate their assigned information.
    /// </summary>
    /// <param name="validationGroup">The validation group name of the controls to validate.
    /// </param>
    public void Validate (string validationGroup)
    {
      _page.Validate (validationGroup);
    }

    /// <summary>
    /// Returns a collection of control validators for a specified validation group.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Web.UI.ValidatorCollection"/> that contains the control validators for the specified validation group.
    /// </returns>
    /// <param name="validationGroup">The validation group to return, or null to return the default validation group.
    /// </param>
    public ValidatorCollection GetValidators (string validationGroup)
    {
      return _page.GetValidators (validationGroup);
    }

    /// <summary>
    /// Confirms that an <see cref="T:System.Web.UI.HtmlControls.HtmlForm"/> control is rendered for the specified ASP.NET server control at run time.
    /// </summary>
    /// <param name="control">The ASP.NET server control that is required in the <see cref="T:System.Web.UI.HtmlControls.HtmlForm"/> control. 
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// The specified server control is not contained between the opening and closing tags 
    /// of the <see cref="T:System.Web.UI.HtmlControls.HtmlForm"/> server control at run time. 
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// The control to verify is null.
    /// </exception>
    public void VerifyRenderingInServerForm (Control control)
    {
      _page.VerifyRenderingInServerForm (control);
    }

    /// <summary>
    /// Gets the data item at the top of the data-binding context stack.
    /// </summary>
    /// <returns>
    /// The object at the top of the data binding context stack.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">There is no data-binding context for the page.
    /// </exception>
    public object GetDataItem ()
    {
      return _page.GetDataItem();
    }

    /// <summary>
    /// Gets the <see cref="T:System.Web.HttpApplicationState"/> object for the current Web request.
    /// </summary>
    /// <returns>
    /// The current data in the <see cref="T:System.Web.HttpApplicationState"/> class.
    /// </returns>
    public HttpApplicationState Application
    {
      get { return _page.Application; }
    }

    /// <summary>
    /// Gets a <see cref="T:System.Web.UI.ClientScriptManager"/> object used to manage, register, and add script to the page.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Web.UI.ClientScriptManager"/> object.
    /// </returns>
    public IClientScriptManager ClientScript
    {
      get { return new ClientScriptManagerWrapper (_page.ClientScript); }
    }

    /// <summary>
    /// Gets or sets a value that allows you to override automatic detection of browser capabilities 
    /// and to specify how a page is rendered for particular browser clients.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that specifies the browser capabilities that you want to override.
    /// </returns>
    public string ClientTarget
    {
      get { return _page.ClientTarget; }
      set { _page.ClientTarget = value; }
    }

    /// <summary>
    /// Gets the query string portion of the requested URL.
    /// </summary>
    /// <returns>
    /// The query string portion of the requested URL.
    /// </returns>
    public string ClientQueryString
    {
      get { return _page.ClientQueryString; }
    }

    /// <summary>
    /// Gets or sets the error page to which the requesting browser is redirected in the event of an unhandled page exception.
    /// </summary>
    /// <returns>
    /// The error page to which the browser is redirected.
    /// </returns>
    public string ErrorPage
    {
      get { return _page.ErrorPage; }
      set { _page.ErrorPage = value; }
    }

    /// <summary>
    /// Gets a value indicating whether the page request is the result of a call back.
    /// </summary>
    /// <returns>
    /// true if the page request is the result of a call back; otherwise, false.
    /// </returns>
    public bool IsCallback
    {
      get { return _page.IsCallback; }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Web.UI.Page"/> object can be reused.
    /// </summary>
    /// <returns>
    /// false in all cases. 
    /// </returns>
    public bool IsReusable
    {
      get { return _page.IsReusable; }
    }

    /// <summary>
    /// Gets or sets the control of the page that is used to postback to the server.
    /// </summary>
    /// <returns>
    /// The control that is used to postback to the server.
    /// </returns>
    public Control AutoPostBackControl
    {
      get { return _page.AutoPostBackControl; }
      set { _page.AutoPostBackControl = value; }
    }

    /// <summary>
    /// Gets the document header for the page if the head element is defined with a runat=server in the page declaration.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.UI.HtmlControls.HtmlHead"/> containing the page header.
    /// </returns>
    public HtmlHead Header
    {
      get { return _page.Header; }
    }

    /// <summary>
    /// Gets the character used to separate control identifiers when building a unique ID for a control on a page.
    /// </summary>
    /// <returns>
    /// The character used to separate control identifiers. 
    /// The default is set by the <see cref="T:System.Web.UI.Adapters.PageAdapter"/> instance that renders the page. 
    /// The <see cref="P:System.Web.UI.Page.IdSeparator"/> is a server-side field and should not be modified.
    /// </returns>
    public char IdSeparator
    {
      get { return _page.IdSeparator; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to return the user to the same position in the client browser after postback. 
    /// This property replaces the obsolete <see cref="P:System.Web.UI.Page.SmartNavigation"/> property.
    /// </summary>
    /// <returns>
    /// true if the client position should be maintained; otherwise, false.
    /// </returns>
    public bool MaintainScrollPositionOnPostBack
    {
      get { return _page.MaintainScrollPositionOnPostBack; }
      set { _page.MaintainScrollPositionOnPostBack = value; }
    }

    /// <summary>
    /// Gets the master page that determines the overall look of the page.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.MasterPage"/> associated with this page if it exists; otherwise, null. 
    /// </returns>
    public MasterPage Master
    {
      get { return _page.Master; }
    }

    /// <summary>
    /// Gets or sets the file name of the master page.
    /// </summary>
    /// <returns>
    /// The file name of the master page.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    ///   The <see cref="P:System.Web.UI.Page.MasterPageFile"/> property is set after the 
    ///   <see cref="E:System.Web.UI.Page.PreInit"/> event is complete.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    ///   The file specified in the <see cref="P:System.Web.UI.Page.MasterPageFile"/> property does not exist.
    ///     - or -
    ///     The page does not have a <see cref="T:System.Web.UI.WebControls.Content"/> control as the top level control.
    /// </exception>
    public string MasterPageFile
    {
      get { return _page.MasterPageFile; }
      set { _page.MasterPageFile = value; }
    }

    /// <summary>
    /// Gets or sets the maximum length for the page's state field.
    /// </summary>
    /// <returns>
    /// The maximum length, in bytes, for the page's state field. The default is -1.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    ///   The <see cref="P:System.Web.UI.Page.MaxPageStateFieldLength"/> property  is not equal to -1 or a positive number.
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    ///   The <see cref="P:System.Web.UI.Page.MaxPageStateFieldLength"/> property was set after the page was initialized.
    /// </exception>
    public int MaxPageStateFieldLength
    {
      get { return _page.MaxPageStateFieldLength; }
      set { _page.MaxPageStateFieldLength = value; }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Web.TraceContext"/> object for the current Web request.
    /// </summary>
    /// <returns>
    /// Data from the <see cref="T:System.Web.TraceContext"/> object for the current Web request.
    /// </returns>
    public TraceContext Trace
    {
      get { return _page.Trace; }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Web.HttpRequest"/> object for the requested page.
    /// </summary>
    /// <returns>
    /// The current <see cref="T:System.Web.HttpRequest"/> associated with the page.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">Occurs when the <see cref="T:System.Web.HttpRequest"/> object is not available. 
    /// </exception>
    public HttpRequest Request
    {
      get { return _page.Request; }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Web.HttpResponse"/> object associated with the <see cref="T:System.Web.UI.Page"/> object. 
    /// This object allows you to send HTTP response data to a client and contains information about that response.
    /// </summary>
    /// <returns>
    /// The current <see cref="T:System.Web.HttpResponse"/> associated with the page.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">The <see cref="T:System.Web.HttpResponse"/> object is not available. 
    /// </exception>
    public HttpResponse Response
    {
      get { return _page.Response; }
    }

    /// <summary>
    /// Gets the Server object, which is an instance of the <see cref="T:System.Web.HttpServerUtility"/> class.
    /// </summary>
    /// <returns>
    /// The current Server object associated with the page.
    /// </returns>
    public HttpServerUtility Server
    {
      get { return _page.Server; }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Web.Caching.Cache"/> object associated with the application in which the page resides.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.Caching.Cache"/> associated with the page's application.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">An instance of <see cref="T:System.Web.Caching.Cache"/> is not created. 
    /// </exception>
    public Cache Cache
    {
      get { return _page.Cache; }
    }

    /// <summary>
    /// Gets the current Session object provided by ASP.NET.
    /// </summary>
    /// <returns>
    /// The current session-state data.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">Occurs when the session information is set to null. 
    /// </exception>
    public HttpSessionState Session
    {
      get { return _page.Session; }
    }

    /// <summary>
    /// Gets or sets the title for the page.
    /// </summary>
    /// <returns>
    /// The title of the page.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="P:System.Web.UI.Page.Title"/> property requires a header control on the page.
    /// </exception>
    public string Title
    {
      get { return _page.Title; }
      set { _page.Title = value; }
    }

    /// <summary>
    /// Gets or sets the name of the page theme.
    /// </summary>
    /// <returns>
    /// The name of the page theme.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    ///   An attempt was made to set <see cref="P:System.Web.UI.Page.Theme"/> after the <see cref="E:System.Web.UI.Page.PreInit"/> event has occurred.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    ///   <see cref="P:System.Web.UI.Page.Theme"/> is set to an invalid theme name.
    /// </exception>
    public string Theme
    {
      get { return _page.Theme; }
      set { _page.Theme = value; }
    }

    /// <summary>
    /// Gets or sets the name of the style sheet applied to this page.
    /// </summary>
    /// <returns>
    /// The name of the style sheet applied to this page.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="P:System.Web.UI.Page.StyleSheetTheme"/> property is set before the <see cref="E:System.Web.UI.Control.Init"/> event completes.
    /// </exception>
    public string StyleSheetTheme
    {
      get { return _page.StyleSheetTheme; }
      set { _page.StyleSheetTheme = value; }
    }

    /// <summary>
    /// Gets information about the user making the page request.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Security.Principal.IPrincipal"/> that represents the user making the page request.
    /// </returns>
    public IPrincipal User
    {
      get { return _page.User; }
    }

    /// <summary>
    /// Gets a value indicating whether the page is involved in a cross-page postback.
    /// </summary>
    /// <returns>
    /// true if the page is participating in a cross-page request; otherwise, false.
    /// </returns>
    public bool IsCrossPagePostBack
    {
      get { return _page.IsCrossPagePostBack; }
    }

    /// <summary>
    /// Gets a value indicating whether the page is being loaded in response to a client postback, 
    /// or if it is being loaded and accessed for the first time.
    /// </summary>
    /// <returns>
    /// true if the page is being loaded in response to a client postback; otherwise, false.
    /// </returns>
    public bool IsPostBack
    {
      get { return _page.IsPostBack; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the page validates postback and callback events.
    /// </summary>
    /// <returns>
    /// true if the page validates events; otherwise, false.The default is true.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="P:System.Web.UI.Page.EnableEventValidation"/> property was set after the page was initialized.
    /// </exception>
    public bool EnableEventValidation
    {
      get { return _page.EnableEventValidation; }
      set { _page.EnableEventValidation = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the page maintains its view state, 
    /// and the view state of any server controls it contains, when the current page request ends.
    /// </summary>
    /// <returns>
    /// true if the page maintains its view state; otherwise, false. The default is true.
    /// </returns>
    public bool EnableViewState
    {
      get { return _page.EnableViewState; }
      set { _page.EnableViewState = value; }
    }

    /// <summary>
    /// Gets or sets the encryption mode of the view state.
    /// </summary>
    /// <returns>
    /// One of the <see cref="T:System.Web.UI.ViewStateEncryptionMode"/> values. 
    /// The default value is <see cref="F:System.Web.UI.ViewStateEncryptionMode.Auto"/>. 
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// The value set is not a member of the <see cref="T:System.Web.UI.ViewStateEncryptionMode"/> enumeration.
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="P:System.Web.UI.Page.ViewStateEncryptionMode"/> property can be set only in or before 
    /// the page PreRenderphase in the page life cycle.
    /// </exception>
    public ViewStateEncryptionMode ViewStateEncryptionMode
    {
      get { return _page.ViewStateEncryptionMode; }
      set { _page.ViewStateEncryptionMode = value; }
    }

    /// <summary>
    /// Assigns an identifier to an individual user in the view-state variable associated with the current page.
    /// </summary>
    /// <returns>
    /// The identifier for the individual user.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// The <see cref="P:System.Web.UI.Page.ViewStateUserKey"/> property was accessed too late during page processing. 
    /// </exception>
    public string ViewStateUserKey
    {
      get { return _page.ViewStateUserKey; }
      set { _page.ViewStateUserKey = value; }
    }

    /// <summary>
    /// Gets or sets an identifier for a particular instance of the <see cref="T:System.Web.UI.Page"/> class.
    /// </summary>
    /// <returns>
    /// The identifier for the instance of the <see cref="T:System.Web.UI.Page"/> class. The default value is '_Page'.
    /// </returns>
    public string ID
    {
      get { return _page.ID; }
      set { _page.ID = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="T:System.Web.UI.Page"/> object is rendered.
    /// </summary>
    /// <returns>
    /// true if the <see cref="T:System.Web.UI.Page"/> is to be rendered; otherwise, false. The default is true.
    /// </returns>
    public bool Visible
    {
      get { return _page.Visible; }
      set { _page.Visible = value; }
    }

    /// <summary>
    /// Gets a value indicating whether the control of the page that submits a postback is registered.
    /// </summary>
    /// <returns>
    /// true if the control is registered; otherwise, false.
    /// </returns>
    public bool IsPostBackEventControlRegistered
    {
      get { return _page.IsPostBackEventControlRegistered; }
    }

    /// <summary>
    /// Gets a value indicating whether page validation succeeded.
    /// </summary>
    /// <returns>
    /// true if page validation succeeded; otherwise, false.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">The <see cref="P:System.Web.UI.Page.IsValid"/> property is called before validation has occurred.
    /// </exception>
    public bool IsValid
    {
      get { return _page.IsValid; }
    }

    /// <summary>
    /// Gets a collection of all validation controls contained on the requested page.
    /// </summary>
    /// <returns>
    /// The collection of validation controls.
    /// </returns>
    public ValidatorCollection Validators
    {
      get { return _page.Validators; }
    }

    /// <summary>
    /// Gets the page that transferred control to the current page.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.Page"/> representing the page that transferred control to the current page.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The current user is not allowed to access the previous page.
    /// </exception>
    public Page PreviousPage
    {
      get { return _page.PreviousPage; }
    }

    /// <summary>
    /// Sets a value indicating whether the page output is buffered.
    /// </summary>
    /// <returns>
    /// true if page output is buffered; otherwise, false. The default is true.
    /// </returns>
    public bool Buffer
    {
      get { return _page.Buffer; }
      set { _page.Buffer = value; }
    }

    /// <summary>
    /// Sets the HTTP MIME type for the <see cref="T:System.Web.HttpResponse"/> object associated with the page.
    /// </summary>
    /// <returns>
    /// The HTTP MIME type associated with the current page.
    /// </returns>
    public string ContentType
    {
      get { return _page.ContentType; }
      set { _page.ContentType = value; }
    }

    /// <summary>
    /// Sets the code page identifier for the current <see cref="T:System.Web.UI.Page"/>.
    /// </summary>
    /// <returns>
    /// An integer that represents the code page identifier for the current <see cref="T:System.Web.UI.Page"/>.
    /// </returns>
    public int CodePage
    {
      get { return _page.CodePage; }
      set { _page.CodePage = value; }
    }

    /// <summary>
    /// Sets the encoding language for the current <see cref="T:System.Web.HttpResponse"/> object.
    /// </summary>
    /// <returns>
    /// A string that contains the encoding language for the current <see cref="T:System.Web.HttpResponse"/>.
    /// </returns>
    public string ResponseEncoding
    {
      get { return _page.ResponseEncoding; }
      set { _page.ResponseEncoding = value; }
    }

    /// <summary>
    /// Sets the culture ID for the <see cref="T:System.Threading.Thread"/> object associated with the page.
    /// </summary>
    /// <returns>
    /// A valid culture ID.
    /// </returns>
    public string Culture
    {
      get { return _page.Culture; }
      set { _page.Culture = value; }
    }

    /// <summary>
    /// Sets the locale identifier for the <see cref="T:System.Threading.Thread"/> object associated with the page.
    /// </summary>
    /// <returns>
    /// The locale identifier to pass to the <see cref="T:System.Threading.Thread"/>.
    /// </returns>
    public int LCID
    {
      get { return _page.LCID; }
      set { _page.LCID = value; }
    }

    /// <summary>
    /// Sets the user interface (UI) ID for the <see cref="T:System.Threading.Thread"/> object associated with the page.
    /// </summary>
    /// <returns>
    /// The UI ID.
    /// </returns>
    public string UICulture
    {
      get { return _page.UICulture; }
      set { _page.UICulture = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating the time-out interval used when processing asynchronous tasks.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.TimeSpan"/> that contains the allowed time interval for completion of the asynchronous task. 
    /// The default time interval is 45 seconds.
    /// </returns>
    public TimeSpan AsyncTimeout
    {
      get { return _page.AsyncTimeout; }
      set { _page.AsyncTimeout = value; }
    }

    /// <summary>
    /// Sets a value indicating whether tracing is enabled for the <see cref="T:System.Web.UI.Page"/> object.
    /// </summary>
    /// <returns>
    /// true if tracing is enabled for the page; otherwise, false. The default is false.
    /// </returns>
    public bool TraceEnabled
    {
      get { return _page.TraceEnabled; }
      set { _page.TraceEnabled = value; }
    }

    /// <summary>
    /// Sets the mode in which trace statements are displayed on the page.
    /// </summary>
    /// <returns>
    /// One of the <see cref="T:System.Web.TraceMode"/> enumeration members.
    /// </returns>
    public TraceMode TraceModeValue
    {
      get { return _page.TraceModeValue; }
      set { _page.TraceModeValue = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ASP.NET should run a message authentication check (MAC) on the page's view state 
    /// when the page is posted back from the client.
    /// </summary>
    /// <returns>
    /// true if the view state should be MAC checked and encoded; otherwise, false. The default is false.
    /// </returns>
    public bool EnableViewStateMac
    {
      get { return _page.EnableViewStateMac; }
      set { _page.EnableViewStateMac = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether smart navigation is enabled. This property is deprecated.
    /// </summary>
    /// <returns>
    /// true if smart navigation is enabled; otherwise, false.
    /// </returns>
    [Obsolete]
    public bool SmartNavigation
    {
      get { return _page.SmartNavigation; }
      set { _page.SmartNavigation = value; }
    }

    /// <summary>
    /// Gets a value indicating whether the page is processed asynchronously.
    /// </summary>
    /// <returns>
    /// true if the page is in asynchronous mode; otherwise, false;
    /// </returns>
    public bool IsAsync
    {
      get { return _page.IsAsync; }
    }

    /// <summary>
    /// Gets the HTML form for the page.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.HtmlControls.HtmlForm"/> object associated with the page.
    /// </returns>
    public HtmlForm Form
    {
      get { return _page.Form; }
    }

    /// <summary>
    /// Gets the adapter that renders the page for the specific requesting browser.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.Adapters.PageAdapter"/> that renders the page.
    /// </returns>
    public PageAdapter PageAdapter
    {
      get { return _page.PageAdapter; }
    }

    /// <summary>
    /// Gets a list of objects stored in the page context.
    /// </summary>
    /// <returns>
    /// A reference to an <see cref="T:System.Collections.IDictionary"/> containing objects stored in the page context.
    /// </returns>
    public IDictionary Items
    {
      get { return _page.Items; }
    }

    public event EventHandler LoadComplete
    {
      add { _page.LoadComplete += value; }
      remove { _page.LoadComplete -= value; }
    }

    public event EventHandler PreInit
    {
      add { _page.PreInit += value; }
      remove { _page.PreInit -= value; }
    }

    public event EventHandler PreLoad
    {
      add { _page.PreLoad += value; }
      remove { _page.PreLoad -= value; }
    }

    public event EventHandler PreRenderComplete
    {
      add { _page.PreRenderComplete += value; }
      remove { _page.PreRenderComplete -= value; }
    }

    public event EventHandler InitComplete
    {
      add { _page.InitComplete += value; }
      remove { _page.InitComplete -= value; }
    }

    public event EventHandler SaveStateComplete
    {
      add { _page.SaveStateComplete += value; }
      remove { _page.SaveStateComplete -= value; }
    }
  }
}