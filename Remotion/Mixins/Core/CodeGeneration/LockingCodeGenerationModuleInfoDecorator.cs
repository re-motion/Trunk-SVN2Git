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
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Decorates a given instance of <see cref="IModuleManager"/>, making it thread-safe by synchronizing all access to its members with a lock.
  /// </summary>
  public class LockingCodeGenerationModuleInfoDecorator : ICodeGenerationModuleInfo
  {
    private readonly ICodeGenerationModuleInfo _innerCodeGenerationModuleInfo;
    private readonly object _lockObject;

    public LockingCodeGenerationModuleInfoDecorator (ICodeGenerationModuleInfo innerCodeGenerationModuleInfo, object lockObject)
    {
      ArgumentUtility.CheckNotNull ("innerCodeGenerationModuleInfo", innerCodeGenerationModuleInfo);
      ArgumentUtility.CheckNotNull ("lockObject", lockObject);

      _innerCodeGenerationModuleInfo = innerCodeGenerationModuleInfo;
      _lockObject = lockObject;
    }

    public ICodeGenerationModuleInfo InnerCodeGenerationModuleInfo
    {
      get { return _innerCodeGenerationModuleInfo; }
    }

    public string SignedAssemblyName
    {
      get
      {
        lock (_lockObject)
        {
          return _innerCodeGenerationModuleInfo.SignedAssemblyName;
        }
      }
      set
      {
        lock (_lockObject)
        {
          _innerCodeGenerationModuleInfo.SignedAssemblyName = value;
        }
      }
    }

    public string UnsignedAssemblyName
    {
      get
      {
        lock (_lockObject)
        {
          return _innerCodeGenerationModuleInfo.UnsignedAssemblyName;
        }
      }
      set
      {
        lock (_lockObject)
        {
          _innerCodeGenerationModuleInfo.UnsignedAssemblyName = value;
        }
      }
    }

    public string SignedModulePath
    {
      get
      {
        lock (_lockObject)
        {
          return _innerCodeGenerationModuleInfo.SignedModulePath;
        }
      }
      set
      {
        lock (_lockObject)
        {
          _innerCodeGenerationModuleInfo.SignedModulePath = value;
        }
      }
    }

    public string UnsignedModulePath
    {
      get
      {
        lock (_lockObject)
        {
          return _innerCodeGenerationModuleInfo.UnsignedModulePath;
        }
      }
      set
      {
        lock (_lockObject)
        {
          _innerCodeGenerationModuleInfo.UnsignedModulePath = value;
        }
      }
    }

    public bool HasAssemblies
    {
      get
      {
        lock (_lockObject)
        {
          return _innerCodeGenerationModuleInfo.HasAssemblies;
        }
      }
    }

    public bool HasSignedAssembly
    {
      get
      {
        lock (_lockObject)
        {
          return _innerCodeGenerationModuleInfo.HasSignedAssembly;
        }
      }
    }

    public bool HasUnsignedAssembly
    {
      get
      {
        lock (_lockObject)
        {
          return _innerCodeGenerationModuleInfo.HasUnsignedAssembly;
        }
      }
    }
  }
}