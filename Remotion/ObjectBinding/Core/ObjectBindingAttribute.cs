// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
