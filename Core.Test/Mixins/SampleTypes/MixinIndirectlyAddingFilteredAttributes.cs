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
  [CopyCustomAttributes (typeof (AttributeSource), typeof (AttributeWithParameters2), typeof (AttributeWithParameters3))]
  public class MixinIndirectlyAddingFilteredAttributes : Mixin<object>
  {
    [AttributeWithParameters (1, "one", Property = 4, Field = 5)]
    [AttributeWithParameters2 (1, "one", Property = 4, Field = 5)]
    [AttributeWithParameters2 (1, "two", Property = 4, Field = 5)]
    [AttributeWithParameters3 (1, "two", Property = 4, Field = 5)]
    public class AttributeSource
    {
      [AttributeWithParameters (4)]
      [AttributeWithParameters2 (4)]
      [AttributeWithParameters2 (4)]
      [AttributeWithParameters3 (4)]
      public void AttributeSourceMethod ()
      {
      }
    }

    [OverrideTarget]
    [CopyCustomAttributes (typeof (AttributeSource), "AttributeSourceMethod", typeof (AttributeWithParameters2), typeof (AttributeWithParameters3))]
    public new string ToString ()
    {
      return "";
    }
  }
}
