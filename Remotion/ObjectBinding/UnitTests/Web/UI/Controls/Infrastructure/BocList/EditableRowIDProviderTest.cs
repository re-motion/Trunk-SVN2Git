// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;

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
