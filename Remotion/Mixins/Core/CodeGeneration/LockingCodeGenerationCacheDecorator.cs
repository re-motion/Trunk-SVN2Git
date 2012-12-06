// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using Remotion.Mixins.Context;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Decorates a given instance of <see cref="ICodeGenerationCache"/>, making it thread-safe by synchronizing all access to its members with a lock.
  /// </summary>
  public class LockingCodeGenerationCacheDecorator : ICodeGenerationCache
  {
    private readonly ICodeGenerationCache _innerCache;
    private readonly object _lockObject;

    public LockingCodeGenerationCacheDecorator (ICodeGenerationCache innerCache, object lockObject)
    {
      ArgumentUtility.CheckNotNull ("innerCache", innerCache);
      ArgumentUtility.CheckNotNull ("lockObject", lockObject);

      _innerCache = innerCache;
      _lockObject = lockObject;
    }

    public ICodeGenerationCache InnerCache
    {
      get { return _innerCache; }
    }

    public void Clear ()
    {
      lock (_lockObject)
      {
        _innerCache.Clear();
      }
    }

    public Type GetOrCreateConcreteType (ClassContext classContext)
    {
      lock (_lockObject)
      {
        return _innerCache.GetOrCreateConcreteType (classContext);
      }
    }

    public IConstructorLookupInfo GetOrCreateConstructorLookupInfo (ClassContext classContext, bool allowNonPublic)
    {
      lock (_lockObject)
      {
        return _innerCache.GetOrCreateConstructorLookupInfo (classContext, allowNonPublic);
      }
    }

    public ConcreteMixinType GetOrCreateConcreteMixinType (ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier)
    {
      lock (_lockObject)
      {
        return _innerCache.GetOrCreateConcreteMixinType (concreteMixinTypeIdentifier);
      }
    }

    public void ImportTypes (IEnumerable<Type> types, IConcreteTypeMetadataImporter metadataImporter)
    {
      lock (_lockObject)
      {
        _innerCache.ImportTypes (types, metadataImporter);
      }
    }
  }
}