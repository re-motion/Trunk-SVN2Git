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
using System.Web.UI.WebControls;
using AttributeCollection=System.Web.UI.AttributeCollection;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering
{
  public interface IBocRenderableControl
  {
    /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocCheckBox</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    string CssClassBase { get; }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>readOnly</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocCheckBox.readOnly</c> as a selector. </para>
    /// </remarks>
    string CssClassReadOnly { get; }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocCheckBox.disabled</c> as a selector.</para>
    /// </remarks>
    string CssClassDisabled { get; }

    /// <summary> Evalutes whether this control is in <b>Design Mode</b>. </summary>
    [Browsable (false)]
    bool IsDesignMode { get; }

    bool Enabled { get; }
    AttributeCollection Attributes { get; }
    string CssClass { get; set; }
    CssStyleCollection Style { get; }
    Style ControlStyle { get; }

    Unit Width { get; set; }
    Unit Height { get; set; }
  }
}