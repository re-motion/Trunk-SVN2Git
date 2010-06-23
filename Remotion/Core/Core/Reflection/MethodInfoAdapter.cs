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
using System.Globalization;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Implements the <see cref="IMethodInformation"/> to wrap a <see cref="MethodInfo"/> instance.
  /// </summary>
  public class MethodInfoAdapter : IMethodInformation
  {
    private readonly MethodInfo _methodInfo;

    public MethodInfoAdapter (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);

      _methodInfo = methodInfo;
    }

    public MethodInfo MethodInfo
    {
      get { return _methodInfo; }
    }

    public Type ReturnType
    {
      get { throw new NotImplementedException(); }
    }

    public string Name
    {
      get { throw new NotImplementedException(); }
    }

    public Type DeclaringType
    {
      get { throw new NotImplementedException(); }
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      throw new NotImplementedException();
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      throw new NotImplementedException();
    }

    public object Invoke (object instance, object parameters)
    {
      throw new NotImplementedException();
    }

    public object Invoke (object instance, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}