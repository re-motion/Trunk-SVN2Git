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
  internal interface IInternalInterface1
  {
    string MethodInInternalInterface ();
  }

  internal interface IInternalInterface2
  {
  }

  [Uses (typeof (MixinWithClassFaceImplementingInternalInterface))]
  public class ClassImplementingInternalInterface : IInternalInterface1, IInternalInterface2
  {
    public string MethodInInternalInterface ()
    {
      return "ClassImplementingInternalInterface.Foo";
    }
  }

  public class MixinWithClassFaceImplementingInternalInterface : Mixin <ClassImplementingInternalInterface>
  {
    public string GetStringViaThis ()
    {
      return This.MethodInInternalInterface ();
    }
  }
}
