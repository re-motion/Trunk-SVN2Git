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
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Schemas;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Schemas
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
