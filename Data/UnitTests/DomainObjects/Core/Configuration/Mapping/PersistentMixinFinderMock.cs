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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins.Context;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  public class PersistentMixinFinderMock : IPersistentMixinFinder
  {
    private readonly Type[] _persistentMixins;

    public PersistentMixinFinderMock (params Type[] persistentMixins)
    {
      _persistentMixins = persistentMixins;
    }

    public ClassContext MixinConfiguration
    {
      get { throw new System.NotImplementedException(); }
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
      throw new System.NotImplementedException();
    }
  }
}