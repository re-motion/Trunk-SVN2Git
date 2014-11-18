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
  /// Interface for all <see cref="ControlObject"/> implementations representing a tab strip.
  /// </summary>
  public interface IControlObjectWithTabs
  {
    /// <summary>
    /// Start of the fluent interface for switching to a tab.
    /// </summary>
    IControlObjectWithTabs SwitchTo ();

    /// <summary>
    /// Short for explicitly implemented <see cref="WithItemID"/>.
    /// </summary>
    UnspecifiedPageObject SwitchTo (
        [NotNull] string itemID,
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null);

    /// <summary>
    /// Switches to the tab using the given <paramref name="itemID"/>.
    /// </summary>
    UnspecifiedPageObject WithItemID (
        [NotNull] string itemID,
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null);

    /// <summary>
    /// Switches to the tab using the given <paramref name="index"/>.
    /// </summary>
    UnspecifiedPageObject WithIndex (
        int index,
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null);

    /// <summary>
    /// Switches to the tab using the given <paramref name="htmlID"/>.
    /// </summary>
    UnspecifiedPageObject WithHtmlID (
        [NotNull] string htmlID,
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null);

    /// <summary>
    /// Switches to the tab using the given <paramref name="displayText"/>.
    /// </summary>
    UnspecifiedPageObject WithDisplayText (
        [NotNull] string displayText,
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null);
  }
}