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

namespace Remotion
{
  /// <summary>
  /// Represents a nullable object according to the "Null Object Pattern".
  /// </summary>
  public interface INullObject
  {
    /// <summary>
    /// Gets a value indicating whether the object is a "Null Object".
    /// </summary>
    bool IsNull {get;}
  }
}
