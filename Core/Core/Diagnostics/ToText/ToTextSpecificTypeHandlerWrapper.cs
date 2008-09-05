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

namespace Remotion.Diagnostics.ToText
{
  public class ToTextSpecificTypeHandlerWrapper<T> : IToTextSpecificTypeHandler
  {
    private readonly Action<T, IToTextBuilderBase> _handler;

    public ToTextSpecificTypeHandlerWrapper (Action<T, IToTextBuilderBase> handler)
    {
      _handler = handler;
    }

    public void ToText (object obj, IToTextBuilderBase toTextBuilder)
    {
      _handler ((T) obj, toTextBuilder);
    }
  }
}