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
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface which must be supported by all control objects holding <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/> nodes.
  /// </summary>
  public interface IControlObjectWithNodes<TNodeControlObject>
      where TNodeControlObject : ControlObject
  {
    /// <summary>
    /// Start of the fluent interface for selecting a node.
    /// </summary>
    IControlObjectWithNodes<TNodeControlObject> GetNode ();

    /// <summary>
    /// Short for explicitly implemented <see cref="WithItemID"/>.
    /// </summary>
    TNodeControlObject GetNode ([NotNull] string itemID);

    /// <summary>
    /// Selects the node using the given <paramref name="itemID"/>.
    /// </summary>
    TNodeControlObject WithItemID ([NotNull] string itemID);

    /// <summary>
    /// Selects the node using the given <paramref name="index"/>.
    /// </summary>
    TNodeControlObject WithIndex (int index);

    /// <summary>
    /// Selects the node using the given <paramref name="displayText"/>.
    /// </summary>
    TNodeControlObject WithDisplayText ([NotNull] string displayText);
  }
}