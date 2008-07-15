/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Collections;
using Remotion.Implementation;

namespace Remotion.Security.BridgeInterfaces
{
  [ConcreteImplementation ("Remotion.Security.BridgeImplementations.AccessTypeCacheImplementation, Remotion.Security, Version = <version>, PublicKeyToken = <publicKeyToken>")]
  public interface IAccessTypeCacheImplementation
  {
    ICache<EnumWrapper, AccessType> CreateCache ();
  }
}
