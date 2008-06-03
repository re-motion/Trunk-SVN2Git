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
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList
{

[TestFixture]
public class EditableRowIDProviderTest
{
  // types

  // static members and constants

  // member fields
  
  private EditableRowIDProvider _provider;

  // construction and disposing

  public EditableRowIDProviderTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _provider = new EditableRowIDProvider ("Row{0}");
  }

  [Test]
  public void GetNextID ()
  {
    Assert.AreEqual ("Row0", _provider.GetNextID());
    Assert.AreEqual ("Row1", _provider.GetNextID());
  }

  [Test]
  public void ExcludeIDZero ()
  {
    _provider.ExcludeID ("Row0");

    Assert.AreEqual (new string[] {"Row0"}, _provider.GetExcludedIDs());

    Assert.AreEqual ("Row1", _provider.GetNextID());
    Assert.AreEqual ("Row2", _provider.GetNextID());
  }

  [Test]
  public void ExcludeIDOne ()
  {
    _provider.ExcludeID ("Row1");

    Assert.AreEqual (new string[] {"Row1"}, _provider.GetExcludedIDs());

    Assert.AreEqual ("Row0", _provider.GetNextID());
    Assert.AreEqual ("Row2", _provider.GetNextID());
  }

  [Test]
  public void ExcludeIDIntegration ()
  {
    Assert.AreEqual ("Row0", _provider.GetNextID());
    _provider.ExcludeID ("Row1");
    Assert.AreEqual ("Row2", _provider.GetNextID());
    Assert.AreEqual ("Row3", _provider.GetNextID());
    _provider.ExcludeID ("Row3");
    Assert.AreEqual ("Row4", _provider.GetNextID());
    Assert.AreEqual ("Row5", _provider.GetNextID());
    _provider.ExcludeID ("Row6");
    _provider.ExcludeID ("Row7");
    Assert.AreEqual ("Row8", _provider.GetNextID());

    Assert.AreEqual (new string[] {"Row1", "Row3", "Row6", "Row7"}, _provider.GetExcludedIDs());
  }

  [Test]
  public void Reset ()
  {
    Assert.AreEqual ("Row0", _provider.GetNextID());
    Assert.AreEqual ("Row1", _provider.GetNextID());
    Assert.AreEqual ("Row2", _provider.GetNextID());

    _provider.Reset();

    Assert.AreEqual ("Row0", _provider.GetNextID());
    Assert.AreEqual ("Row1", _provider.GetNextID());
    Assert.AreEqual ("Row2", _provider.GetNextID());
  }

  [Test]
  public void ResetWithExcludedIDs ()
  {
    Assert.AreEqual ("Row0", _provider.GetNextID());
    Assert.AreEqual ("Row1", _provider.GetNextID());
    Assert.AreEqual ("Row2", _provider.GetNextID());
    Assert.AreEqual ("Row3", _provider.GetNextID());
    Assert.AreEqual ("Row4", _provider.GetNextID());
    Assert.AreEqual ("Row5", _provider.GetNextID());
    Assert.AreEqual ("Row6", _provider.GetNextID());
    Assert.AreEqual ("Row7", _provider.GetNextID());
    Assert.AreEqual ("Row8", _provider.GetNextID());

    _provider.ExcludeID ("Row1");
    _provider.ExcludeID ("Row3");
    _provider.ExcludeID ("Row6");
    _provider.ExcludeID ("Row7");

    _provider.Reset();

    Assert.AreEqual ("Row0", _provider.GetNextID());
    Assert.AreEqual ("Row2", _provider.GetNextID());
    Assert.AreEqual ("Row4", _provider.GetNextID());
    Assert.AreEqual ("Row5", _provider.GetNextID());
    Assert.AreEqual ("Row8", _provider.GetNextID());
  }

  [Test]
  public void Serialization ()
  {
    Assert.AreEqual ("Row0", _provider.GetNextID());
    Assert.AreEqual ("Row1", _provider.GetNextID());
    Assert.AreEqual ("Row2", _provider.GetNextID());
    Assert.AreEqual ("Row3", _provider.GetNextID());
    Assert.AreEqual ("Row4", _provider.GetNextID());
    Assert.AreEqual ("Row5", _provider.GetNextID());
    Assert.AreEqual ("Row6", _provider.GetNextID());
    Assert.AreEqual ("Row7", _provider.GetNextID());
    Assert.AreEqual ("Row8", _provider.GetNextID());

    _provider.ExcludeID ("Row1");
    _provider.ExcludeID ("Row3");
    _provider.ExcludeID ("Row6");
    _provider.ExcludeID ("Row7");

    EditableRowIDProvider deserialized = (EditableRowIDProvider) SerializeAndDeserialize (_provider);

    deserialized.Reset();

    Assert.AreEqual ("Row0", deserialized.GetNextID());
    Assert.AreEqual ("Row2", deserialized.GetNextID());
    Assert.AreEqual ("Row4", deserialized.GetNextID());
    Assert.AreEqual ("Row5", deserialized.GetNextID());
    Assert.AreEqual ("Row8", deserialized.GetNextID());
  }

  private object SerializeAndDeserialize (object obj)
  {
    using (MemoryStream stream = new MemoryStream())
    {
      BinaryFormatter formatter = new BinaryFormatter();
      formatter.Serialize (stream, obj);
      stream.Position = 0;
      return formatter.Deserialize (stream);
    }
  }
}

}
