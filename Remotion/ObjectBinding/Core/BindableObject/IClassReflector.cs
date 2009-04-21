// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
