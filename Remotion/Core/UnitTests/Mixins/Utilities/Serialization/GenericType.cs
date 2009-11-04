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
namespace Remotion.UnitTests.Mixins.Utilities.Serialization
{
  public class GenericType<T>
  {
    public GenericType () { }
      
    public GenericType (T t) { }

    public void NonGenericMethod (T t)
    {
    }

    public void GenericMethod<U> (T t, U u)
    {
    }

    public int NonGenericProperty
    {
      get { return 0; }
    }

    public T GenericProperty
    {
      get { return default (T); }
    }

    public object this [int index]
    {
      get { return null; }
    }

    public T this [string index]
    {
      get { return default (T); }
    }

    public int this[T index]
    {
      get { return 0; }
    }

    public T this[T index, int i]
    {
      get { return index; }
    }

    public T this[T index, T t]
    {
      get { return index; }
    }
  }
}
