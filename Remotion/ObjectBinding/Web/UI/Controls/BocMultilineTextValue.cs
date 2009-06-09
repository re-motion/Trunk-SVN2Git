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
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocTextValueBase;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> This control can be used to display or edit a list of strings. </summary>
  /// <include file='doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/Class/*' />
  [ValidationProperty ("Text")]
  [DefaultEvent ("TextChanged")]
  [ToolboxItemFilter ("System.Web.UI")]
  public class BocMultilineTextValue : BocTextValueBase, IBocMultilineTextValue
  {
    // types

    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocMultilineTextValue")]
    protected enum ResourceIdentifier
    {
      /// <summary> The validation error message displayed when no text is entered but input is required. </summary>
      RequiredValidationMessage,
      /// <summary> The validation error message displayed when entered text exceeds the maximum length. </summary>
      MaxLengthValidationMessage
    }

    // static members

    private static readonly Type[] s_supportedPropertyInterfaces = new[] { typeof (IBusinessObjectStringProperty) };

    private string _text = string.Empty;

    // construction and disposing

    public BocMultilineTextValue ()
        : base (TextBoxMode.MultiLine)
    {
    }

    // methods and properties

    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (! interim)
      {
        if (Property != null && DataSource != null && DataSource.BusinessObject != null)
        {
          string[] value = (string[]) DataSource.BusinessObject.GetProperty (Property);
          LoadValueInternal (value, interim);
        }
      }
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> The <see cref="String"/> <see cref="Array"/> to load or <see langword="null"/>. </param>
    /// <include file='doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/LoadUnboundValue/*' />
    public void LoadUnboundValue (string[] value, bool interim)
    {
      LoadValueInternal (value, interim);
    }

    /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/SaveValue/*' />
    public override void SaveValue (bool interim)
    {
      if (!interim && IsDirty)
      {
        if (Property != null && DataSource != null && DataSource.BusinessObject != null && !IsReadOnly)
        {
          DataSource.BusinessObject.SetProperty (Property, Value);
          IsDirty = false;
        }
      }
    }

    public override void RenderControl (HtmlTextWriter writer)
    {
      var factory = ServiceLocator.Current.GetInstance<IBocMultilineTextValueRendererFactory> ();
      var renderer = factory.CreateRenderer (new HttpContextWrapper (Context), writer, this);
      renderer.Render();
    }

    // ReSharper disable RedundantOverridenMember
    // included for documentation
    /// <include file='doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/CreateValidators/*' />
    public override BaseValidator[] CreateValidators ()
    {
      return base.CreateValidators ();
    }
    // ReSharper restore RedundantOverridenMember

    /// <summary> Gets or sets the <see cref="IBusinessObjectStringProperty"/> object this control is bound to. </summary>
    /// <value> An <see cref="IBusinessObjectStringProperty"/> object. </value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public new IBusinessObjectStringProperty Property
    {
      get { return (IBusinessObjectStringProperty) base.Property; }
      set { base.Property = ArgumentUtility.CheckType<IBusinessObjectStringProperty> ("value", value); }
    }

    /// <summary> Gets or sets the current value. </summary>
    /// <value> The <see cref="String"/> array currently displayed or <see langword="null"/> if no text is entered. </value>
    /// <remarks> The dirty state is reset when the value is set. </remarks>
    [Browsable (false)]
    public new string[] Value
    {
      get
      {
        string text = _text;
        if (text != null)
          text = text.Trim ();

        if (StringUtility.IsNullOrEmpty (text))
          return null;
        else
        {
          //  Allows for an optional \r
          string temp = _text.Replace ("\r", "");
          return temp.Split ('\n');
        }
      }
      set
      {
        IsDirty = true;

        if (value == null)
          _text = null;
        else
          _text = StringUtility.ConcatWithSeparator (value, "\r\n");
      }
    }

    /// <summary> Gets or sets the string representation of the current value. </summary>
    /// <remarks> Uses <c>\r\n</c> or <c>\n</c> as separation characters. The default value is an empty <see cref="String"/>. </remarks>
    [Description ("The string representation of the current value.")]
    [Category ("Data")]
    [DefaultValue ("")]
    public override string Text
    {
      get { return StringUtility.NullToEmpty (_text); }
      set { _text = value; }
    }

    protected override void LoadControlState (object savedState)
    {
      object[] values = (object[]) savedState;

      base.LoadControlState (values[0]);
      _text = (string) values[1];
    }

    protected override object SaveControlState ()
    {
      object[] values = new object[2];

      values[0] = base.SaveControlState ();
      values[1] = _text;

      return values;
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (string[] value, bool interim)
    {
      if (! interim)
      {
        Value = value;
        IsDirty = false;
      }
    }

    /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
    protected override IResourceManager GetResourceManager ()
    {
      return GetResourceManager (typeof (ResourceIdentifier));
    }

    protected override IEnumerable<BaseValidator> GetValidators ()
    {
      IList<BaseValidator> validators = new List<BaseValidator> (2);

      RequiredFieldValidator requiredValidator = new RequiredFieldValidator();
      requiredValidator.ID = ID + "_ValidatorRequired";
      requiredValidator.ControlToValidate = TargetControl.ID;
      if (StringUtility.IsNullOrEmpty (ErrorMessage))
      {
        requiredValidator.ErrorMessage =
            GetResourceManager().GetString (ResourceIdentifier.RequiredValidationMessage);
      }
      else
        requiredValidator.ErrorMessage = ErrorMessage;
      validators.Add (requiredValidator);

      if (TextBoxStyle.MaxLength != null)
      {
        LengthValidator lengthValidator = new LengthValidator();
        lengthValidator.ID = ID + "_ValidatorMaxLength";
        lengthValidator.ControlToValidate = TargetControl.ID;
        lengthValidator.MaximumLength = TextBoxStyle.MaxLength.Value;
        if (StringUtility.IsNullOrEmpty (ErrorMessage))
        {
          string maxLengthMessage = GetResourceManager().GetString (ResourceIdentifier.MaxLengthValidationMessage);
          lengthValidator.ErrorMessage = string.Format (maxLengthMessage, TextBoxStyle.MaxLength.Value);
        }
        else
          lengthValidator.ErrorMessage = ErrorMessage;
        validators.Add (lengthValidator);
      }
      return validators;
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    /// <value> The value must be of type <b>string[]</b>. </value>
    protected override object ValueImplementation
    {
      get { return Value; }
      set { Value = (string[]) value; }
    }

    /// <summary>
    ///   The <see cref="BocMultilineTextValue"/> supports properties of types <see cref="IBusinessObjectStringProperty"/>.
    /// </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    /// <summary> The <see cref="BocMultilineTextValue"/> supports only list properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="true"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return isList;
    }
  }
}