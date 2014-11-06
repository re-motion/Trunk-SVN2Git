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
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> or <see cref="PageObject"/> implementations hosting a collection of
  /// <see cref="WebButtonControlObject"/>s.
  /// </summary>
  public interface IWebTestObjectWithWebButtons
  {
     /// <summary>
    /// Presses the button given by <paramref name="itemID"/>, using a given <paramref name="completionDetection"/> to wait for the triggered
    /// action's results.
    /// </summary>
    /// <param name="itemID">The button's item ID without the trailing "Button", e.g. "Save" for "SaveButton".</param>
    /// <param name="completionDetection">Required <see cref="ICompletionDetection"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    UnspecifiedPageObject Perform ([NotNull] string itemID, [CanBeNull] ICompletionDetection completionDetection = null);
  }
}