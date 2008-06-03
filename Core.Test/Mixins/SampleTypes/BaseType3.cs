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
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBaseType31
  {
    string IfcMethod ();
  }

  public interface IBaseType32
  {
    string IfcMethod ();
  }

  public interface IBaseType33
  {
    string IfcMethod ();
  }

  public interface IBaseType34 : IBaseType33
  {
    new string IfcMethod ();
  }

  public interface IBaseType35
  {
    string IfcMethod2 ();
  }

  [Uses (typeof (BT3Mixin5))]
  [Serializable]
  public class BaseType3 : IBaseType31, IBaseType32, IBaseType34, IBaseType35
  {
    public virtual string IfcMethod ()
    {
      return "BaseType3.IfcMethod";
    }

    public string IfcMethod2 ()
    {
      return "BaseType3.IfcMethod2";
    }
  }
}
