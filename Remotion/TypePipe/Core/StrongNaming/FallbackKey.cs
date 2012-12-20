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
using System.IO;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.StrongNaming
{
  public static class FallbackKey
  {
    // TODO 5057: Use Lazy<T>
    private static readonly DoubleCheckedLockingContainer<StrongNameKeyPair> s_instance = new DoubleCheckedLockingContainer<StrongNameKeyPair> (Load);

    public static StrongNameKeyPair KeyPair
    {
      get { return s_instance.Value; }
    }

    private static StrongNameKeyPair Load ()
    {
      var assembly = Assembly.GetExecutingAssembly();
      using (var resourceStream = assembly.GetManifestResourceStream (typeof (FallbackKey), "FallbackKey.snk"))
      {
        var memoryStream = new MemoryStream (596);
        FileUtility.CopyStream (resourceStream, memoryStream);
        var bytes = memoryStream.ToArray();

        return new StrongNameKeyPair (bytes);
      }
    }
  }
}