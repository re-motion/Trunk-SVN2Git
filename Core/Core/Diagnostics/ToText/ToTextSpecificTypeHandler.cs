// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using Remotion.Diagnostics.ToText.Infrastructure;

namespace Remotion.Diagnostics.ToText
{
  /// <summary>
  /// Convenience base class for defining externally registered ToText type handlers. For details see <see cref="To"/>-class description.
  /// </summary>
  public abstract class ToTextSpecificTypeHandler<T> : IToTextSpecificTypeHandler
  {
    public abstract void ToText (T accessControlEntry, IToTextBuilder toTextBuilder);

    public virtual void ToText (object obj, IToTextBuilder toTextBuilder)
    {
      ToText ((T) obj, toTextBuilder);
    }
  }
}