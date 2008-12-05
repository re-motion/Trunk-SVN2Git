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
  [Serializable]
  public class MixinOverridingClassMethod : Mixin<object, MixinOverridingClassMethod.IRequirements>, IMixinOverridingClassMethod
  {
    public interface IRequirements
    {
      string OverridableMethod (int i);
    }

    public new object This { get { return base.This; } }
    public new object Base { get { return base.Base; } }

    [OverrideTarget]
    public string OverridableMethod (int i)
    {
      return "MixinOverridingClassMethod.OverridableMethod-" + i;
    }

    public virtual string AbstractMethod (int i)
    {
      return "MixinOverridingClassMethod.AbstractMethod-" + i;
    }
  }

  public interface IMixinOverridingClassMethod
  {
    string AbstractMethod (int i);
  }
}
