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
  public class MixinWithCircularThisDependency1 : Mixin<ICircular2>, ICircular1
  {
    public string Circular1 ()
    {
      return "MixinWithCircularThisDependency1.Circular1-" + This.Circular2 ();
    }
  }

  public class MixinWithCircularThisDependency2 : Mixin<ICircular1>, ICircular2
  {
    public string Circular2 ()
    {
      return "MixinWithCircularThisDependency2.Circular2";
    }

    public string Circular12 ()
    {
      return "MixinWithCircularThisDependency2.Circular12-" + This.Circular1();
    }
  }

  public interface ICircular1
  {
    string Circular1 ();
  }

  public interface ICircular2
  {
    string Circular2 ();
    string Circular12 ();
  }
}
