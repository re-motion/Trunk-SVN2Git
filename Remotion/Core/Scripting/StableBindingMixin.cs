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
using System.Runtime.CompilerServices;
using Remotion.Mixins;

namespace Remotion.Scripting
{
  /// <summary>
  /// Mix (shaken not stirred) to your class to get stable binding in DLR scripts 
  /// (see <see cref="ScriptContext"/> and <see cref="StableBindingProxyProvider"/>). 
  /// </summary>
  public class StableBindingMixin : Mixin<object>, IStableBindingMixin
  {
    // GetCustomMember needs to be public.
    [MemberVisibility (MemberVisibility.Public)]
    public object GetCustomMember (string name)
    {
      return ScriptContext.GetAttributeProxy (This, name);
    }    
  }

  public interface IStableBindingMixin
  {
    // SpecialName attribute is copied from interface, not from class method.
    [SpecialName]
    object GetCustomMember (string name);
  }
}