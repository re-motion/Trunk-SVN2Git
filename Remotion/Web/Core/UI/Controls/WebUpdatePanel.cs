// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;
using AttributeCollection = System.Web.UI.AttributeCollection;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Extends the ASP.NET <see cref="UpdatePanel"/> with support for HTML attributes, allowing specification of a <see cref="CssClass"/> 
  /// or inline styles.
  /// </summary>
  public class WebUpdatePanel : UpdatePanel, IAttributeAccessor
  {
    private string _cssClass = "";
    private AttributeCollection _attributeCollection;
    private StateBag _attributeStateBag;
    private WebUpdatePanelRenderMode _renderMode;
    private InternalControlMemberCaller _memberCaller = new InternalControlMemberCaller();

    public WebUpdatePanel ()
    {
    }

    [Category ("Appearance")]
    [CssClassProperty]
    [DefaultValue ("")]
    [Description ("Class name applied to the control.")]
    public string CssClass
    {
      get { return _cssClass; }
      set { _cssClass = StringUtility.NullToEmpty (value); }
    }

    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public CssStyleCollection Style
    {
      get { return Attributes.CssStyle; }
    }

    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public AttributeCollection Attributes
    {
      get
      {
        if (_attributeCollection == null)
        {
          if (_attributeStateBag == null)
          {
            _attributeStateBag = new StateBag (true);
            if (IsTrackingViewState)
              ((IStateManager) _attributeStateBag).TrackViewState();
          }
          _attributeCollection = new AttributeCollection (_attributeStateBag);
        }
        return _attributeCollection;
      }
    }

    [Description ("Indicates whether the UpdatePanel should render as a block tag (<div>), an inline tag (<span>), or a table section (tbody, thead, tfoot).")]
    [DefaultValue (WebUpdatePanelRenderMode.Div)]
    [Category ("Layout")]
    public new WebUpdatePanelRenderMode RenderMode
    {
      get
      {
        return _renderMode;
      }
      set
      {
        _renderMode = ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<WebUpdatePanelRenderMode> ("value", value);
      }
    }
 
    protected override void LoadViewState (object savedState)
    {
      if (savedState != null)
      {
        var triplet = (Triplet) savedState;

        base.LoadViewState (triplet.First);

        if (triplet.Second != null)
        {
          if (_attributeStateBag == null)
          {
            _attributeStateBag = new StateBag (true);
            ((IStateManager) _attributeStateBag).TrackViewState();
          }
          ((IStateManager) _attributeStateBag).LoadViewState (triplet.Second);
        }

        _cssClass = StringUtility.NullToEmpty ((string) triplet.Third);
      }
    }

    protected override object SaveViewState ()
    {
      object baseViewState = base.SaveViewState();

      object attributesViewState = null;
      if (_attributeStateBag != null)
        attributesViewState = ((IStateManager) _attributeStateBag).SaveViewState();

      if ((baseViewState == null) && (attributesViewState == null) && string.IsNullOrEmpty (_cssClass))
        return null;
      return new Triplet (baseViewState, attributesViewState, _cssClass);
    }

    protected virtual void AddAttributesToRender (HtmlTextWriter writer)
    {
      if (!string.IsNullOrEmpty (_cssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, _cssClass);

      if (_attributeStateBag != null)
      {
        foreach (string key in Attributes.Keys)
          writer.AddAttribute (key, Attributes[key]);
      }
    }

    protected override void RenderChildren (HtmlTextWriter writer)
    {
      if (IsInPartialRendering)
      {
        base.RenderChildren (writer);
      }
      else
      {
        AddAttributesToRender (writer);
        writer.AddAttribute (HtmlTextWriterAttribute.Id, ClientID);
        writer.RenderBeginTag (GetTagName());

        _memberCaller.RenderChildrenInternal (this, writer, Controls);

        writer.RenderEndTag ();

        _memberCaller.SetUpdatePanelRendered (this, true);
      }
    }

    private HtmlTextWriterTag GetTagName ()
    {
#pragma warning disable 612,618
      switch (_renderMode)
      {
        case WebUpdatePanelRenderMode.Block:
        case WebUpdatePanelRenderMode.Div:
          return HtmlTextWriterTag.Div;

        case WebUpdatePanelRenderMode.Inline:
        case WebUpdatePanelRenderMode.Span:
          return HtmlTextWriterTag.Span;

        case WebUpdatePanelRenderMode.Tbody:
          return HtmlTextWriterTag.Tbody;

        case WebUpdatePanelRenderMode.Thead:
          return HtmlTextWriterTag.Thead;
        
        case WebUpdatePanelRenderMode.Tfoot:
          return HtmlTextWriterTag.Tfoot;

        default:
          throw new InvalidOperationException(string.Format ("The RenderMode '{0}' is not valid.", _renderMode));
      }
#pragma warning restore 612,618
    }

    string IAttributeAccessor.GetAttribute (string name)
    {
      if (_attributeStateBag == null)
        return null;
      return (string) _attributeStateBag[name];
    }

    void IAttributeAccessor.SetAttribute (string name, string value)
    {
      Attributes[name] = value;
    }
  }
}