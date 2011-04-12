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
namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides useful extension methods for the <see cref="DomainObject"/> class.
  /// </summary>
  public static class DomainObjectExtensions
  {
    /// <summary>
    /// Returns the <see cref="DomainObject.ID"/> of the given <paramref name="domainObject"/>, or <see langword="null" /> if 
    /// <paramref name="domainObject"/> is itself <see langword="null" />.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> whose <see cref="DomainObject.ID"/> to get. If this parameter is 
    /// <see langword="null" />, the method returns <see langword="null" />.</param>
    /// <returns>The <paramref name="domainObject"/>'s <see cref="DomainObject.ID"/>, or <see langword="null" /> if <paramref name="domainObject"/>
    /// is <see langword="null" />.</returns>
    public static ObjectID GetIDOrNull (this DomainObject domainObject)
    {
      return domainObject != null ? domainObject.ID : null;
    }
  }
}