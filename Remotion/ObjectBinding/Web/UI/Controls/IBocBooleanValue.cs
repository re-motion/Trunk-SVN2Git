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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public interface IBocBooleanValue : IBocBooleanValueBase
  {
    /// <summary>
    ///   Gets the <see cref="Style"/> that you want to apply to the <see cref="Label"/> used for displaying the 
    ///   description. 
    /// </summary>
    Style LabelStyle { get; }

    string NullIconUrl { get; }
    string TrueIconUrl { get; }
    string FalseIconUrl { get; }
    string DefaultNullDescription { get; }
    string DefaultTrueDescription { get; }
    string DefaultFalseDescription { get; }
    string ResourceKey { get; }
    bool ShowDescription { get; }
    AttributeCollection Attributes { get; }
    string CssClass { get; set; }
    Unit Width { get; }
    CssStyleCollection Style { get; }
    Style ControlStyle { get; }
    string GetLabelKey ();
    string GetImageKey ();
    string GetHiddenFieldKey ();
    string GetHyperLinkKey ();
  }
}