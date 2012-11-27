// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Globalization;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

[TestFixture]
public class StringUtilityTest
{
  private enum TestEnum 
  { 
    Value1 
  }

  private CultureInfo _cultureBackup;
  private CultureInfo _cultureEnUs;
  private CultureInfo _cultureDeDe;

  private readonly Type _int32 = typeof (int);
  private readonly Type _nullableInt32 = typeof (int?);
  private readonly Type _double = typeof (double);
  private readonly Type _string = typeof (string);
  private readonly Type _object = typeof (object);
  private readonly Type _guid = typeof (Guid);
  private readonly Type _nullableGuid = typeof (Guid?);
  private readonly Type _enum = typeof (TestEnum);
  private readonly Type _nullableEnum = typeof (TestEnum?);
  private readonly Type _dbNull = typeof (DBNull);
  private readonly Type _doubleArray = typeof (double[]);
  private readonly Type _stringArray = typeof (string[]);

  [SetUp]
  public void SetUp()
  {
    _cultureEnUs = new CultureInfo ("en-US");
    _cultureDeDe = new CultureInfo ("de-DE");
    
    _cultureBackup = Thread.CurrentThread.CurrentCulture;
    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

    StubStringUtility.ClearCache();
  }

  [TearDown]
  public void TearDown()
  {
    Thread.CurrentThread.CurrentCulture = _cultureBackup;
  }

  [Test]
	public void NullToEmpty()
	{
    Assert.AreEqual (string.Empty, StringUtility.NullToEmpty (null));
    Assert.AreEqual ("1", StringUtility.NullToEmpty ("1"));
	}

  [Test]
  public void IsNullOrEmpty()
  {
    Assert.AreEqual (true, StringUtility.IsNullOrEmpty (null));
    Assert.AreEqual (true, StringUtility.IsNullOrEmpty (string.Empty));
    Assert.AreEqual (false, StringUtility.IsNullOrEmpty (" "));
  }

  [Test]
  public void AreEqual()
  {
    Assert.AreEqual (true, StringUtility.AreEqual ("test1", "test1", false));
    Assert.AreEqual (true, StringUtility.AreEqual ("test1", "test1", true));
    Assert.AreEqual (false, StringUtility.AreEqual ("test1", "TEST1", false));
    Assert.AreEqual (true, StringUtility.AreEqual ("test1", "TEST1", true));
    Assert.AreEqual (false, StringUtility.AreEqual ("täst1", "TÄST1", false));
    Assert.AreEqual (true, StringUtility.AreEqual ("täst1", "TÄST1", true));
  }

  [Test]
  public void CanParseInt32()
  {
    Assert.IsTrue (StringUtility.CanParse (_int32));
  }

  [Test]
  public void CanParseNullableInt32 ()
  {
    Assert.IsTrue (StringUtility.CanParse (_nullableInt32));
  }

  [Test]
  public void CanParseEnum ()
  {
    Assert.IsTrue (StringUtility.CanParse (_enum));
  }

  [Test]
  public void CanParseNullableEnum ()
  {
    Assert.IsTrue (StringUtility.CanParse (_nullableEnum));
  }

  [Test]
  public void GetParseMethodForInt32()
  {
    MethodInfo parseMethod = StubStringUtility.GetParseMethod (_int32, true);
    Assert.IsNotNull (parseMethod);
    Assert.AreEqual ("Parse", parseMethod.Name);
    Assert.AreEqual (2, parseMethod.GetParameters().Length);
    Assert.AreEqual (typeof (string), parseMethod.GetParameters()[0].ParameterType);
    Assert.AreEqual (typeof (IFormatProvider), parseMethod.GetParameters()[1].ParameterType);
    Assert.AreEqual (typeof (int), parseMethod.ReturnType);
    Assert.IsTrue (parseMethod.IsPublic);
    Assert.IsTrue (parseMethod.IsStatic);
  }

  [Test]
  public void GetParseMethodFromTypeForInt32()
  {
    MethodInfo parseMethod = StubStringUtility.GetParseMethodFromType (_int32);
    Assert.IsNotNull (parseMethod);
    Assert.AreEqual ("Parse", parseMethod.Name);
    Assert.AreEqual (1, parseMethod.GetParameters().Length);
    Assert.AreEqual (typeof (string), parseMethod.GetParameters()[0].ParameterType);
    Assert.AreEqual (typeof (int), parseMethod.ReturnType);
    Assert.IsTrue (parseMethod.IsPublic);
    Assert.IsTrue (parseMethod.IsStatic);
  }

  [Test]
  public void GetParseMethodWithFormatProviderFromTypeForInt32()
  {
    MethodInfo parseMethod = StubStringUtility.GetParseMethodWithFormatProviderFromType (_int32);
    Assert.IsNotNull (parseMethod);
    Assert.AreEqual ("Parse", parseMethod.Name);
    Assert.AreEqual (2, parseMethod.GetParameters().Length);
    Assert.AreEqual (typeof (string), parseMethod.GetParameters()[0].ParameterType);
    Assert.AreEqual (typeof (IFormatProvider), parseMethod.GetParameters()[1].ParameterType);
    Assert.AreEqual (typeof (int), parseMethod.ReturnType);
    Assert.IsTrue (parseMethod.IsPublic);
    Assert.IsTrue (parseMethod.IsStatic);
  }

  [Test]
  public void CanParseObject()
  {
    Assert.IsFalse (StringUtility.CanParse (_object));
  }
  
  [Test]
  [ExpectedException (typeof (ParseException))]
  public void GetParseMethodForObjectWithException()
  {
    StubStringUtility.GetParseMethod (_object, true);
  }

  [Test]
  public void GetParseMethodForObjectWithoutException()
  {
    Assert.IsNull (StubStringUtility.GetParseMethod (_object, false));
  }

  [Test]
  public void GetParseMethodFromTypeForObject()
  {
    Assert.IsNull (StubStringUtility.GetParseMethodFromType (_object));
  }

  [Test]
  public void GetParseMethodWithFormatProviderFromTypeForObject()
  {
    Assert.IsNull (StubStringUtility.GetParseMethodWithFormatProviderFromType (_object));
  }

  [Test]
  public void ParseInt32 ()
  {
    object value = StringUtility.Parse (_int32, "1", CultureInfo.InvariantCulture);
    Assert.IsNotNull (value);
    Assert.AreEqual (_int32, value.GetType ());
    Assert.AreEqual (1, value);
  }

  [Test]
  [ExpectedException (typeof (ParseException))]
  public void ParseInt32WithEmpty ()
  {
    StringUtility.Parse (_int32, string.Empty, CultureInfo.InvariantCulture);
  }

  [Test]
  [ExpectedException (typeof (ParseException))]
  public void ParseInt32WithNull ()
  {
    StringUtility.Parse (_int32, null, CultureInfo.InvariantCulture);
  }

  [Test]
  public void ParseNullableInt32 ()
  {
    object value = StringUtility.Parse (_nullableInt32, "1", CultureInfo.InvariantCulture);
    Assert.IsNotNull (value);
    Assert.AreEqual (_int32, value.GetType ());
    Assert.AreEqual (1, value);
  }

  [Test]
  public void ParseNullableInt32WithEmpty ()
  {
    Assert.IsNull (StringUtility.Parse (_nullableInt32, string.Empty, CultureInfo.InvariantCulture));
  }

  [Test]
  public void ParseNullableInt32WithNull ()
  {
    Assert.IsNull (StringUtility.Parse (_nullableInt32, null, CultureInfo.InvariantCulture));
  }

  [Test]
  public void ParseEnum ()
  {
    object value = StringUtility.Parse (_enum, "Value1", CultureInfo.InvariantCulture);
    Assert.IsNotNull (value);
    Assert.AreEqual (_enum, value.GetType ());
    Assert.AreEqual (TestEnum.Value1, value);
  }

  [Test]
  [ExpectedException (typeof (ParseException), ExpectedMessage = " is not a valid value for TestEnum.")]
  public void ParseEnumWithEmpty ()
  {
    StringUtility.Parse (_enum, string.Empty, CultureInfo.InvariantCulture);
  }

  [Test]
  [ExpectedException (typeof (ParseException), ExpectedMessage = " is not a valid value for TestEnum.")]
  public void ParseEnumWithNull ()
  {
    StringUtility.Parse (_enum, null, CultureInfo.InvariantCulture);
  }

  [Test]
  public void ParseNullableEnum ()
  {
    object value = StringUtility.Parse (_nullableEnum, "Value1", CultureInfo.InvariantCulture);
    Assert.IsNotNull (value);
    Assert.AreEqual (_enum, value.GetType ());
    Assert.AreEqual (TestEnum.Value1, value);
  }

  [Test]
  public void ParseNullableEnumWithEmpty ()
  {
    Assert.IsNull (StringUtility.Parse (_nullableEnum, string.Empty, CultureInfo.InvariantCulture));
  }

  [Test]
  public void ParseNullableEnumWithNull ()
  {
    Assert.IsNull (StringUtility.Parse (_nullableEnum, null, CultureInfo.InvariantCulture));
  }

  [Test]
  public void ParseDoubleWithCultureInvariant()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    object value = StringUtility.Parse (_double, "4,321.123", CultureInfo.InvariantCulture);
    Assert.IsNotNull (value);
    Assert.AreEqual (_double, value.GetType());
    Assert.AreEqual (4321.123, value);
  }

  [Test]
  public void ParseDoubleWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    object value = StringUtility.Parse (_double, "4,321.123", _cultureEnUs);
    Assert.IsNotNull (value);
    Assert.AreEqual (_double, value.GetType());
    Assert.AreEqual (4321.123, value);
  }

  [Test]
  [ExpectedException (typeof (ParseException))]
  public void ParseDoubleEnUsWithCultureDeDe()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    StringUtility.Parse (_double, "4,321.123", _cultureDeDe);
  }

  [Test]
  public void ParseDoubleWithCultureDeDe()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    object value = StringUtility.Parse (_double, "4.321,123", _cultureDeDe);
    Assert.IsNotNull (value);
    Assert.AreEqual (_double, value.GetType());
    Assert.AreEqual (4321.123, value);
  }

  [Test]
  [ExpectedException (typeof (ParseException))]
  public void ParseDoubleDeDeWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    StringUtility.Parse (_double, "4.321,123", _cultureEnUs);
  }

  [Test]
  [Ignore (@"Bug in ParseArrayItem: Escape Sequence '\,' does not work.")]
  public void ParseDoubleArrayWithCultureInvariant()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    object value = StringUtility.Parse (_doubleArray, @"6\,543.123,5\,432.123,4\,321.123", CultureInfo.InvariantCulture);
    Assert.IsNotNull (value);
    Assert.AreEqual (_doubleArray, value.GetType());
    double[] values = (double[]) value;
    Assert.AreEqual (3, values.Length);
    Assert.AreEqual (6543.123, values[0]);
    Assert.AreEqual (5432.123, values[1]);
    Assert.AreEqual (4321.123, values[2]);
  }

  [Test]
  public void ParseDoubleArrayWithCultureInvariantNoThousands()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    object value = StringUtility.Parse (_doubleArray, @"6543.123,5432.123,4321.123", CultureInfo.InvariantCulture);
    Assert.IsNotNull (value);
    Assert.AreEqual (_doubleArray, value.GetType());
    double[] values = (double[]) value;
    Assert.AreEqual (3, values.Length);
    Assert.AreEqual (6543.123, values[0]);
    Assert.AreEqual (5432.123, values[1]);
    Assert.AreEqual (4321.123, values[2]);
  }

  [Test]
  [Ignore (@"Bug in ParseArrayItem: Escape Sequence '\,' does not work.")]
  public void ParseDoubleArrayWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    object value = StringUtility.Parse (_doubleArray, @"6\,543.123,5\,432.123,4\,321.123", _cultureEnUs);
    Assert.IsNotNull (value);
    Assert.AreEqual (_doubleArray, value.GetType());
    double[] values = (double[]) value;
    Assert.AreEqual (3, values.Length);
    Assert.AreEqual (6543.123, values[0]);
    Assert.AreEqual (5432.123, values[1]);
    Assert.AreEqual (4321.123, values[2]);
  }

  [Test]
  public void ParseDoubleArrayWithCultureEnUsNoThousands()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    object value = StringUtility.Parse (_doubleArray, @"6543.123,5432.123,4321.123", _cultureEnUs);
    Assert.IsNotNull (value);
    Assert.AreEqual (_doubleArray, value.GetType());
    double[] values = (double[]) value;
    Assert.AreEqual (3, values.Length);
    Assert.AreEqual (6543.123, values[0]);
    Assert.AreEqual (5432.123, values[1]);
    Assert.AreEqual (4321.123, values[2]);
  }

  [Test]
  [Ignore (@"Bug in ParseArrayItem: Escape Sequence '\,' does not work.")]
  public void ParseDoubleArrayWithCultureDeDe()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    object value = StringUtility.Parse (_doubleArray, @"6.543\,123,5.432\,123,4.321\,123", _cultureDeDe);
    Assert.IsNotNull (value);
    Assert.AreEqual (_doubleArray, value.GetType());
    double[] values = (double[]) value;
    Assert.AreEqual (3, values.Length);
    Assert.AreEqual (6543.123, values[0]);
    Assert.AreEqual (5432.123, values[1]);
    Assert.AreEqual (4321.123, values[2]);
  }

  [Test]
  public void ParseStringArray()
  {
    object value = StringUtility.Parse (_stringArray, "\"a\",\"b\",\"c\",\"d\"", null);
    Assert.IsNotNull (value);
    Assert.AreEqual (_stringArray, value.GetType());
    string[] values = (string[]) value;
    Assert.AreEqual (4, values.Length);
    Assert.AreEqual ("a", values[0]);
    Assert.AreEqual ("b", values[1]);
    Assert.AreEqual ("c", values[2]);
    Assert.AreEqual ("d", values[3]);
  }

  [Test]
  [ExpectedException (typeof (ParseException))]
  public void ParseArrayOfDoubleArrays()
  {
    StringUtility.Parse (typeof (double[][]), "1,2,3", null);
  }

  [Test]
  public void CanParseDoubleArray()
  {
    Assert.IsTrue (StringUtility.CanParse (_doubleArray));
  }

  [Test]
  public void CanParseArrayDoubleArray()
  {
    Assert.IsFalse (StringUtility.CanParse (typeof (double[][])));
  }

  [Test]
  public void CanParseString()
  {
    Assert.IsTrue (StringUtility.CanParse (_string));
  }

  [Test]
  public void CanParseDBNull()
  {
    Assert.IsTrue (StringUtility.CanParse (_dbNull));
  }

  [Test]
  public void ParseDBNull()
  {
    object value = StringUtility.Parse (_dbNull, DBNull.Value.ToString(), null);
    Assert.IsNotNull (value);
    Assert.AreEqual (_dbNull, value.GetType());
    Assert.AreEqual (DBNull.Value, value);
  }

  [Test]
  public void CanParseGuid()
  {
    Assert.IsTrue (StringUtility.CanParse (_guid));
  }

  [Test]
  public void ParseGuid()
  {
    Guid guid = Guid.NewGuid();
    object value = StringUtility.Parse (_guid, guid.ToString(), null);
    Assert.IsNotNull (value);
    Assert.AreEqual (_guid, value.GetType());
    Assert.AreEqual (guid, value);
  }

  [Test]
  [ExpectedException (typeof (FormatException))]
  public void ParseGuidWithEmpty ()
  {
    StringUtility.Parse (_guid, string.Empty, CultureInfo.InvariantCulture);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ParseGuidWithNull ()
  {
    StringUtility.Parse (_guid, null, CultureInfo.InvariantCulture);
  }

  [Test]
  public void ParseEmptyGuid()
  {
    Guid guid = Guid.Empty;
    object value = StringUtility.Parse (_guid, guid.ToString(), null);
    Assert.IsNotNull (value);
    Assert.AreEqual (_guid, value.GetType());
    Assert.AreEqual (guid, value);
  }

  [Test]
  public void ParseNullableGuid ()
  {
    Guid? guid = Guid.NewGuid ();
    object value = StringUtility.Parse (_nullableGuid, guid.ToString (), null);
    Assert.IsNotNull (value);
    Assert.AreEqual (_guid, value.GetType ());
    Assert.AreEqual (guid, value);
  }

  [Test]
  public void ParseNullableGuidWithEmpty ()
  {
    Assert.IsNull (StringUtility.Parse (_nullableGuid, string.Empty, null));
  }

  [Test]
  public void ParseNullableGuidWithNull ()
  {
    Assert.IsNull (StringUtility.Parse (_nullableGuid, null, null));
  }
  [Test]
  public void FormatNull()
  {
    Assert.AreEqual (string.Empty, StringUtility.Format (null, null));
  }

  [Test]
  public void FormatString()
  {
    const string value = "Hello World!";
    Assert.AreEqual (value, StringUtility.Format (value, null));
  }

  [Test]
  public void FormatDoubleWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    Assert.AreEqual ("4321.123", StringUtility.Format (4321.123, _cultureEnUs));
  }

  [Test]
  public void FormatDoubleWithCultureDeDe()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    Assert.AreEqual ("4321,123", StringUtility.Format (4321.123, _cultureDeDe));
  }

  [Test]
  public void FormatGuid()
  {
    Guid guid = Guid.Empty;
    Assert.AreEqual (guid.ToString(), StringUtility.Format (guid, null));
  }

  [Test]
  public void FormatDoubleArrayWithCultureEnUsNoThousands()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    double[] values = new double[] {6543.123, 5432.123, 4321.123};
    Assert.AreEqual (@"6543.123,5432.123,4321.123", StringUtility.Format (values, _cultureEnUs));
  }


  [Test]
  public void GetFileNameTimestampTest ()
  {
    var dt = new DateTime (2008, 12, 24, 23, 59, 59, 999);
    string result = StringUtility.GetFileNameTimestamp (dt);
    Assert.That (result, Is.EqualTo("2008_12_24__23_59_59_999"));
  }

  [Test]
  public void GetFileNameTimestampNowTest ()
  {
    string result = StringUtility.GetFileNameTimestampNow ();
    DateTime dateTimeNow = DateTime.Now;
    DateTime dateTimeResult = DateTime.ParseExact (result, "yyyy_M_d__H_m_s_FFF", CultureInfo.InvariantCulture.NumberFormat);
    Assert.That (dateTimeNow - dateTimeResult, Is.LessThanOrEqualTo (new TimeSpan(0,0,0,0,50)));
  }

}

[TestFixture]
public class StringUtility_ParseSeparatedListTest
{
  [Test]
  public void TestParseSeparatedList()
  {
    // char doubleq = '\"';
    const char singleq = '\'';
    const char backsl = '\\';
    const char comma = ',';
    const string whitespace = " ";

    Check ("1", comma, singleq, singleq, backsl, whitespace, true,
           unquoted ("1"));
    Check ("1,2", comma, singleq, singleq, backsl, whitespace, true,
           unquoted ("1"), unquoted ("2"));
    Check ("'1', '2'", comma, singleq, singleq, backsl, whitespace, true,
           quoted ("1"), quoted ("2"));
    Check ("<1>, <2>", comma, '<', '>', backsl, whitespace, true,
           quoted ("1"), quoted ("2"));
    Check ("a='A', b='B'", comma, singleq, singleq, backsl, whitespace, true,
           unquoted ("a='A'"), unquoted ("b='B'"));
    Check ("a='A', b='B,B\\'B\\''", comma, singleq, singleq, backsl, whitespace, true,
           unquoted ("a='A'"), unquoted ("b='B,B'B''"));
    Check ("a b c = 'd,e' f 'g,h'", comma, singleq, singleq, backsl, whitespace, true,
           unquoted ("a b c = 'd,e' f 'g,h'"));
    Check ("a <a ,<a,> a, <b", comma, '<', '>', backsl, whitespace, true,
           unquoted ("a <a ,<a,> a"), quoted ("b"));
  }

  private StringUtility.ParsedItem quoted (string value)
  {
    return new StringUtility.ParsedItem (value, true);
  }

  private StringUtility.ParsedItem unquoted (string value)
  {
    return new StringUtility.ParsedItem (value, false);
  }

  private void Check (
      string value,
      char delimiter, char openingQuote, char closingQuote, char escapingChar, string whitespaceCharacters, 
      bool interpretSpecialCharacters,
      params StringUtility.ParsedItem[] expectedItems)
  {
    StringUtility.ParsedItem[] actualItems = StringUtility.ParseSeparatedList (
      value, delimiter, openingQuote, closingQuote, escapingChar, whitespaceCharacters, interpretSpecialCharacters);

    Assert.AreEqual (expectedItems.Length, actualItems.Length);
    for (int i = 0; i < expectedItems.Length; ++i)
    {
      Assert.AreEqual (expectedItems[i].Value, actualItems[i].Value, string.Format ("[{0}].Value", i));
      Assert.AreEqual (expectedItems[i].IsQuoted, actualItems[i].IsQuoted, string.Format ("[{0}].IsQuoted", i));
    }
  }
}

}
