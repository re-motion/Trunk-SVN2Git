// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
