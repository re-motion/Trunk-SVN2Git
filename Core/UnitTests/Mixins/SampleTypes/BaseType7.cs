/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBaseType7
  {
    string One<T> (T t);
    string Two ();
    string Three ();
    string Four ();
    string Five ();
    string NotOverridden ();
  }

  public class BaseType7 : IBaseType7
  {
    public virtual string One<T> (T t)
    {
      return "BaseType7.One(" + t + ")";
    }

    public virtual string Two ()
    {
      return "BaseType7.Two";
    }

    public virtual string Three ()
    {
      return "BaseType7.Three";
    }

    public virtual string Four ()
    {
      return "BaseType7.Four-" + Five();
    }

    public virtual string Five ()
    {
      return "BaseType7.Five";
    }

    public string NotOverridden ()
    {
      return "BaseType7.NotOverridden";
    }
  }
}
