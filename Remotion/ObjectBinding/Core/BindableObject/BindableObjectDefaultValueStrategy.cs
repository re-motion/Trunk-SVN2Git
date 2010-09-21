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
using System;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Provides a standard implementaton of <see cref="IDefaultValueStrategy"/>. The standard implementation is that the value is never seen as the
  /// default value.
  /// </summary>
  public class BindableObjectDefaultValueStrategy : IDefaultValueStrategy
  {
    // TODO Review 3272: Since the strategy is stateless, refactor to make it a singleton with private ctor and public static readonly Instance field.

    public bool IsDefaultValue (IBusinessObject obj, PropertyBase property)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);
      ArgumentUtility.CheckNotNull ("property", property);

      return false;
    }
  }
}