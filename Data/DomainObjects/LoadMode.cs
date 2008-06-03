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

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Indicates whether a <see cref="DomainObject"/> was loaded as a whole or if only its <see cref="DataContainer"/> was loaded.
  /// </summary>
  public enum LoadMode
  {
    /// <summary>
    /// The whole object has been loaded, e.g. as a reaction to <see cref="DomainObject.GetObject{T}"/>.
    /// </summary>
    WholeDomainObjectInitialized,
    /// <summary>
    /// Only the object's <see cref="DataContainer"/> has been loaded, e.g. as a reaction to <see cref="ClientTransaction.EnlistDomainObject"/>.
    /// </summary>
    DataContainerLoadedOnly
  }
}
