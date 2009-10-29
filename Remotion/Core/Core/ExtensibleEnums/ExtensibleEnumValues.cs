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
    public T[] GetValues ()
    {
      var typeDiscoveryService = ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService();
      var types = typeDiscoveryService.GetTypes (null, false).Cast<Type>();

      throw new NotImplementedException ("TODO");
    }
  }
}