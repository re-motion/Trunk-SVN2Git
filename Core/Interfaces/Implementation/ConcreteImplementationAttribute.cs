// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Text;
using Remotion.Reflection;

namespace Remotion.Implementation
{
  [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
  public class ConcreteImplementationAttribute : Attribute
  {
    private readonly string _typeNameTemplate;

    public ConcreteImplementationAttribute (string typeNameTemplate)
    {
      _typeNameTemplate = ArgumentUtility.CheckNotNull ("typeNameTemplate", typeNameTemplate);
    }

    public string TypeNameTemplate
    {
      get { return _typeNameTemplate; }
    }

    public string GetTypeName()
    {
      string versioned = _typeNameTemplate.Replace ("<version>", FrameworkVersion.Value.ToString());
      return versioned.Replace ("<publicKeyToken>", GetPublicKeyTokenString());
    }

    private string GetPublicKeyTokenString ()
    {
      byte[] bytes = typeof (ConcreteImplementationAttribute).Assembly.GetName().GetPublicKeyToken();
      return string.Format ("{0:x2}{1:x2}{2:x2}{3:x2}{4:x2}{5:x2}{6:x2}{7:x2}",
          bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]);
    }

    public Type ResolveType ()
    {
      return ContextAwareTypeDiscoveryUtility.GetType (GetTypeName (), true);
    }

    public object InstantiateType ()
    {
      return Activator.CreateInstance (ResolveType());
    }
  }
}
