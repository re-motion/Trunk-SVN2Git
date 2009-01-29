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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Samples.PhotoStuff;
using Remotion.Mixins.Samples.PhotoStuff.Variant3;
using Remotion.Reflection;

namespace Remotion.Mixins.Samples.UnitTests.PhotoStuff
{
  [TestFixture]
  public class Variant3
  {
    [Test]
    public void StoredMembers()
    {
      Photo photo = ObjectFactory.Create<Photo>(ParamList.Empty);
      Assert.IsNotNull (photo.Document);
      PropertyInfo[] properties = Array.FindAll (photo.GetType().GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
          delegate (PropertyInfo pi)
          {
            return pi.IsDefined (typeof (StoredAttribute), false);
          });

      Assert.AreEqual (2, properties.Length);
    }

    [Test]
    public void InitializeWithConcreteDocument()
    {
      Document doc = new Document();
      doc.CreatedAt = new DateTime (2006, 01, 01);
      Photo photo = ObjectFactory.Create<Photo>(ParamList.Empty, doc);
      Assert.IsNotNull (photo.Document);
      Assert.AreEqual (new DateTime (2006, 01, 01), photo.Document.CreatedAt);
    }
  }
}
