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

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  //TODO: doc
  public interface IExtendedBusinessObjectBoundWebControl : IBusinessObjectBoundWebControl
  {
    /// <summary>
    ///   Gets the interfaces derived from <see cref="IBusinessObjectProperty"/> supported by this control, or <see langword="null"/> if no 
    ///   restrictions are made.
    /// </summary>
    Type[] SupportedPropertyInterfaces { get; }

    /// <summary> Indicates whether properties with the specified multiplicity are supported. </summary>
    /// <param name="isList"> <see langword="true"/> if the property is a list property. </param>
    /// <returns> <see langword="true"/> if the multiplicity specified by <paramref name="isList"/> is supported. </returns>
    bool SupportsPropertyMultiplicity (bool isList);
  }
}
