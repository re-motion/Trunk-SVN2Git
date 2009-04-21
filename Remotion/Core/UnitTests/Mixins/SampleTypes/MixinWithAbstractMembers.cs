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
using System.Reflection;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IMixinWithAbstractMembers
  {
    string ImplementedMethod ();
    string ImplementedProperty ();
    string ImplementedEvent ();
  }

  [Serializable]
  public abstract class MixinWithAbstractMembers : Mixin<object, object>, IMixinWithAbstractMembers
  {
    public int I;

    public string ImplementedMethod ()
    {
      return "MixinWithAbstractMembers.ImplementedMethod-" + AbstractMethod (25);
    }

    public string ImplementedProperty ()
    {
      return "MixinWithAbstractMembers.ImplementedProperty-" + AbstractProperty;
    }

    public string ImplementedEvent ()
    {
      Func<string> func = delegate { return "MixinWithAbstractMembers.ImplementedEvent"; };
      AbstractEvent += func;
      string result = RaiseEvent ();
      AbstractEvent -= func;
      return result;
    }

    protected abstract string AbstractMethod (int i);
    protected abstract string AbstractProperty { get; }
    protected abstract event Func<string> AbstractEvent;
    protected abstract string RaiseEvent ();
  }
}
