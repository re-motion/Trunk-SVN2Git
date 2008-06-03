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
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine.UrlMapping;

namespace Remotion.Web.UnitTests.ExecutionEngine.UrlMapping
{

[TestFixture]
public class UrlMappingSchemaTest
{
  [SetUp]
  public virtual void SetUp()
  {
  }

  [TearDown]
  public virtual void TearDown()
  {
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaValidationException))]
  public void LoadMappingWithMissingPath()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithMissingPath.xml");
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaValidationException))]
  public void LoadMappingWithEmptyPath()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithEmptyPath.xml");
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaValidationException))]
  public void LoadMappingWithMissingFunctionType()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithMissingFunctionType.xml");
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaValidationException))]
  public void LoadMappingWithEmptyFunctionType()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithEmptyFunctionType.xml");
  }

  [Test]
  [ExpectedException (typeof (XmlException))]
  public void LoadMappingWithFunctionTypeHavingNoAssembly()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithFunctionTypeHavingNoAssembly.xml");
  }

  [Test]
  public void LoadSchemaSet ()
  {
    UrlMappingSchema urlMappingSchema = new UrlMappingSchema ();
    XmlSchemaSet xmlSchemaSet = urlMappingSchema.LoadSchemaSet ();
    Assert.AreEqual (1, xmlSchemaSet.Count);
    Assert.IsTrue (xmlSchemaSet.Contains (urlMappingSchema.SchemaUri));
  }

}

}
