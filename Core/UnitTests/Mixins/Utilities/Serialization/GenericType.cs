/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
