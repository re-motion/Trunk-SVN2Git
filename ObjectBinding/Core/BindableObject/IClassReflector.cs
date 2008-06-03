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

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Instances exposing the <see cref="IClassReflector"/> interface can be used to create a <see cref="BindableObjectClass"/> for a <see cref="Type"/>.
  /// </summary>
  public interface IClassReflector
  {
    /// <summary>
    /// The <see cref="Type"/> for which to create the <see cref="BindableObjectClass"/>.
    /// </summary>
    Type TargetType { get; }

    /// <summary>
    /// The <see cref="BindableObjectProvider"/> associated with the <see cref="TargetType"/>.
    /// </summary>
    BindableObjectProvider BusinessObjectProvider { get; }

    /// <summary>
    /// Creates an instance of type <see cref="BindableObjectClass"/> for the <see cref="TargetType"/>.
    /// </summary>
    BindableObjectClass GetMetadata ();
  }
}
