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
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata.MetadataObjectTests
{
  [TestFixture]
  public class Test : DomainTest
  {
    private MetadataObject _metadataObject;
    private Culture _cultureInvariant;
    private Culture _cultureDe;
    private Culture _cultureDeAt;
    private Culture _cultureRu;
    private CultureInfo _backupUICulture;
    private CultureInfo _backupCulture;

    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();

      _metadataObject = SecurableClassDefinition.NewObject();
      _metadataObject.Name = "Technical Name";

      _cultureInvariant = Culture.NewObject (string.Empty);
      _cultureDe = Culture.NewObject ("de");
      _cultureDeAt = Culture.NewObject ("de-AT");
      _cultureRu = Culture.NewObject ("ru");

      _backupCulture = Thread.CurrentThread.CurrentCulture;
      _backupUICulture = Thread.CurrentThread.CurrentUICulture;
    }

    public override void TearDown ()
    {
      base.TearDown();

      Thread.CurrentThread.CurrentCulture = _backupCulture;
      Thread.CurrentThread.CurrentUICulture = _backupUICulture;
    }

    [Test]
    public void DisplayName_NeutralCulture ()
    {
      CreateLocalizedName (_metadataObject, _cultureInvariant, "Class Invariant");
      CreateLocalizedName (_metadataObject, _cultureDe, "Class de");
      CreateLocalizedName (_metadataObject, _cultureDeAt, "Class de-AT");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo (_cultureDe.CultureName);

      Assert.AreEqual ("Class de", _metadataObject.DisplayName);
    }

    [Test]
    public void DisplayName_SpecificCulture ()
    {
      CreateLocalizedName (_metadataObject, _cultureInvariant, "Class Invariant");
      CreateLocalizedName (_metadataObject, _cultureDe, "Class de");
      CreateLocalizedName (_metadataObject, _cultureDeAt, "Class de-AT");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo (_cultureDeAt.CultureName);

      Assert.AreEqual ("Class de-AT", _metadataObject.DisplayName);
    }

    [Test]
    public void DisplayName_Invariant ()
    {
      CreateLocalizedName (_metadataObject, _cultureInvariant, "Class Invariant");
      CreateLocalizedName (_metadataObject, _cultureDe, "Class de");
      CreateLocalizedName (_metadataObject, _cultureDeAt, "Class de-AT");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

      Assert.AreEqual ("Class Invariant", _metadataObject.DisplayName);
    }

    [Test]
    public void DisplayName_UnknownCulture ()
    {
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo ("en");

      Assert.AreEqual ("Technical Name", _metadataObject.DisplayName);
    }

    [Test]
    public void DisplayName_FallbackToNeutralCulture ()
    {
      CreateLocalizedName (_metadataObject, _cultureInvariant, "Class Invariant");
      CreateLocalizedName (_metadataObject, _cultureDe, "Class de");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo (_cultureDeAt.CultureName);

      Assert.AreEqual ("Class de", _metadataObject.DisplayName);
    }

    [Test]
    public void DisplayName_FallbackToInvariantCulture ()
    {
      CreateLocalizedName (_metadataObject, _cultureInvariant, "Class Invariant");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo (_cultureDeAt.CultureName);

      Assert.AreEqual ("Class Invariant", _metadataObject.DisplayName);
    }

    [Test]
    public void GetLocalizedName_ExistingCulture ()
    {
      LocalizedName expectedLocalizedName = CreateLocalizedName (_metadataObject, _cultureDe, "Class de");

      Assert.AreSame (expectedLocalizedName, _metadataObject.GetLocalizedName (_cultureDe));
    }

    [Test]
    public void GetLocalizedName_NotExistingLocalizedNameForCulture ()
    {
      Assert.IsNull (_metadataObject.GetLocalizedName (_cultureRu));
    }

    [Test]
    public void GetLocalizedName_ExistingCultureName ()
    {
      LocalizedName expectedLocalizedName = CreateLocalizedName (_metadataObject, _cultureDe, "Class de");

      Assert.AreSame (expectedLocalizedName, _metadataObject.GetLocalizedName ("de"));
    }

    [Test]
    public void GetLocalizedName_NotExistingCultureName ()
    {
      LocalizedName localizedName = _metadataObject.GetLocalizedName ("ru");

      Assert.IsNull (localizedName);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      _metadataObject.Index = 1;
      Assert.AreEqual (1, _metadataObject.Index);
    }

    private LocalizedName CreateLocalizedName (MetadataObject metadataObject, Culture culture, string text)
    {
      return LocalizedName.NewObject (text, culture, metadataObject);
    }
  }
}
