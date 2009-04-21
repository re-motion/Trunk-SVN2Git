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
  [Uses(typeof(MixinWithAbstractMembers))]
  [Serializable]
  public class ClassOverridingMixinMembers
  {
    [OverrideMixin]
    public virtual string AbstractMethod(int i)
    {
      return "ClassOverridingMixinMembers.AbstractMethod-" + i;
    }

    [OverrideMixin]
    public virtual string AbstractProperty
    {
      get { return "ClassOverridingMixinMembers.AbstractProperty"; }
    }

    [OverrideMixin]
    public virtual string RaiseEvent ()
    {
      return _abstractEvent ();
    }

    private Func<string> _abstractEvent;

    [OverrideMixin]
    public virtual event Func<string> AbstractEvent
    {
      add { _abstractEvent += value; }
      remove { _abstractEvent -= value; }
    }
  }
}
