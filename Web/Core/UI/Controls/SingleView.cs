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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.UI.Design;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  [ToolboxData ("<{0}:SingleView id=\"SingleView\" runat=\"server\">\r\n\t<View>\r\n\t</View>\r\n</{0}:SingleView>")]
  //[Designer( typeof (SingleViewDesigner))]
  public class SingleView : WebControl, IControl
  {
    private PlaceHolder _view;
    //private Control _viewTemplateContainer;
    //private ITemplate _viewTemplate;
    private PlaceHolder _topControl;
    private PlaceHolder _bottomControl;

    private Style _viewStyle;
    private Style _topControlsStyle;
    private Style _bottomControlsStyle;

    public SingleView ()
    {
      CreateControls();
      _viewStyle = new Style();
      _topControlsStyle = new Style();
      _bottomControlsStyle = new Style();
    }

    private void CreateControls ()
    {
      _view = new PlaceHolder ();
      _topControl = new PlaceHolder();
      _bottomControl = new PlaceHolder();
    }

    protected override void CreateChildControls ()
    {
      _view.ID = ID + "_View";
      Controls.Add (_view);

      _topControl.ID = ID + "_TopControl";
      Controls.Add (_topControl);

      _bottomControl.ID = ID + "_BottomControl";
      Controls.Add (_bottomControl);
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      //CreateTemplatedControls (DesignMode);
      EnsureChildControls();

      string key = typeof (SingleView).FullName + "_Style";
      if (!HtmlHeadAppender.Current.IsRegistered (key))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            this, Context, typeof (SingleView), ResourceType.Html, "SingleView.css");
        HtmlHeadAppender.Current.RegisterStylesheetLink (key, styleSheetUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    //private void CreateTemplatedControls (bool recreate)
    //{
    //  if (recreate)
    //  {
    //    ViewTemplateContainer.Controls.Clear();
    //    _viewTemplateContainer = null;
    //    Controls.Clear();
    //  }
    //  if (_viewTemplateContainer == null)
    //  {
    //    _viewTemplateContainer = CreateViewTemplateContainer();
    //    if (_viewTemplate != null)
    //      _viewTemplate.InstantiateIn (_viewTemplateContainer);
    //    AddViewTemplateContainer();
    //  }
    //  else if (ViewTemplate != null)
    //  {
    //    throw new InvalidOperationException (
    //        string.Format (
    //            "Cannot instantiate the ViewTemplate in the Init event when the ViewTemplateContainer was already created manually in SingleCiew with ID '{0}'.",
    //            ID));
    //  }
    //}

    //private void AddViewTemplateContainer ()
    //{
    //  Controls.Add (_viewTemplateContainer);
    //}

    //protected virtual Control CreateViewTemplateContainer ()
    //{
    //  return new PlaceHolder();
    //}

    protected override HtmlTextWriterTag TagKey
    {
      get { return HtmlTextWriterTag.Div; }
    }

    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      base.AddAttributesToRender (writer);
      if (ControlHelper.IsDesignMode (this, Context))
      {
        writer.AddStyleAttribute ("width", "100%");
        writer.AddStyleAttribute ("height", "75%");
      }

      if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
    }

    protected override void RenderContents (HtmlTextWriter writer)
    {
      EnsureChildControls ();

      if (ScriptUtility.IsPartOfRenderedOutput (this))
      {
        ScriptUtility.RegisterElementForBorderSpans (Page, ClientID + "_View");
        ScriptUtility.RegisterElementForBorderSpans (Page, _topControl.ClientID);
        ScriptUtility.RegisterElementForBorderSpans (Page, _bottomControl.ClientID);
      }

      if (!StringUtility.IsNullOrEmpty (CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClass);
      else if (!StringUtility.IsNullOrEmpty (Attributes["class"]))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, Attributes["class"]);
      else
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);

      writer.RenderBeginTag (HtmlTextWriterTag.Table);

      RenderTopControls (writer);
      RenderView (writer);
      RenderBottomControls (writer);

      writer.RenderEndTag();
    }

    protected virtual void RenderView (HtmlTextWriter writer)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Tr); // begin tr

      if (ControlHelper.IsDesignMode (this, Context))
        writer.AddStyleAttribute ("border", "solid 1px black");
      _viewStyle.AddAttributesToRender (writer);
      if (StringUtility.IsNullOrEmpty (_viewStyle.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassView);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      writer.AddAttribute (HtmlTextWriterAttribute.Id, ViewClientID);
      _viewStyle.AddAttributesToRender (writer);
      if (StringUtility.IsNullOrEmpty (_viewStyle.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassView);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassViewBody);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin body div

      writer.AddAttribute (HtmlTextWriterAttribute.Id, ClientID + "_View_Content");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      //_viewTemplateContainer.RenderControl (writer);
      _view.RenderControl (writer);

      writer.RenderEndTag(); // end content div
      writer.RenderEndTag (); // end body div
      writer.RenderEndTag(); // end outer div

      writer.RenderEndTag(); // end td
      writer.RenderEndTag(); // end tr
    }

    protected virtual void RenderTopControls (HtmlTextWriter writer)
    {
      Style style = _topControlsStyle;
      PlaceHolder placeHolder = _topControl;
      string cssClass = CssClassTopControls;
      RenderPlaceHolder (writer, style, placeHolder, cssClass);
    }

    protected virtual void RenderBottomControls (HtmlTextWriter writer)
    {
      Style style = _bottomControlsStyle;
      PlaceHolder placeHolder = _bottomControl;
      string cssClass = CssClassBottomControls;
      RenderPlaceHolder (writer, style, placeHolder, cssClass);
    }

    private void RenderPlaceHolder (HtmlTextWriter writer, Style style, PlaceHolder placeHolder, string cssClass)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Tr); // begin tr
      if (StringUtility.IsNullOrEmpty (style.CssClass))
      {
        if (placeHolder.Controls.Count > 0)
          writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
        else
          writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass + " " + CssClassEmpty);
      }
      else
      {
        if (placeHolder.Controls.Count > 0)
          writer.AddAttribute (HtmlTextWriterAttribute.Class, style.CssClass);
        else
          writer.AddAttribute (HtmlTextWriterAttribute.Class, style.CssClass + " " + CssClassEmpty);
      }
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      writer.AddAttribute (HtmlTextWriterAttribute.Id, placeHolder.ClientID);
      style.AddAttributesToRender (writer);
      if (StringUtility.IsNullOrEmpty (style.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      placeHolder.RenderControl (writer);

      writer.RenderEndTag(); // end content div
      writer.RenderEndTag(); // end outer div

      writer.RenderEndTag(); // end td
      writer.RenderEndTag(); // end tr
    }

    protected string ViewClientID
    {
      get { return ClientID + "_View"; }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public override ControlCollection Controls
    {
      get
      {
        EnsureChildControls();
        return base.Controls;
      }
    }

    [PersistenceMode (PersistenceMode.InnerProperty)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Browsable (false)]
    //[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    //[Browsable (false)]
    public ControlCollection View
    {
      get { return _view.Controls; }
      //get { return ViewTemplateContainer.Controls; }
    }

    //[TemplateInstance (TemplateInstance.Single)]
    ////[DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    //[PersistenceMode (PersistenceMode.InnerProperty)]
    //[Browsable (false)]
    //public ITemplate ViewTemplate
    //{
    //  get { return _viewTemplate; }
    //  set
    //  {
    //    if (!DesignMode)
    //    {
    //      if (_viewTemplateContainer != null)
    //      {
    //        throw new InvalidOperationException (
    //            string.Format (
    //                "The ViewTemplate of SingleView with ID '{0}' cannot be set after the template has been instantiated or the view template container has been created.",
    //                ID));
    //      }
    //      _viewTemplate = value;
    //    }
    //    else
    //    {
    //      _viewTemplate = value;
    //      CreateTemplatedControls (true);
    //    }
    //  }
    //}

    ////[Browsable (false)]
    //private Control ViewTemplateContainer
    //{
    //  get
    //  {
    //    if (_viewTemplateContainer == null)
    //    {
    //      _viewTemplateContainer = CreateViewTemplateContainer();
    //      AddViewTemplateContainer();
    //    }
    //    return _viewTemplateContainer;
    //  }
    //}

    [Category ("Style")]
    [Description ("The style that you want to apply to the active view.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style ViewStyle
    {
      get { return _viewStyle; }
    }

    [Category ("Style")]
    [Description ("The style that you want to the top section. The height cannot be provided in percent.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style TopControlsStyle
    {
      get { return _topControlsStyle; }
    }

    [Category ("Style")]
    [Description ("The style that you want to apply to the bottom section. The height cannot be provided in percent.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style BottomControlsStyle
    {
      get { return _bottomControlsStyle; }
    }

    [PersistenceMode (PersistenceMode.InnerProperty)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Browsable (false)]
    public ControlCollection TopControls
    {
      get { return _topControl.Controls; }
    }

    [PersistenceMode (PersistenceMode.InnerProperty)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Browsable (false)]
    public ControlCollection BottomControls
    {
      get { return _bottomControl.Controls; }
    }

    #region protected virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="SingleView"/>. </summary>
    /// <remarks> 
    ///   <para> Class: <c>singleView</c>. </para>
    /// </remarks>
    protected virtual string CssClassBase
    {
      get { return "singleView"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="SingleView"/>'s active view. </summary>
    /// <remarks> 
    ///   <para> Class: <c>singleViewActiveView</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="ViewStyle"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassView
    {
      get { return "singleViewView"; }
    }

    /// <summary> Gets the CSS-Class applied to the top section. </summary>
    /// <remarks> 
    ///   <para> Class: <c>singleViewTopControls</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="TopControlsStyle"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassTopControls
    {
      get { return "singleViewTopControls"; }
    }

    /// <summary> Gets the CSS-Class applied to the bottom section. </summary>
    /// <remarks> 
    ///   <para> Class: <c>singleViewBottomControls</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="BottomControlsStyle"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassBottomControls
    {
      get { return "singleViewBottomControls"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>div</c> wrapping the content and the border elements. </summary>
    /// <remarks> 
    ///   <para> Class: <c>body</c>. </para>
    /// </remarks>
    protected virtual string CssClassViewBody
    {
      get { return "body"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>div</c> intended for formatting the content. </summary>
    /// <remarks> 
    ///   <para> Class: <c>content</c>. </para>
    /// </remarks>
    protected virtual string CssClassContent
    {
      get { return "content"; }
    }

    /// <summary> Gets the CSS-Class applied when the section is empty. </summary>
    /// <remarks> 
    ///   <para> Class: <c>empty</c>. </para>
    ///   <para> 
    ///     Applied in addition to the regular CSS-Class. Use <c>td.singleViewTopControls.emtpy</c> or 
    ///     <c>td.singleViewBottomControls.emtpy</c>as a selector.
    ///   </para>
    /// </remarks>
    protected virtual string CssClassEmpty
    {
      get { return "empty"; }
    }

    #endregion
  }
}
