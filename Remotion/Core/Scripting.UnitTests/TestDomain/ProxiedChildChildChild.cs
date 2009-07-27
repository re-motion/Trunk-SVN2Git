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
namespace Remotion.Scripting.UnitTests.TestDomain
{
  public class ProxiedChildChildChild : ProxiedChildChild
  {
    public ProxiedChildChildChild (string name)
      : base (name)
    {
    }

    public new string ProcessText (string s)
    {
      return "ProxiedChildChildChild: " + s.ToUpper().Replace("holla","die waldfee");
    }

    public new string PrependName (string text)
    {
      return "ProxiedChildChildChild " + Name + " " + text.ToUpper() + " " + text.ToLower();
    }

    public new string PrependName (string text, int number)
    {
      return "ProxiedChildChildChild " + Name + " " + text.ToUpper () + " " + text.ToLower () + ", number=" + number;
    }    

    public override string PrependNameVirtual (string text)
    {
      return PrependName (text);
    }

    public override string VirtualMethodNotInBaseType (string text)
    {
      return "ProxiedChildChildChild::VirtualMethodNotInBaseType " + Name + " " + text.ToUpper () + " " + text.ToLower ();
    }
  }
}