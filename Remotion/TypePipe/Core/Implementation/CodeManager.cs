﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Remotion.FunctionalProgramming;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.Implementation.Synchronization;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.Implementation
{
  /// <summary>
  /// Manages the code generated by the pipeline by delegating to the contained <see cref="IGeneratedCodeFlusher"/> and
  /// <see cref="ITypeCache"/> instance.
  /// </summary>
  public class CodeManager : ICodeManager
  {
    private static readonly ConstructorInfo s_typePipeAssemblyAttributeCtor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new TypePipeAssemblyAttribute ("participantConfigurationID"));

    private readonly ICodeManagerSynchronizationPoint _codeManagerSynchronizationPoint;
    private readonly ITypeCache _typeCache;

    public CodeManager (ICodeManagerSynchronizationPoint codeManagerSynchronizationPoint, ITypeCache typeCache)
    {
      ArgumentUtility.CheckNotNull ("codeManagerSynchronizationPoint", codeManagerSynchronizationPoint);
      ArgumentUtility.CheckNotNull ("typeCache", typeCache);

      _codeManagerSynchronizationPoint = codeManagerSynchronizationPoint;
      _typeCache = typeCache;
    }

    public string AssemblyDirectory
    {
      get { return _codeManagerSynchronizationPoint.AssemblyDirectory; }
    }

    public string AssemblyNamePattern
    {
      get { return _codeManagerSynchronizationPoint.AssemblyNamePattern; }
    }

    public void SetAssemblyDirectory (string assemblyDirectory)
    {
      _codeManagerSynchronizationPoint.SetAssemblyDirectory (assemblyDirectory);
    }

    public void SetAssemblyNamePattern (string assemblyNamePattern)
    {
      _codeManagerSynchronizationPoint.SetAssemblyNamePattern (assemblyNamePattern);
    }

    public string FlushCodeToDisk (IEnumerable<CustomAttributeDeclaration> assemblyAttributes)
    {
      var participantConfigurationID = _typeCache.ParticipantConfigurationID;
      var typePipeAttribute = new CustomAttributeDeclaration (s_typePipeAssemblyAttributeCtor, new object[] { participantConfigurationID });
      var attributes = assemblyAttributes.Concat (typePipeAttribute);

      return _codeManagerSynchronizationPoint.FlushCodeToDisk (attributes);
    }

    public string FlushCodeToDisk (CustomAttributeDeclaration[] assemblyAttributes)
    {
      ArgumentUtility.CheckNotNull ("assemblyAttributes", assemblyAttributes);

      return FlushCodeToDisk ((IEnumerable<CustomAttributeDeclaration>) assemblyAttributes);
    }

    public void LoadFlushedCode (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      LoadFlushedCode ((_Assembly) assembly);
    }

    [CLSCompliant (false)]
    public void LoadFlushedCode (_Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      var typePipeAttribute = (TypePipeAssemblyAttribute) assembly.GetCustomAttributes (typeof (TypePipeAssemblyAttribute), inherit: false).SingleOrDefault();
      if (typePipeAttribute == null)
        throw new ArgumentException ("The specified assembly was not generated by the pipeline.", "assembly");

      if (typePipeAttribute.ParticipantConfigurationID != _typeCache.ParticipantConfigurationID)
      {
        var message = string.Format (
            "The specified assembly was generated with a different participant configuration: '{0}'.", typePipeAttribute.ParticipantConfigurationID);
        throw new ArgumentException (message, "assembly");
      }

      _typeCache.LoadTypes (assembly.GetTypes());
    }
  }
}