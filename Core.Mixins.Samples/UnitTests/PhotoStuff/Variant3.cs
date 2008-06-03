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
using Remotion.Mixins;
using Remotion.Mixins.Samples.PhotoStuff;
using Remotion.Mixins.Samples.PhotoStuff.Variant3;
using NUnit.Framework;
using System.Reflection;

namespace Remotion.Mixins.Samples.UnitTests.PhotoStuff
{
  [TestFixture]
  public class Variant3
  {
    [Test]
    public void StoredMembers()
    {
      Photo photo = ObjectFactory.Create<Photo>().With();
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
      Photo photo = ObjectFactory.Create<Photo>(doc).With ();
      Assert.IsNotNull (photo.Document);
      Assert.AreEqual (new DateTime (2006, 01, 01), photo.Document.CreatedAt);
    }
  }
}
