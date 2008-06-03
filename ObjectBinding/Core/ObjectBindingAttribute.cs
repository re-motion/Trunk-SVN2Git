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

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Specifies if a property or field is read only.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class ObjectBindingAttribute : Attribute
  {
    private bool _readOnly = false;
    private bool _visible = true;
    public ObjectBindingAttribute ()
    {
    }

    public bool ReadOnly
    {
      get { return _readOnly; }
      set { _readOnly = value; }
    }

    public bool Visible
    {
      get { return _visible; }
      set { _visible = value; }
    }
  }
}
