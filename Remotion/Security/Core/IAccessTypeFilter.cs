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
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Remotion.Security
{
  /// <summary>
  /// Allows the filtering of a sequence of access types during the permission evaluation.
  /// </summary>
  /// <remarks>
  /// <para>Implementators can decide whether to provide a shareable or a non-shared filter implementation.</para>
  /// <para>The filter rules can be based on the <see cref="ISecurityContext"/> and the <see cref="ISecurityPrincipal"/>.</para>
  /// <note type="inotes">
  /// The <see cref="Filter"/> method may only remove (i.e. filter) the access types but not introduce new access types into the result.
  /// </note>
  /// </remarks>
  /// <seealso cref="NullAccessTypeFilter"/>
  public interface IAccessTypeFilter : INullObject
  {
    /// <summary>
    /// Streams the <paramref name="accessTypes"/> to the return value and optionally removes items from the sequence.
    /// </summary>
    /// <param name="accessTypes">The sequence of <see cref="AccessType"/>s to filter.</param>
    /// <param name="context">The <see cref="ISecurityContext"/> for which the permissions are evaluated.</param>
    /// <param name="principal">The <see cref="ISecurityPrincipal"/> on whose behalf the permissions are evaluated.</param>
    /// <returns></returns>
    [NotNull]
    IEnumerable<AccessType> Filter (
        [NotNull] IEnumerable<AccessType> accessTypes,
        [NotNull] ISecurityContext context,
        [NotNull] ISecurityPrincipal principal);
  }
}