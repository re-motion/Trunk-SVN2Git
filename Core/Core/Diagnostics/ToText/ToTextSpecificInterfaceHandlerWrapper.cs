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
using Remotion.Diagnostics.ToText;

namespace Remotion.Diagnostics.ToText
{
  public class ToTextSpecificInterfaceHandlerWrapper<T> : IToTextSpecificInterfaceHandler
  {
    private readonly Action<T, ToTextBuilder> _interfaceHandler;

    public ToTextSpecificInterfaceHandlerWrapper (Action<T, ToTextBuilder> interfaceHandler, int priority)
    {
      _interfaceHandler = interfaceHandler;
      Priority = priority;
    }

    public int Priority
    {
      get; private set;
    }

    public void ToText (object obj, ToTextBuilder toTextBuilder)
    {
      _interfaceHandler ((T) obj, toTextBuilder);
    }
  }
}