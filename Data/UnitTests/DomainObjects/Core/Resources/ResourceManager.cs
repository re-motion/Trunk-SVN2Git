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
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Utilities;
using Assertion=Remotion.Utilities.Assertion;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Resources
{
  public static class ResourceManager
  {    
    public static byte[] GetResource (string resourceID)
    {
      using (Stream resourceStream = GetResourceStream (resourceID))
      {
        byte[] buffer = new byte[resourceStream.Length];
        resourceStream.Read (buffer, 0, buffer.Length);
        return buffer;
      }
    }

    public static Stream GetResourceStream (string resourceID)
    {
      Type resourceManagerType = typeof (ResourceManager);
      Stream stream = resourceManagerType.Assembly.GetManifestResourceStream (resourceManagerType, resourceID);
      Assertion.IsNotNull (stream, "Resource '{0}.{1}' was not found", resourceManagerType.Namespace, resourceID);

      return stream;
    }

    public static byte[] GetDomainObjectsConfigurationWithFakeMappingLoader ()
    {
      return GetResource ("DomainObjectsConfigurationWithFakeMappingLoader.xml");
    }

    public static byte[] GetDomainObjectsConfigurationWithCustomSectionGroupName ()
    {
      return GetResource ("DomainObjectsConfigurationWithCustomSectionGroupName.xml");
    }

    public static byte[] GetDomainObjectsConfigurationWithMinimumSettings ()
    {
      return GetResource ("DomainObjectsConfigurationWithMinimumSettings.xml");
    }

    public static byte[] GetImage1()
    {
      return GetResource ("Image1.png");
    }

    public static byte[] GetImage2()
    {
      return GetResource ("Image2.png");
    }

    public static byte[] GetImageLarger1MB()
    {
      return GetResource ("ImageLarger1MB.bmp");
    }

    public static void IsEqualToImage1 (byte[] actual)
    {
      IsEqualToImage1 (actual, null);
    }

    public static void IsEqualToImage2 (byte[] actual)
    {
      IsEqualToImage2 (actual, null);
    }

    public static void IsEqualToImageLarger1MB (byte[] actual)
    {
      IsEqualToImageLarger1MB (actual, null);
    }

    public static void IsEmptyImage (byte[] actual)
    {
      IsEmptyImage (actual, null);
    }

    public static void IsEqualToImage1 (byte[] actual, string message)
    {
      AreEqual (GetImage1(), actual, message);
    }

    public static void IsEqualToImage2 (byte[] actual, string message)
    {
      AreEqual (GetImage2(), actual, message);
    }

    public static void IsEqualToImageLarger1MB (byte[] actual, string message)
    {
      AreEqual (GetImageLarger1MB(), actual, message);
    }

    public static void IsEmptyImage (byte[] actual, string message)
    {
      AreEqual (new byte[0], actual, message);
    }

    public static void AreEqual (byte[] expected, byte[] actual)
    {
      AreEqual (expected, actual, null);
    }

    public static void AreEqual (byte[] expected, byte[] actual, string message)
    {
      if (expected == actual)
        return;

      if (expected == null)
        Assert.Fail ("Expected array is null, but actual array is not null.");
      
      Assert.That (actual, Is.EqualTo (expected));
    }
  }
}
