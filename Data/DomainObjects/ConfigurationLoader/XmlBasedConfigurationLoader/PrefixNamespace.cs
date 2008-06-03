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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader
{
  public class PrefixNamespace
  {
    // types

    // static members and constants

    public static readonly PrefixNamespace QueryConfigurationNamespace = new PrefixNamespace (
        "q", "http://www.re-motion.org/Data/DomainObjects/Queries/1.0");

    // member fields

    private string _prefix;
    private string _uri;

    // construction and disposing

    public PrefixNamespace (string prefix, string uri)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("prefix", prefix);
      ArgumentUtility.CheckNotNullOrEmpty ("uri", uri);

      _prefix = prefix;
      _uri = uri;
    }

    // methods and properties

    public string Prefix
    {
      get { return _prefix; }
    }

    public string Uri
    {
      get { return _uri; }
    }
  }
}
