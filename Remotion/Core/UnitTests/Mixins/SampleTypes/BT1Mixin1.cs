// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
  public interface IBT1Mixin1
  {
    string IntroducedMethod ();
    string IntroducedProperty { get; }
    event EventHandler IntroducedEvent;
  }

  public class BT1M1Attribute : Attribute {}

  [Extends (typeof (BaseType1))]
  [Serializable]
  [BT1M1Attribute]
  [AcceptsAlphabeticOrdering]
  public class BT1Mixin1 : IBT1Mixin1
  {
    [OverrideTarget]
    [BT1M1Attribute]
    public string VirtualMethod ()
    {
      return "BT1Mixin1.VirtualMethod";
    }

    public string BackingField = "BT1Mixin1.BackingField";

    [OverrideTarget]
    [BT1M1Attribute]
    public virtual string VirtualProperty
    {
      set { BackingField = value; } // no getter
    }

    public bool VirtualEventAddCalled = false;
    public bool VirtualEventRemoveCalled = false;

    [OverrideTarget]
    [BT1M1Attribute]
    public virtual event EventHandler VirtualEvent
    {
      add { VirtualEventAddCalled = true; }
      remove { VirtualEventRemoveCalled = true; }
    }


    [BT1M1Attribute]
    public string IntroducedMethod ()
    {
      return "BT1Mixin1.IntroducedMethod";
    }

    [BT1M1Attribute]
    public string IntroducedProperty
    {
      get { return "BT1Mixin1.IntroducedProperty"; }
    }

    [BT1M1Attribute]
    public event EventHandler IntroducedEvent;
  }
}
