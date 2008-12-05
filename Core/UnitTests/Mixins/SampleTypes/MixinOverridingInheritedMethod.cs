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
  public class MixinOverridingInheritedMethod : Mixin<object, MixinOverridingInheritedMethod.IBaseMethods>
  {
    public interface IBaseMethods
    {
      string ProtectedInheritedMethod ();
      string ProtectedInternalInheritedMethod ();
      string PublicInheritedMethod ();
    }

    [OverrideTarget]
    public string ProtectedInheritedMethod ()
    {
      return "MixinOverridingInheritedMethod.ProtectedInheritedMethod-" + Base.ProtectedInheritedMethod ();
    }

    [OverrideTarget]
    public string ProtectedInternalInheritedMethod ()
    {
      return "MixinOverridingInheritedMethod.ProtectedInternalInheritedMethod-" + Base.ProtectedInternalInheritedMethod ();
    }

    [OverrideTarget]
    public string PublicInheritedMethod ()
    {
      return "MixinOverridingInheritedMethod.PublicInheritedMethod-" + Base.PublicInheritedMethod ();
    }
  }

  [Serializable]
  public class BaseClassWithInheritedMethod
  {
    protected internal virtual string ProtectedInternalInheritedMethod ()
    {
      return "BaseClassWithInheritedMethod.ProtectedInternalInheritedMethod";
    }

    protected virtual string ProtectedInheritedMethod ()
    {
      return "BaseClassWithInheritedMethod.ProtectedInheritedMethod";
    }

    public virtual string PublicInheritedMethod ()
    {
      return "BaseClassWithInheritedMethod.PublicInheritedMethod";
    }
  }

  [Uses (typeof (MixinOverridingInheritedMethod))]
  public class ClassWithInheritedMethod : BaseClassWithInheritedMethod
  {
    public string InvokeInheritedMethods ()
    {
      return ProtectedInheritedMethod ()+ "-" + ProtectedInternalInheritedMethod() + "-" +  PublicInheritedMethod();
    }
  }
}
