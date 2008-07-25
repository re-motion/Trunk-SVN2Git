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
using Remotion.Implementation;

namespace Remotion.Mixins
{
  /// <summary>
  /// When applied to a mixin, specifies that this mixin does not introduce a specific interface or attribute to the target class.
  /// </summary>
  /// <remarks>Use this attribute if a mixin should implement an interface or declare an attribute "just for itself" and the interface should not be
  /// forwarded to the target class.</remarks>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public class NonIntroducedAttribute : Attribute
  {
    private readonly Type _nonIntroducedType;

    public NonIntroducedAttribute (Type nonIntroducedType)
    {
      _nonIntroducedType = ArgumentUtility.CheckNotNull ("nonIntroducedType", nonIntroducedType);
    }

    public Type NonIntroducedType
    {
      get { return _nonIntroducedType; }
    }
  }
}
