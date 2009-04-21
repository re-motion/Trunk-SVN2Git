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
using System.Diagnostics;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Apply this mixin to a type in order to add an <see cref="IBusinessObjectWithIdentity"/> implementation.
  /// </summary>
  [Serializable]
  [BindableObjectWithIdentityProvider]
  [CopyCustomAttributes (typeof (DebuggerDisplay))]
  public abstract class BindableObjectWithIdentityMixin : BindableObjectMixin, IBusinessObjectWithIdentity
  {
    [DebuggerDisplay ("{UniqueIdentifier} ({((Remotion.Mixins.IMixinTarget)this).Configuration.Type.FullName})")]
    internal class DebuggerDisplay // the attributes of this class are copied to the target class
    {
    }

    public BindableObjectWithIdentityMixin ()
    {
    }

    public abstract string UniqueIdentifier { get; }
  }
}
