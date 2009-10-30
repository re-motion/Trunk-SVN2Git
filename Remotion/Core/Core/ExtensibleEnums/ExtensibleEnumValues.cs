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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Reflection.TypeDiscovery;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Provides discovery services for extensible enum values. Extensible enum implementations should
  /// hold an instance of this class in a static propery of the <see cref="ExtensibleEnum"/> subclass representing
  /// the enumeration. The values of the enumeration should be defined as extension methods for <see cref="ExtensibleEnumValues{T}"/>,
  /// where <typeparamref name="T"/> is the <see cref="ExtensibleEnum"/> subclass.
  /// </summary>
  /// <typeparam name="T">The subclass of <see cref="ExtensibleEnum"/> that represents the enumeration.</typeparam>
  public class ExtensibleEnumValues<T>
    where T : ExtensibleEnum
  {
    private readonly DoubleCheckedLockingContainer<T[]> _valueCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtensibleEnumValues{T}"/> class.
    /// </summary>
    public ExtensibleEnumValues ()
    {
      _valueCache = new DoubleCheckedLockingContainer<T[]> (RetrieveValues);
    }

    /// <summary>
    /// Gets the values defined by the extensible enum type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>A <see cref="ReadOnlyCollection{T}"/> holding the values for <typeparamref name="T"/>.</returns>
    /// <remarks>
    /// The values are retrieved by scanning all types found by <see cref="ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService"/>
    /// and discovering the extension methods defining values via <see cref="ExtensibleEnumValueDiscoveryService"/>.
    /// </remarks>
    public ReadOnlyCollection<T> GetValues ()
    {
      return new ReadOnlyCollection<T> (_valueCache.Value);
    }

    private T[] RetrieveValues ()
    {
      var typeDiscoveryService = ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService();
      var types = typeDiscoveryService.GetTypes (null, false).Cast<Type>();

      var valueDiscoveryService = new ExtensibleEnumValueDiscoveryService();
      return valueDiscoveryService.GetValues (this, types).ToArray();
    }
  }
}