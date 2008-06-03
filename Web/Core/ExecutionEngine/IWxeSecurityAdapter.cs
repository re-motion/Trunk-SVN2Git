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
using Remotion.Security;

namespace Remotion.Web.ExecutionEngine
{
  //TODO FS: Move to SecurityInterfaces
  //verwendet in wxe um security abfragen zu tun.
  public interface IWxeSecurityAdapter : ISecurityAdapter
  {
    // verwendet wenn function l�uft. 
    bool HasAccess (WxeFunction function);
    //verwendet bevor wxefunction initialisiert wurde und nur typ bekannt ist.
    bool HasStatelessAccess (Type functionType);
    // verwendet wenn function l�uft. zb um zurgriffe auf urls (= wxefunction) zu sch�tzen.
    void CheckAccess (WxeFunction function);
  }
}
