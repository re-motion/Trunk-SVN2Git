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
