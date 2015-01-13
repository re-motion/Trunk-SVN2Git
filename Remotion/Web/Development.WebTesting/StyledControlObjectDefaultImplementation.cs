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
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// See <see cref="IStyledControlObject"/> for documentation.
  /// </summary>
  public static class StyledControlObjectDefaultImplementation
  {
    /// <summary>
    /// Returns whether the control has the given <paramref name="cssClass"/>.
    /// </summary>
    /// <returns>True if the control has the given <paramref name="cssClass"/>, false otherwise.</returns>
    public static bool HasCssClass ([NotNull] this IStyledControlObject styledControlObject, string cssClass)
    {
      ArgumentUtility.CheckNotNull ("styledControlObject", styledControlObject);
      ArgumentUtility.CheckNotNullOrEmpty ("cssClass", cssClass);

      var controlObject = GetControlObject (styledControlObject);
      var scope = GetStyleScope (controlObject);
      return scope["class"].Split (' ').Contains (cssClass);
    }

    /// <summary>
    /// Returns the computed background color of the control. This method ignores background images as well as transparencies - the first
    /// non-transparent color set in the node's DOM hierarchy is returned. The returned color's alpha value is always 255 (opaque).
    /// </summary>
    /// <returns>The background color or <see cref="Color.Transparent"/> if no background color is set (not even on any parent node).</returns>
    public static Color GetBackgroundColor ([NotNull] this IStyledControlObject styledControlObject)
    {
      ArgumentUtility.CheckNotNull ("styledControlObject", styledControlObject);

      var controlObject = GetControlObject (styledControlObject);
      var scope = GetStyleScope (controlObject);
      return scope.GetComputedBackgroundColor (controlObject.Context); // Todo RM-6337: Move GetComutedBackgroundColor() source to here?
    }

    /// <summary>
    /// Returns the computed text color of the control. This method ignores transparencies - the first non-transparent color set in the node's
    /// DOM hierarchy is returned. The returned color's alpha value is always 255 (opaque).
    /// </summary>
    /// <returns>The text color or <see cref="Color.Transparent"/> if no text color is set (not even on any parent node).</returns>
    public static Color GetTextColor ([NotNull] this IStyledControlObject styledControlObject)
    {
      ArgumentUtility.CheckNotNull ("styledControlObject", styledControlObject);

      var controlObject = GetControlObject (styledControlObject);
      var scope = GetStyleScope (controlObject);
      return scope.GetComputedTextColor (controlObject.Context); // Todo RM-6337: Move GetComputedTextColor() source to here?
    }

    private static ControlObject GetControlObject ([NotNull] this IStyledControlObject styledControlObject)
    {
      ArgumentUtility.CheckNotNull ("styledControlObject", styledControlObject);

      var controlObject = styledControlObject as ControlObject;
      if (controlObject == null)
      {
        throw new NotSupportedException (
            "The IStyledControlObject interface may only be put on classes derived from Remotion.Web.Development.WebTesting.ControlObject.");
      }

      return controlObject;
    }

    /// <summary>
    /// Default implementation for <see cref="IStyledControlObjectWithCustomStyleScope"/>, 
    /// </summary>
    private static ElementScope GetStyleScope ([NotNull] this ControlObject controlObject)
    {
      ArgumentUtility.CheckNotNull ("controlObject", controlObject);

      var styledControlObjectWithCustomStyleScope = controlObject as IStyledControlObjectWithCustomStyleScope;
      if (styledControlObjectWithCustomStyleScope != null)
        return styledControlObjectWithCustomStyleScope.GetStyleScope();

      return controlObject.Scope;
    }
  }
}