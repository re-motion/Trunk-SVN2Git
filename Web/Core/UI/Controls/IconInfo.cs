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
using System.Drawing.Design;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{

[TypeConverter (typeof (IconInfoConverter))]
public sealed class IconInfo
{
  private static IconInfo s_spacer;

  public static IconInfo Spacer
  {
    get 
    { 
      if (s_spacer == null)
      {
        lock (typeof (IconInfo))
        {
          if (s_spacer == null)
          {
            string url = 
                ResourceUrlResolver.GetResourceUrl (null, typeof (IconInfo), ResourceType.Image, "Spacer.gif");
            s_spacer = new IconInfo (url);
          }
        }
      }
      return s_spacer; 
    }
  }

  public static bool ShouldSerialize (IconInfo icon)
  {
    if (icon == null)
    {
      return false;
    }
    else if (   StringUtility.IsNullOrEmpty (icon.Url)
             && StringUtility.IsNullOrEmpty (icon.AlternateText)
             && StringUtility.IsNullOrEmpty (icon.ToolTip)
             && icon.Height.IsEmpty
             && icon.Width.IsEmpty)
    {
      return false;
    }
    else
    {
      return true;
    }
  }

  public static void RenderInvisibleSpacer (HtmlTextWriter writer)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);

    writer.AddAttribute (HtmlTextWriterAttribute.Src, Spacer.Url);
    writer.AddAttribute (HtmlTextWriterAttribute.Alt, string.Empty);
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0px");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "0px");
    writer.AddStyleAttribute ("vertical-align", "middle");
    writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
    writer.RenderBeginTag (HtmlTextWriterTag.Img);
    writer.RenderEndTag();
  }

  private string _url;
  private string _alternateText;
  private string _toolTip;
  private Unit _width;
  private Unit _height;

  public IconInfo (string url, string alternateText, string toolTip, Unit width, Unit height)
  {
    Url = url;
    AlternateText = alternateText;
    ToolTip = toolTip;
    _width = width;
    _height = height;
  }

  public IconInfo (string url, Unit width, Unit height)
    : this (url, null, null, width, height)
  {
  }

  public IconInfo (string url, string alternateText, string toolTip, string width, string height)
    : this (url, null, toolTip, new Unit (width), new Unit (height))
  {
  }

  public IconInfo (string url, string width, string height)
    : this (url, null, null, width, height)
  {
  }

  public IconInfo (string url)
    : this (url, null, null, Unit.Empty, Unit.Empty)
  {
  }

  public IconInfo ()
    : this (string.Empty)
  {
  }

  [Editor (typeof (ImageUrlEditor), typeof (UITypeEditor))]
  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  [NotifyParentProperty (true)]
  public string Url
  {
    get { return _url; }
    set { _url = StringUtility.NullToEmpty (value); }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  [NotifyParentProperty (true)]
  public string AlternateText
  {
    get { return _alternateText; }
    set { _alternateText = StringUtility.NullToEmpty (value); }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  [NotifyParentProperty (true)]
  public string ToolTip
  {
    get { return  _toolTip; }
    set { _toolTip = StringUtility.NullToEmpty (value); }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue (typeof (Unit), "")]
  [NotifyParentProperty (true)]
  public Unit Width
  {
    get { return _width; }
    set { _width = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue (typeof (Unit), "")]
  [NotifyParentProperty (true)]
  public Unit Height
  {
    get { return _height; }
    set { _height = value; }
  }

  public override string ToString()
  {
    return _url;
  }

  public void Render (HtmlTextWriter writer)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);

    string url = UrlUtility.ResolveUrl (_url);
    writer.AddAttribute (HtmlTextWriterAttribute.Src, url);

    if (! _width.IsEmpty && ! _height.IsEmpty)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Width, _width.ToString());
      writer.AddAttribute (HtmlTextWriterAttribute.Height, _height.ToString());
    }
    writer.AddStyleAttribute ("vertical-align", "middle");
    writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");

    writer.AddAttribute (HtmlTextWriterAttribute.Alt, StringUtility.NullToEmpty (_alternateText));
    
    if (! StringUtility.IsNullOrEmpty (_toolTip))
      writer.AddAttribute (HtmlTextWriterAttribute.Title, _toolTip);

    writer.RenderBeginTag (HtmlTextWriterTag.Img);
    writer.RenderEndTag();
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public bool HasRenderingInformation
  {
    get { return ! StringUtility.IsNullOrEmpty (_url); }
  }

  public void Reset()
  {
    _url = string.Empty;
    _alternateText = string.Empty;
    _toolTip = string.Empty;
    _width = Unit.Empty;
    _height = Unit.Empty;
  }

  public void LoadResources (IResourceManager resourceManager)
  {
    if (resourceManager == null)
      return;

    string key;
    key = ResourceManagerUtility.GetGlobalResourceKey (Url);
    if (! StringUtility.IsNullOrEmpty (key))
      Url = resourceManager.GetString (key);

    key = ResourceManagerUtility.GetGlobalResourceKey (AlternateText);
    if (! StringUtility.IsNullOrEmpty (key))
      AlternateText = resourceManager.GetString (key);

    key = ResourceManagerUtility.GetGlobalResourceKey (ToolTip);
    if (! StringUtility.IsNullOrEmpty (key))
      ToolTip = resourceManager.GetString (key);
  }
}

public class IconInfoConverter: ExpandableObjectConverter
{
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (   context == null // Requried to circumvent the Designer
        && sourceType == typeof (string))
    {
      return true;
    }
    return base.CanConvertFrom (context, sourceType);
  }

  public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
  {
    if (destinationType == typeof (string))
      return true;
    return base.CanConvertTo (context, destinationType);
  }

  public override object ConvertFrom 
      (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
  {
    if (value == null)
      return null;
    if (value is string)
    {
      string stringValue = (string) value;
      IconInfo icon = new IconInfo();
      if (stringValue != string.Empty)
      {
        string[] valueParts = stringValue.Split (new char[]{'\0'}, 5);
        icon.Url = valueParts[0];
        if (valueParts[1] != string.Empty)
          icon.Width = Unit.Parse (valueParts[1]);
        if (valueParts[2] != string.Empty)
          icon.Height = Unit.Parse (valueParts[2]);
        icon.AlternateText = valueParts[3];
        icon.ToolTip = valueParts[4];
      }
      return icon;
    }

    return base.ConvertFrom (context, culture, value);
  }

  public override object ConvertTo
    (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
  {
    if (destinationType == typeof (string))
    {
      if (value == null)
        return null;
      if (value is IconInfo)
      {
        IconInfo icon = (IconInfo) value;
        if (context == null) // Requried to circumvent the Designer
        {
          if (IconInfo.ShouldSerialize (icon))
          {
            StringBuilder serializedValue = new StringBuilder();
            serializedValue.Append (icon.Url).Append ("\0");
            serializedValue.Append (icon.Width.ToString()).Append ("\0");
            serializedValue.Append (icon.Height.ToString()).Append ("\0");
            serializedValue.Append (icon.AlternateText).Append ("\0");
            serializedValue.Append (icon.ToolTip);
            return serializedValue.ToString();
          }
          else
          {
            return string.Empty;
          }
        }
        else
        {
          return icon.Url;
        }
      }
    }
    return base.ConvertTo (context, culture, value, destinationType);
  }


}

}
