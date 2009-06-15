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
using Remotion.Globalization;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  ///   Provides <see cref="IComponent"/>-like functionality for non-UI items of controls.
  /// </summary>
  /// <remarks>
  ///   <b>IComponent</b> is not used because it involves CodeDOM designer serialization.
  /// </remarks>
  public interface IControlItem
  {
    IControl OwnerControl { get; set; }
    string ItemID { get; }
    void LoadResources (IResourceManager resourceManager);
  }
}