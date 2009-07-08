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
  public interface IControl
      : IComponent,
        IParserAccessor,
        IUrlResolutionService,
        IDataBindingsAccessor,
        IControlBuilderAccessor,
        IControlDesignerAccessor,
        IExpressionsAccessor
  {
    /// <summary>
    /// Applies the style properties defined in the page style sheet to the control.
    /// </summary>
    /// <param name="page">The <see cref="T:System.Web.UI.Page"/> containing the control.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">The style sheet is already applied.
    /// </exception>
    void ApplyStyleSheetSkin (Page page);

    /// <summary>
    /// Binds a data source to the invoked server control and all its child controls.
    /// </summary>
    void DataBind ();

    /// <summary>
    /// Sets input focus to a control.
    /// </summary>
    void Focus ();

    /// <summary>
    /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object and stores tracing information about the control if tracing is enabled.
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Web.UI.HTmlTextWriter"/> object that receives the control content. 
    /// </param>
    void RenderControl (HtmlTextWriter writer);

    /// <summary>
    /// Converts a URL into one that is usable on the requesting client.
    /// </summary>
    /// <returns>
    /// The converted URL.
    /// </returns>
    /// <param name="relativeUrl">The URL associated with the <see cref="P:System.Web.UI.Control.TemplateSourceDirectory"/> property. 
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">Occurs if the <paramref name="relativeUrl"/> parameter contains null. 
    /// </exception>
    string ResolveUrl (string relativeUrl);

    /// <summary>
    /// Searches the current naming container for a server control with the specified <paramref name="id"/> parameter.
    /// </summary>
    /// <returns>
    /// The specified control, or null if the specified control does not exist.
    /// </returns>
    /// <param name="id">The identifier for the control to be found. 
    /// </param>
    Control FindControl (string id);

    /// <summary>
    /// Determines if the server control contains any child controls.
    /// </summary>
    /// <returns>
    /// true if the control contains other controls; otherwise, false.
    /// </returns>
    bool HasControls ();

    /// <summary>
    /// Assigns an event handler delegate to render the server control and its content into its parent control.
    /// </summary>
    /// <param name="renderMethod">The information necessary to pass to the delegate so that it can render the server control. 
    /// </param>
    void SetRenderMethodDelegate (RenderMethod renderMethod);

    /// <summary>
    /// Gets the server control identifier generated by ASP.NET.
    /// </summary>
    /// <returns>
    /// The server control identifier generated by ASP.NET.
    /// </returns>
    string ClientID { get; }

    /// <summary>
    /// Gets or sets the programmatic identifier assigned to the server control.
    /// </summary>
    /// <returns>
    /// The programmatic identifier assigned to the control.
    /// </returns>
    string ID { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether themes apply to this control.
    /// </summary>
    /// <returns>
    /// true to use themes; otherwise, false. The default is true. 
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The Page_PreInit event has already occurred.
    ///     <para>- or -</para>
    ///     The control has already been added to the Controls collection.
    /// </exception>
    bool EnableTheming { get; set; }

    /// <summary>
    /// Gets or sets the skin to apply to the control.
    /// </summary>
    /// <returns>
    /// The name of the skin to apply to the control. The default is <see cref="F:System.String.Empty"/>.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The style sheet has already been applied.
    ///     <para>- or -</para>
    ///     The Page_PreInit event has already occurred.
    ///     <para>- or -</para>
    ///     The control was already added to the Controls collection.
    /// </exception>
    string SkinID { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the server control persists its view state, and the view state of any child controls it contains, to the requesting client.
    /// </summary>
    /// <returns>
    /// true if the server control maintains its view state; otherwise false. The default is true.
    /// </returns>
    bool EnableViewState { get; set; }

    /// <summary>
    /// Gets a reference to the server control's naming container, which creates a unique namespace for differentiating between server controls with the same <see cref="P:System.Web.UI.Control.ID"/> property value.
    /// </summary>
    /// <returns>
    /// The server control's naming container.
    /// </returns>
    Control NamingContainer { get; }

    /// <summary>
    /// Gets the control that contains this control's data binding.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.Control"/> that contains this control's data binding.
    /// </returns>
    Control BindingContainer { get; }

    /// <summary>
    /// Gets a reference to the <see cref="T:System.Web.UI.Page"/> instance that contains the server control.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.Page"/> instance that contains the server control.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The control is a <see cref="T:System.Web.UI.WebControls.Substitution"/> control.
    /// </exception>
    IPage Page { get; }

    /// <summary>
    /// Gets or sets a reference to the template that contains this control. 
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.TemplateControl"/> instance that contains this control. 
    /// </returns>
    TemplateControl TemplateControl { get; set; }

    /// <summary>
    /// Gets a reference to the server control's parent control in the page control hierarchy.
    /// </summary>
    /// <returns>
    /// A reference to the server control's parent control.
    /// </returns>
    Control Parent { get; }

    /// <summary>
    /// Gets the virtual directory of the <see cref="T:System.Web.UI.Page"/> or <see cref="T:System.Web.UI.UserControl"/> that contains the current server control.
    /// </summary>
    /// <returns>
    /// The virtual directory of the page or user control that contains the server control.
    /// </returns>
    string TemplateSourceDirectory { get; }

    /// <summary>
    /// Gets or sets the application-relative virtual directory of the <see cref="T:System.Web.UI.Page"/> or <see cref="T:System.Web.UI.UserControl"/> object that contains this control.
    /// </summary>
    /// <returns>
    /// The application-relative virtual directory of the page or user control that contains this control.
    /// </returns>
    string AppRelativeTemplateSourceDirectory { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether a server control is rendered as UI on the page.
    /// </summary>
    /// <returns>
    /// true if the control is visible on the page; otherwise false.
    /// </returns>
    bool Visible { get; set; }

    /// <summary>
    /// Gets the unique, hierarchically qualified identifier for the server control.
    /// </summary>
    /// <returns>
    /// The fully qualified identifier for the server control.
    /// </returns>
    string UniqueID { get; }

    /// <summary>
    /// Gets a <see cref="T:System.Web.UI.ControlCollection"/> object that represents the child controls for a specified server control in the UI hierarchy.
    /// </summary>
    /// <returns>
    /// The collection of child controls for the specified server control.
    /// </returns>
    ControlCollection Controls { get; }

    event EventHandler DataBinding;
    event EventHandler Init;
    event EventHandler Load;
    event EventHandler PreRender;
    event EventHandler Unload;
  }
}