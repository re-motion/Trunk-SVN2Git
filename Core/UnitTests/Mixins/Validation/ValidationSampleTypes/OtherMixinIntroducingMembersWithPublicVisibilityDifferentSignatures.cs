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

namespace Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes
{
  public interface IOtherMixinIntroducingMembersWithPublicVisibilityDifferentSignatures
  {
    void MethodWithPublicVisibility (int i);
    string PropertyWithPublicVisibility { get; set; }
    event Action EventWithPublicVisibility;
  }

  public class OtherMixinIntroducingMembersWithPublicVisibilityDifferentSignatures : IOtherMixinIntroducingMembersWithPublicVisibilityDifferentSignatures
  {
    [MemberVisibility (MemberVisibility.Public)]
    public void MethodWithPublicVisibility (int i)
    {
    }

    [MemberVisibility (MemberVisibility.Public)]
    public string PropertyWithPublicVisibility
    {
      get { return ""; }
      set { Dev.Null = value; }
    }

    [MemberVisibility (MemberVisibility.Public)]
    public event Action EventWithPublicVisibility;
  }
}