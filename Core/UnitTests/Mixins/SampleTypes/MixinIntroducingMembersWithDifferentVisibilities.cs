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
using Remotion.Development.UnitTesting;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IMixinIntroducingMembersWithDifferentVisibilities
  {
    void MethodWithDefaultVisibility ();
    int PropertyWithDefaultVisibility { get; set; }
    event EventHandler EventWithDefaultVisibility;

    void MethodWithPublicVisibility ();
    int PropertyWithPublicVisibility { get; set; }
    event EventHandler EventWithPublicVisibility;
  }

  public class MixinIntroducingMembersWithDifferentVisibilities : IMixinIntroducingMembersWithDifferentVisibilities
  {
    public void MethodWithDefaultVisibility ()
    {
    }

    public int PropertyWithDefaultVisibility
    {
      get { return 0; }
      set { Dev.Null = value; }
    }

    public event EventHandler EventWithDefaultVisibility;

    [MemberVisibility (MemberVisibility.Public)]
    public void MethodWithPublicVisibility ()
    {
    }

    [MemberVisibility (MemberVisibility.Public)]
    public int PropertyWithPublicVisibility
    {
      get { return 0; }
      set { Dev.Null = value; }
    }

    [MemberVisibility (MemberVisibility.Public)]
    public event EventHandler EventWithPublicVisibility;
  }
}