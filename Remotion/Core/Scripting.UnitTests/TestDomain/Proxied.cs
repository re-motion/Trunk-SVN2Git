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
using System.Linq;

namespace Remotion.Scripting.UnitTests.TestDomain
{
  public class Proxied : IProxiedGetName, ISumMe
  {
    protected string _name;

    public Proxied ()
        : this ("default")
    {
    }

    public Proxied (string name)
    {
      _name = name;
    }

    public string Name
    {
      get { return _name; }
    }

    public string MutableName
    {
      get { return _name; }
      set { _name = value; }
    }

    public string ReadonlyName
    {
      get { return _name; }
    }

    public string WriteonlyName
    {
      set { _name = value; }
    }


    public virtual string NameVirtual
    {
      get { return _name; }
    }

    public virtual string MutableNameVirtual
    {
      get { return _name; }
      set { _name = value; }
    }

    public virtual string ReadonlyNameVirtual
    {
      get { return _name; }
    }

    public virtual string WriteonlyNameVirtual
    {
      set { _name = value; }
    }



    public string GetName ()
    {
      return "Implementer.IProxiedGetName";
    }

    public string Sum (params int[] numbers)
    {
      return Name + ": " + numbers.Sum ();
    }

    public string PrependName (string text)
    {
      return Name + " " + text;
    }

    public virtual string PrependNameVirtual (string text)
    {
      return PrependName (text);
    }

    public override string ToString ()
    {
      return "[Proxied: " + Name + "]";
    }

    public string SumMe (params int[] numbers)
    {
      return Sum (numbers);
    }

    public string GenericToString<T0, T1> (T0 t0, T1 t1)
    {
      return t0.ToString () + t1;
    }

 
    public string OverloadedGenericToString<T0> (T0 t0)
    {
      return t0.ToString ();
    }

    public string OverloadedGenericToString<T0, T1> (T0 t0, T1 t1)
    {
      return t0.ToString () + t1;
    }

    public virtual string OverrideMe (string s)
    {
      return "Proxied: " + s;
    }
  }

  public interface ISumMe
  {
    string SumMe (params int[] numbers);
  }
}