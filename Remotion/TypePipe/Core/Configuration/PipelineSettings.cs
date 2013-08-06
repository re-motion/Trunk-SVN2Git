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
using Remotion.Utilities;

namespace Remotion.TypePipe.Configuration
{
  /// <summary>
  /// Holds configuration options for pipelines. This class is immutable.
  /// </summary>
  /// <seealso cref="PipelineFactory.Create(string,Remotion.TypePipe.Configuration.PipelineSettings,Remotion.TypePipe.IParticipant[])"/>
  /// <seealso cref="IPipeline.Settings"/>
  public class PipelineSettings
  {
    private static readonly PipelineSettings s_defaults = New().Build();

    public static PipelineSettings Defaults
    {
      get { return s_defaults; }
    }

    public static Builder New ()
    {
      return new Builder();
    }

    public static Builder From (PipelineSettings settings)
    {
      ArgumentUtility.CheckNotNull ("settings", settings);

      return New()
          .SetForceStrongNaming (settings.EnableSerializationWithoutAssemblySaving)
          .SetKeyFilePath (settings.KeyFilePath)
          .SetEnableSerializationWithoutAssemblySaving (settings.EnableSerializationWithoutAssemblySaving);
    }

    private readonly bool _forceStrongNaming;
    private readonly string _keyFilePath;
    private readonly bool _enableSerializationWithoutAssemblySaving;

    public PipelineSettings (bool forceStrongNaming, string keyFilePath, bool enableSerializationWithoutAssemblySaving)
    {
      // Key file path may be null.

      _forceStrongNaming = forceStrongNaming;
      _keyFilePath = keyFilePath;
      _enableSerializationWithoutAssemblySaving = enableSerializationWithoutAssemblySaving;
    }

    /// <summary>
    /// If <see langword="true"/>, the pipeline signs all generated assemblies or throws an <see cref="InvalidOperationException"/> if that is not
    /// possible.
    /// </summary>
    public bool ForceStrongNaming
    {
      get { return _forceStrongNaming; }
    }

    /// <summary>
    /// When <see cref="ForceStrongNaming"/> is enabled, the key file (<c>*.snk</c>) denoted by this property is used to sign generated assemblies.
    /// If this property is <see langword="null"/> a pipeline-provided default key file is used instead.
    /// </summary>
    public string KeyFilePath
    {
      get { return _keyFilePath; }
    }

    /// <summary>
    /// Enables the serialization of assembled type instances without the need of saving the generated assembly to disk.
    /// </summary>
    public bool EnableSerializationWithoutAssemblySaving
    {
      get { return _enableSerializationWithoutAssemblySaving; }
    }

    public class Builder
    {
      private bool _forceStrongNaming;
      private string _keyFilePath;
      private bool _enableSerializationWithoutAssemblySaving;

      public Builder SetForceStrongNaming (bool forceStrongNaming)
      {
        _forceStrongNaming = forceStrongNaming;
        return this;
      }

      public Builder SetKeyFilePath (string keyFilePath)
      {
        // Key file path may be null.

        _keyFilePath = keyFilePath;
        return this;
      }

      public Builder SetEnableSerializationWithoutAssemblySaving (bool enableSerializationWithoutAssemblySaving)
      {
        _enableSerializationWithoutAssemblySaving = enableSerializationWithoutAssemblySaving;
        return this;
      }

      public PipelineSettings Build ()
      {
        return new PipelineSettings (_forceStrongNaming, _keyFilePath, _enableSerializationWithoutAssemblySaving);
      }
    }
  }
}