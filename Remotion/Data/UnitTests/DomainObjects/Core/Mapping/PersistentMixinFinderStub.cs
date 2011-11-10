// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins.Context;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  public class PersistentMixinFinderStub : IPersistentMixinFinder
  {
    private readonly Type _classType;
    private readonly Type[] _persistentMixins;

    private readonly ClassContext _mixinConfiguration;

    public PersistentMixinFinderStub (Type classType, params Type[] persistentMixins) : this (classType, new ClassContext (classType, persistentMixins))
    {
    }

    public PersistentMixinFinderStub (Type classType, ClassContext mixinConfiguration)
    {
      _classType = classType;
      _persistentMixins = mixinConfiguration.Mixins.Select (m => m.MixinType).ToArray();

      _mixinConfiguration = mixinConfiguration;
    }

    public ClassContext MixinConfiguration
    {
      get { return _mixinConfiguration; }
    }

    public ClassContext ParentClassContext
    {
      get { throw new System.NotImplementedException(); }
    }

    public bool IncludeInherited
    {
      get { throw new System.NotImplementedException(); }
    }

    public Type[] GetPersistentMixins ()
    {
      return _persistentMixins;
    }

    public bool IsInParentContext (Type mixinType)
    {
      throw new System.NotImplementedException();
    }

    public Type FindOriginalMixinTarget (Type mixinType)
    {
      return _classType;
    }
  }
}
