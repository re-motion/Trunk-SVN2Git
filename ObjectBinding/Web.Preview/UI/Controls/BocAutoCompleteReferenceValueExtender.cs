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
using System.Collections.Generic;
using Microsoft.Web.Preview.UI;
using System.Web.UI;
using System.ComponentModel;
using AjaxControlToolkit;

[assembly: WebResource (Remotion.ObjectBinding.Web.UI.Controls.BocAutoCompleteReferenceValueExtender.ScriptReference, "text/javascript")]

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  [TargetControlType (typeof (Control))]
  [ToolboxItem (false)]
  //[Designer (typeof (BocAutoCompleteReferenceValueExtenderDesigner))]
  [ClientScriptResource ("Remotion.UI.BocAutoCompleteReferenceValueBehavior", BocAutoCompleteReferenceValueExtender.ScriptReference)]
  [RequiredScript (typeof (CommonToolkitScripts))]
  [RequiredScript (typeof (PopupExtender))]
  [RequiredScript (typeof (TimerScript))]
  public class BocAutoCompleteReferenceValueExtender : ExtenderControlBase
  {
    // constants

    internal const string ScriptReference = "Remotion.ObjectBinding.Web.UI.Controls.BocAutoCompleteReferenceValueBehavior.js";

    // types

    // static members

    // member fields

    // construction and disposing

    // methods and properties

    //protected override void RenderScriptAttributes (XmlWriter writer, TargetControlPropertiesBase<IBusinessObjectBoundEditableWebControl> properties)
    //{
      //ArgumentUtility.CheckNotNull ("writer", writer);
      //ArgumentUtility.CheckNotNull ("properties", properties);

      //if (StringUtility.IsNullOrEmpty (_servicePath))
      //  throw new InvalidOperationException ("The ServicePath must be set for BocAutoCompleteReferenceValueExtender");
      //if (StringUtility.IsNullOrEmpty (_serviceMethod))
      //  throw new InvalidOperationException ("The ServiceMethod must be set for BocAutoCompleteReferenceValueExtender");

      //writer.WriteAttributeString ("dropDownPanelClass", _dropDownPanelClass);
      //writer.WriteAttributeString ("minimumPrefixLength", _minimumPrefixLength.ToString ());
      //writer.WriteAttributeString ("completionSetCount", _completionSetCount.ToString ());
      //writer.WriteAttributeString ("completionInterval", _completionInterval.ToString ());
      //writer.WriteAttributeString ("serviceURL", ResolveClientUrl (_servicePath));
      //writer.WriteAttributeString ("serviceMethod", _serviceMethod);
      //writer.WriteAttributeString ("args", _args);
      //if (_ownerControl.DataSource != null)
      //{
      //  if (_ownerControl.DataSource.BusinessObjectClass != null && _ownerControl.Property != null)
      //  {
      //    writer.WriteAttributeString ("businessObjectClass", _ownerControl.DataSource.BusinessObjectClass.Identifier);
      //    writer.WriteAttributeString ("businessObjectProperty", _ownerControl.Property.Identifier);
      //  }

      //  if (_ownerControl.DataSource != null)
      //  {
      //    IBusinessObjectWithIdentity businessObject = _ownerControl.DataSource.BusinessObject as IBusinessObjectWithIdentity;
      //    if (businessObject != null)
      //      writer.WriteAttributeString ("businessObjectID", businessObject.UniqueIdentifier);
      //  }
      //}
    //}

    //protected virtual bool IsEmpty
    //{
    //  get
    //  {
    //    if (!Enabled
    //        && MinimumPrefixLength == 1
    //        && string.IsNullOrEmpty (ServiceMethod)
    //        && string.IsNullOrEmpty (ServiceUrl)
    //        && CompletionSetCount == 10
    //        && CompletionInterval == 1000
    //        && SuggestionInterval == 200
    //        && string.IsNullOrEmpty (BusinessObjectClass)
    //        && string.IsNullOrEmpty (BusinessObjectProperty)
    //        && string.IsNullOrEmpty (BusinessObjectID)
    //        && string.IsNullOrEmpty (Args)
    //        && string.IsNullOrEmpty (DropDownPanelClass)
    //        && string.IsNullOrEmpty (TextBoxID)
    //        && string.IsNullOrEmpty (HiddenFieldID)
    //     )
    //    {
    //      return true;
    //    }
    //    return false;
    //  }
    //}

    //[ExtenderControlProperty]
    //[Category ("Behavior")]
    //[DefaultValue (false)]
    //public bool Enabled
    //{
    //  get { return GetPropertyBoolValue ("Enabled"); }
    //  set { SetPropertyBoolValue ("Enabled", value); }
    //}

    protected override IEnumerable<ScriptReference> GetScriptReferences ()
    {
      List<ScriptReference> scriptReferences = new List<ScriptReference> ();
      scriptReferences.Add (new ScriptReference(FrameworkScript.PreviewMainScriptResource, FrameworkScript.PreviewAssembly));
      scriptReferences.AddRange (base.GetScriptReferences ());
      return scriptReferences;
    }
    [ExtenderControlProperty]
    [Category ("Behavior")]
    [DefaultValue (1)]
    public int MinimumPrefixLength
    {
      get
      {
        return GetPropertyValue<int> ("MinimumPrefixLength", 1);
      }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException ("value", "The MinimumPrefixLength must be greater than or equal to 0.");
        SetPropertyIntValue ("MinimumPrefixLength", value);
      }
    }

    [ExtenderControlProperty]
    [Category ("Behavior")]
    [DefaultValue ("")]
    public string ServiceMethod
    {
      get { return GetPropertyStringValue ("ServiceMethod"); }
      set { SetPropertyStringValue ("ServiceMethod", value); }
    }

    [ExtenderControlProperty]
    [Category ("Behavior")]
    [DefaultValue ("")]
    public string ServiceUrl
    {
      get { return GetPropertyStringValue ("ServiceUrl"); }
      set { SetPropertyStringValue ("ServiceUrl", value); }
    }

    [ExtenderControlProperty]
    [Category ("Behavior")]
    [DefaultValue (10)]
    public int? CompletionSetCount
    {
      get
      {
        return GetPropertyValue<int?> ("CompletionSetCount", 10);
      }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException ("value", "The CompletionSetCount must be greater than or equal to 0.");
        SetPropertyValue<int?> ("CompletionSetCount", value);
      }
    }

    [ExtenderControlProperty]
    [Category ("Behavior")]
    [DefaultValue (1000)]
    public int CompletionInterval
    {
      get
      {
        return GetPropertyValue<int> ("CompletionInterval", 1000);
      }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException ("value", "The CompletionInterval must be greater than or equal to 0.");
        SetPropertyIntValue ("CompletionInterval", value);
      }
    }

    [ExtenderControlProperty]
    [Category ("Behavior")]
    [DefaultValue (200)]
    public int SuggestionInterval
    {
      get
      {
        return GetPropertyValue<int> ("SuggestionInterval", 200);
      }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException ("value", "The SuggestionInterval must be greater than or equal to 0.");
        SetPropertyIntValue ("SuggestionInterval", value);
      }
    }

    [ExtenderControlProperty]
    [Category ("Style")]
    [DefaultValue ("")]
    public string DropDownPanelClass
    {
      get { return GetPropertyStringValue ("DropDownPanelClass"); }
      set { SetPropertyStringValue ("DropDownPanelClass", value); }
    }

    [ExtenderControlProperty]
    [Category ("Data")]
    [DefaultValue ("")]
    public string BusinessObjectClass
    {
      get { return GetPropertyStringValue ("BusinessObjectClass"); }
      set { SetPropertyStringValue ("BusinessObjectClass", value); }
    }

    [ExtenderControlProperty]
    [Category ("Data")]
    [DefaultValue ("")]
    public string BusinessObjectProperty
    {
      get { return GetPropertyStringValue ("BusinessObjectProperty"); }
      set { SetPropertyStringValue ("BusinessObjectProperty", value); }
    }

    [ExtenderControlProperty]
    [Category ("Data")]
    [DefaultValue ("")]
    public string BusinessObjectID
    {
      get { return GetPropertyStringValue ("BusinessObjectID"); }
      set { SetPropertyStringValue ("BusinessObjectID", value); }
    }

    [ExtenderControlProperty]
    [Category ("Data")]
    [DefaultValue ("")]
    public string Args
    {
      get { return GetPropertyStringValue ("Args"); }
      set { SetPropertyStringValue ("Args", value); }
    }

    [ExtenderControlProperty]
    [Browsable (false)]
    public string TextBoxID
    {
      get { return GetPropertyStringValue ("TextBoxID"); }
      set { SetPropertyStringValue ("TextBoxID", value); }
    }

    [ExtenderControlProperty]
    [Browsable (false)]
    public string HiddenFieldID
    {
      get { return GetPropertyStringValue ("HiddenFieldID"); }
      set { SetPropertyStringValue ("HiddenFieldID", value); }
    }
  }
}
