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

namespace Remotion.Reflection
{
  /// <summary>
  /// Null-object implementation of <see cref="IMethodInformation"/>.
  /// </summary>
  public class NullMethodInformation : IMethodInformation
  {
    public string Name
    {
      get { return null; }
    }

    public Type DeclaringType
    {
      get { return null; }
    }

    public Type GetOriginalDeclaringType ()
    {
      return null;
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return null;
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return new T[] {};
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return false;
    }

    public Type ReturnType
    {
      get { return null; }
    }

    public object Invoke (object instance, object[] parameters)
    {
      return null;
    }
  }
}