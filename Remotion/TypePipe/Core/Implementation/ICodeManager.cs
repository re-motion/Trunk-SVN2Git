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
using System.Reflection;

namespace Remotion.TypePipe.Implementation
{
  /// <summary>
  /// Manages the code generated by the pipeline. Supports saving and loading of generated code.
  /// </summary>
  public interface ICodeManager
  {
    /// <summary>
    /// Gets the directory in which assemblies will be saved when <see cref="FlushCodeToDisk"/> is invoked; <see langword="null"/> means the
    /// current working directory.
    /// </summary>
    /// <value>The assembly directory path or <see langword="null"/>.</value>
    string AssemblyDirectory { get; }

    /// <summary>
    /// Gets the assembly name, that is, the assembly file name without the extension (<c>.dll</c>) that will be used for flushing the next assembly.
    /// Note that <see cref="FlushCodeToDisk"/> assigns a new value to this property to ensure unqiue assembly names.
    /// </summary>
    /// <value>The assembly name (file name without <c>.dll</c> extension).</value>
    //
    // TODO review: Update docs!!
    // 
    string AssemblyNamePattern { get; }

    /// <summary>
    /// Sets the directory in that assemblies will be saved, when <see cref="FlushCodeToDisk"/> is called.
    /// If <see langword="null"/> is specified, the assemblies are saved to the current working directory.
    /// </summary>
    /// <param name="assemblyDirectory">The assembly directory, or <see langword="null"/>.</param>
    /// <exception cref="InvalidOperationException">
    /// If the pipeline already generated a type (because it was requested) into the new assembly, the assembly directory cannot be changed.
    /// </exception>
    void SetAssemblyDirectory (string assemblyDirectory);

    /// <summary>
    /// Sets the name for the next assembly.
    /// When using this API, the user is responsible for providing unique assembly names.
    /// </summary>
    /// <remarks>
    /// Note that the provided value is overwritten after every call to <see cref="FlushCodeToDisk"/> to ensure uniqueness.
    /// </remarks>
    /// <param name="assemblyNamePattern">The assembly name (file name without <c>.dll</c> extension).</param>
    /// <exception cref="InvalidOperationException">
    /// If the pipeline already generated a type (because it was requested) into the new assembly, the assembly name cannot be changed.
    /// </exception>
    //
    // TODO review: Update docs!!
    // 
    void SetAssemblyNamePattern (string assemblyNamePattern);

    /// <summary>
    /// Saves all types that have been generated since the last call to this method into a new assembly on disk.
    /// The file name of the assembly consists of <see cref="AssemblyNamePattern"/> plus the file ending <c>.dll</c>.
    /// The assembly is written to the directory defined by <see cref="AssemblyDirectory"/>.
    /// If <see cref="AssemblyDirectory"/> is <see langword="null"/> the assembly is saved in the current working directory.
    /// </summary>
    /// <remarks>
    /// If no new types have been generated since the last call to <see cref="FlushCodeToDisk"/>, this method does nothing
    /// and returns <see langword="null"/>.
    /// This method also generates a new unique name for the next assembly.
    /// </remarks>
    /// <returns>The absolute path to the saved assembly file, or <see langword="null"/> if no assembly was saved.</returns>
    string FlushCodeToDisk ();

    /// <summary>
    /// Attempts to load all types in a previously flushed <see cref="Assembly"/>.
    /// Note that only assemblies that were generated by the pipeline with an equivalent participant configuration can be loaded.
    /// </summary>
    /// <param name="assembly">The flushed assembly which should be loaded.</param>
    /// <exception cref="ArgumentException">
    /// If the assembly was not generated by the pipeline; or if it was generated with a different participant configuration.
    /// </exception>
    void LoadFlushedCode (Assembly assembly);
  }
}