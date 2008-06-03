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
using NUnit.Framework;

namespace Remotion.Web.UnitTests.UI.Controls
{

[TestFixture]
public class EmailAddressValidatorTest
{
  private EmailAddressValidatorMock _validator;

  [SetUp]
  public virtual void SetUp()
  {
    _validator = new EmailAddressValidatorMock();
  }

  #region public void MatchValidEmailAddress*

	[Test]
  public void MatchValidEmailAddress()
  {
    string text = @"jdoe@provider.net";
    bool result = _validator.IsMatchComplete (text);
    Assert.AreEqual (true, result);
  }

  #endregion

  #region public void MatchValidUserPart*

	[Test]
  public void MatchValidUserPartSingleCharacter()
  {
    string text = @"j";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartAllLowerCase()
  {
    string text = @"jdoe";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithLeadingCapitalCharacter()
  {
    string text = @"Jdoe";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithMiddleCapitalCharacter()
  {
    string text = @"jDoe";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithTrailingCapitalCharacter()
  {
    string text = @"jdoE";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithLeadingDigit()
  {
    string text = @"2jdoe";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithMiddleDigit()
  {
    string text = @"j2doe";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithTrailingDigit()
  {
    string text = @"jdoe2";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithLeadingUnderscore()
  {
    string text = @"_jdoe";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithMiddleUnderscore()
  {
    string text = @"j_doe";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithTrailingUnderscore()
  {
    string text = @"jdoe_";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithLeadingUmlaut()
  {
    string text = @"äjdoe";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithMiddleUmlaut()
  {
    string text = @"jädoe";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithTrailingUmlaut()
  {
    string text = @"jdoeä";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithHyphen()
  {
    string text = @"j-doe";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidUserPartWithDot()
  {
    string text = @"j.doe";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (true, result);
  }

  #endregion

  #region public void MatchValidDomainPart*

	[Test]
  public void MatchValidDomainPartTwoCharacters()
  {
    string text = @"pr.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartAllLowerCase()
  {
    string text = @"provider.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithLeadingCapitalCharacter()
  {
    string text = @"Provider.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithMiddleCapitalCharacter()
  {
    string text = @"proVider.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithTrailingCapitalCharacter()
  {
    string text = @"provideR.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithLeadingDigit()
  {
    string text = @"2provider.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithMiddleDigit()
  {
    string text = @"pro2vider.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithTrailingDigit()
  {
    string text = @"provider2.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithLeadingUnderscore()
  {
    string text = @"_provider.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithMiddleUnderscore()
  {
    string text = @"pro_vider.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithTrailingUnderscore()
  {
    string text = @"provider_.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithLeadingUmlaut()
  {
    string text = @"äprovider.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithMiddleUmlaut()
  {
    string text = @"proävider.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithTrailingUmlaut()
  {
    string text = @"providerä.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithHyphen()
  {
    string text = @"pro-vider.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithDot()
  {
    string text = @"pro.vider.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithTwoCharacterRoot()
  {
    string text = @"provider.ab";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

	[Test]
  public void MatchValidDomainPartWithNineCharacterRoot()
  {
    string text = @"provider.abcdefghi";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (true, result);
  }

  #endregion

  #region public void MatchInvalidEmailAddress*

	[Test]
  public void MatchInvalidEmailAddressNoAtCharacter()
  {
    string text = @"jdoeprovider.net";
    bool result = _validator.IsMatchComplete (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidEmailAddressTwoAtCharacter()
  {
    string text = @"jdoe@pro@vider.net";
    bool result = _validator.IsMatchComplete (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidEmailAddressNoCharacters()
  {
    string text = @"";
    bool result = _validator.IsMatchComplete (text);
    Assert.AreEqual (false, result);
  }

  #endregion

  #region public void MatchInvalidUserPart*

	[Test]
  public void MatchInvalidUserPartWithNoCharacters()
  {
    string text = @"";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidUserPartWithLeadingWhitespace()
  {
    string text = @" jdoe";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidUserPartWithTrailingWhitespace()
  {
    string text = @"jdoe ";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidUserPartWithLeadingHyphen()
  {
    string text = @"-jdoe";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidUserPartWithTrailingHyphen()
  {
    string text = @"jdoe-";
    bool result = _validator.IsMatchUserPart (text);
    Assert.AreEqual (false, result);
  }

  #endregion

  #region public void MatchInvalidDomainPart*

	[Test]
  public void MatchInvalidDomainPartWithNoCharacters()
  {
    string text = @"";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidDomainPartSingleCharacter()
  {
    string text = @"p.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidDomainPartWithLeadingWhitespace()
  {
    string text = @" provider.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidDomainPartWithTrailingWhitespace()
  {
    string text = @"provider.net ";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidDomainPartWithLeadingHyphen()
  {
    string text = @"-provider.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidDomainPartWithTrailingHyphen()
  {
    string text = @"provider-.net";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidDomainPartWithMissingTld()
  {
    string text = @"provider.";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidDomainPartWithMissingTldAndDot()
  {
    string text = @"provider";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidDomainPartWithSingleCharacterTld()
  {
    string text = @"provider.a";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (false, result);
  }

	[Test]
  public void MatchInvalidDomainPartWithTenCharacterTld()
  {
    string text = @"provider.abcdefghij";
    bool result = _validator.IsMatchDomainPart (text);
    Assert.AreEqual (false, result);
  }

  #endregion
}

}
