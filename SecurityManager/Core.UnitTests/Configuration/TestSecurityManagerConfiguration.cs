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
using System.IO;
using System.Xml;
using Remotion.SecurityManager.Configuration;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Configuration
{
  internal class TestSecurityManagerConfiguration : SecurityManagerConfiguration
  {
    public void DeserializeSection (string xmlFragment)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("xmlFragment", xmlFragment);

      XmlDocument document = new XmlDocument ();
      document.LoadXml (xmlFragment);

      using (MemoryStream stream = new MemoryStream ())
      {
        document.Save (stream);
        stream.Position = 0;
        using (XmlReader reader = XmlReader.Create (stream))
        {
          DeserializeSection (reader);
        }
      }
    }
  }
}
