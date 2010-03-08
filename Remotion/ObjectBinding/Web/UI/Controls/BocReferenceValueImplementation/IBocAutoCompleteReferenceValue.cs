// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Web.UI.WebControls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation
{
  public interface IBocAutoCompleteReferenceValue : IBocRenderableControl, IBusinessObjectBoundEditableWebControl
  {
    BocCommand Command { get; }
    DropDownMenu OptionsMenu { get; }
    string TextBoxUniqueID { get; }
    string TextBoxClientID { get; }
    string HiddenFieldUniqueID { get; }
    string HiddenFieldClientID { get; }
    string IconUniqueID { get; }
    string BusinessObjectDisplayName { get; }
    string BusinessObjectUniqueIdentifier { get; }
    string DropDownButtonClientID { get; }
    string ServicePath { get; }
    string ServiceMethod { get; }

    /// <summary> Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    SingleRowTextBoxStyle TextBoxStyle { get; }

    /// <summary> Gets the style that you want to apply to the <see cref="Label"/> (read-only mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    Style LabelStyle { get; }

    /// <summary>
    ///   Gets the style that you want to apply to the text box (edit mode) 
    ///   and the label (read-only mode).
    /// </summary>
    /// <remarks>
    ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual 
    ///   style settings for the respective modes. Note that if you set one of the <b>Font</b> 
    ///   attributes (Bold, Italic etc.) to <see langword="true"/>, this cannot be overridden using 
    ///   <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/>  properties.
    /// </remarks>
    Style CommonStyle { get; }

    bool EmbedInOptionsMenu { get; }
    bool HasOptionsMenu { get; }
    Unit OptionsMenuWidth { get; }
    bool EnableIcon { get; }
    bool? HasValueEmbeddedInsideOptionsMenu { get; }
    bool IsCommandEnabled (bool readOnly);
    IconInfo GetIcon();

    new IBusinessObjectReferenceProperty Property { get; }
    new IBusinessObjectWithIdentity Value { get; }
    string NullValueString { get; }
    int? CompletionSetCount { get; }
    int CompletionInterval { get; }
    int SuggestionInterval { get; }
    string Args { get; }
    string GetLabelText ();
  }
}