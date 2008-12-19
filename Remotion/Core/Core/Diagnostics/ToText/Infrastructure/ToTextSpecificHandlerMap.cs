// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;

namespace Remotion.Diagnostics.ToText.Infrastructure
{
  /// <summary>
  /// A map mapping from types to  <see cref="IToTextSpecificHandler"/>s.
  /// </summary>
  /// <typeparam name="THandler"></typeparam>
  public class ToTextSpecificHandlerMap<THandler> : Dictionary<Type, THandler> where THandler : IToTextSpecificHandler
  {
    /// <summary>
    /// Adds the entries from the passed <see cref="ToTextSpecificHandlerMap{THandler}"/> to the map.
    /// </summary>
    public void Add(ToTextSpecificHandlerMap<THandler> handlerMap)
    {
      foreach (var pair in handlerMap) 
      {
        Add (pair.Key, pair.Value);
      }
    }
  }
}