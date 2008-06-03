/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Reflection;

namespace Remotion.Globalization
{
  public interface IResourcesAttribute
  {
    /// <summary>
    ///   Gets the base name of the resource container as specified by the attributes construction.
    /// </summary>
    /// <remarks>
    /// The base name of the resource conantainer to be used by this type
    /// (&lt;assembly&gt;.&lt;path inside project&gt;.&lt;resource file name without extension&gt;).
    /// </remarks>
    string BaseName { get; }

    Assembly ResourceAssembly { get; }
  }
}
