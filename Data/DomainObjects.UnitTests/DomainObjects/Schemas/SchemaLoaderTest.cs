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
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Schemas;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Schemas
{
  [TestFixture]
  public class SchemaLoaderTest
  {
    [Test]
    public void InitializeWithQueries ()
    {
      Assert.AreEqual (PrefixNamespace.QueryConfigurationNamespace.Uri, SchemaLoader.Queries.SchemaUri);
    }

    [Test]
    public void LoadSchemaSetWithQueries ()
    {
      XmlSchemaSet schemaSet = SchemaLoader.Queries.LoadSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (PrefixNamespace.QueryConfigurationNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.re-motion.org/Data/DomainObjects/Types"));
    }
  }
}
