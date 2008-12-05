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
using System.Collections.Specialized;
using System.Text;
using System.Web;
using NUnit.Framework;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Utilities
{

[TestFixture]
public class UrlUtilityTest
{
  private Encoding _currentEncoding;

  [SetUp]
  public virtual void SetUp()
  {
    _currentEncoding = System.Text.Encoding.UTF8;
  }


  [Test]
  public void AddParameterToEmptyUrl()
  {
    string url = string.Empty;
    string parameter = "Parameter1";
    string value = "Value1ä#";

    string expected = string.Format (
        "{0}?{1}={2}", 
        url,
        parameter, 
        HttpUtility.UrlEncode (value, _currentEncoding));

    string actual = UrlUtility.AddParameter (url, parameter, value, _currentEncoding);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void AddParameterToUrl()
  {
    string url = "http://localhost/Default.html";
    string parameter = "Parameter1";
    string value = "Value1ä#";

    string expected = string.Format (
        "{0}?{1}={2}", 
        url,
        parameter, 
        HttpUtility.UrlEncode (value, _currentEncoding));

    string actual = UrlUtility.AddParameter (url, parameter, value, _currentEncoding);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void AddParameterToUrlWithExistingQueryString()
  {
    string url = "http://localhost/Default.html?Parameter2=Value2";
    string parameter = "Parameter1";
    string value = "Value1ä#";

    string expected = string.Format (
        "{0}&{1}={2}", 
        url,
        parameter, 
        HttpUtility.UrlEncode (value, _currentEncoding));

    string actual = UrlUtility.AddParameter (url, parameter, value, _currentEncoding);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void AddParameterToUrlWithQuestionmark()
  {
    string url = "http://localhost/Default.html?";
    string parameter = "Parameter1";
    string value = "Value1ä#";

    string expected = string.Format (
        "{0}{1}={2}", 
        url,
        parameter, 
        HttpUtility.UrlEncode (value, _currentEncoding));

    string actual = UrlUtility.AddParameter (url, parameter, value, _currentEncoding);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void AddParameterToUrlWithAmpersand()
  {
    string url = "http://localhost/Default.html?Parameter2=Value2&";
    string parameter = "Parameter1";
    string value = "Value1ä#";

    string expected = string.Format (
        "{0}{1}={2}", 
        url,
        parameter, 
        HttpUtility.UrlEncode (value, _currentEncoding));

    string actual = UrlUtility.AddParameter (url, parameter, value, _currentEncoding);
    Assert.AreEqual (expected, actual);
  }


  [Test]
  public void AddParametersToUrl()
  {
    string url = "http://localhost/Default.html";
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1ä#";
    string value2 = "Value2";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameter1, value1);
    queryString.Add (parameter2, value2);

    string expected = string.Format (
        "{0}?{1}={2}&{3}={4}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string actual = UrlUtility.AddParameters (url, queryString, _currentEncoding);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void AddParametersToUrlNoParameters()
  {
    string url = "http://localhost/Default.html";

    NameValueCollection queryString = new NameValueCollection();
    string expected = url;

    string actual = UrlUtility.AddParameters (url, queryString, _currentEncoding);
    Assert.AreEqual (expected, actual);
  }


  [Test]
  public void FormatQueryString()
  {
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1ä#";
    string value2 = "Value2";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameter1, value1);
    queryString.Add (parameter2, value2);

    string expected = string.Format (
        "?{0}={1}&{2}={3}", 
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string actual = UrlUtility.FormatQueryString (queryString, _currentEncoding);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void FormatQueryStringNoParameters()
  {
    NameValueCollection queryString = new NameValueCollection();
    string expected = string.Empty;

    string actual = UrlUtility.FormatQueryString (queryString, _currentEncoding);
    Assert.AreEqual (expected, actual);
  }


  [Test]
  public void DeleteParameterFromUrlWithLastParameter ()
  {
    string url = "http://localhost/Default.html";
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1ä#";
    string value2 = "Value2";

    string original = string.Format (
        "{0}?{1}={2}&{3}={4}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string expected = string.Format (
        "{0}?{1}={2}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string actual = UrlUtility.DeleteParameter (original, parameter2);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void DeleteParameterFromUrlWithFirstParameter ()
  {
    string url = "http://localhost/Default.html";
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1ä#";
    string value2 = "Value2";

    string original = string.Format (
        "{0}?{1}={2}&{3}={4}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string expected = string.Format (
        "{0}?{3}={4}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string actual = UrlUtility.DeleteParameter (original, parameter1);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void DeleteParameterFromUrlWithParameterNamePartOfUrl ()
  {
    string url = "http://localhost/Parameter1.html";
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1ä#";
    string value2 = "Value2";

    string original = string.Format (
        "{0}?{1}={2}&{3}={4}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string expected = string.Format (
        "{0}?{3}={4}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string actual = UrlUtility.DeleteParameter (original, parameter1);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void DeleteParameterFromUrlWithParameterNamePartOfOtherParameter ()
  {
    string url = "http://localhost/Default.html";
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1ä#Parameter2";
    string value2 = "Value2";

    string original = string.Format (
        "{0}?{1}={2}&{3}={4}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string expected = string.Format (
        "{0}?{1}={2}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string actual = UrlUtility.DeleteParameter (original, parameter2);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void DeleteParameterFromUrlWithSingleParameter ()
  {
    string url = "http://localhost/Default.html";
    string parameter1 = "Parameter1";
    string value1 = "Value1ä#";

    string original = string.Format (
        "{0}?{1}={2}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding));

    string expected = url;

    string actual = UrlUtility.DeleteParameter (original, parameter1);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void DeleteParameterFromUrlWithNoUrl ()
  {
    string parameter1 = "Parameter1";
    string value1 = "Value1ä#";

    string original = string.Format (
        "?{0}={1}", 
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding));

    string expected = string.Empty;

    string actual = UrlUtility.DeleteParameter (original, parameter1);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void DeleteParameterFromUrlWithUnknownParameter ()
  {
    string url = "http://localhost/Default.html";
    string parameter1 = "Parameter1";
    string value1 = "Value1ä#";
    string parameter2 = "Parameter2";

    string original = string.Format (
        "{0}?{1}={2}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding));

    string expected = original;

    string actual = UrlUtility.DeleteParameter (original, parameter2);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void DeleteParameterFromUrlWithEmptyLastParameter ()
  {
    string url = "http://localhost/Default.html";
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1ä#";
    string value2 = "Value2";

    string original = string.Format (
        "{0}?{1}={2}&{3}={4}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        string.Empty);

    string expected = string.Format (
        "{0}?{1}={2}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string actual = UrlUtility.DeleteParameter (original, parameter2);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void DeleteParameterFromUrlWithEmptyFirstParameter ()
  {
    string url = "http://localhost/Default.html";
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1ä#";
    string value2 = "Value2";

    string original = string.Format (
        "{0}?{1}={2}&{3}={4}", 
        url,
        parameter1, 
        string.Empty, 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string expected = string.Format (
        "{0}?{3}={4}", 
        url,
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string actual = UrlUtility.DeleteParameter (original, parameter1);
    Assert.AreEqual (expected, actual);
  }
  [Test]
  public void DeleteParameterFromUrlWithNoParameter ()
  {
    string url = "http://localhost/Default.html";
    string parameter1 = "Parameter1";
    string expected = url;

    string actual = UrlUtility.DeleteParameter (url, parameter1);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void DeleteParameterFromUrlWithQuestionMark ()
  {
    string url = "http://localhost/Default.html?";
    string parameter1 = "Parameter1";
    string expected = url;

    string actual = UrlUtility.DeleteParameter (url, parameter1);
    Assert.AreEqual (expected, actual);
  }

  [Test]
  public void GetParameterFromLastValue()
  {
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1ä#";
    string value2 = "Value2";

    string url = string.Format (
        "http://localhost/Default.html?{0}={1}&{2}={3}", 
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string actual = UrlUtility.GetParameter (url, parameter2, _currentEncoding);
    Assert.AreEqual (value2, actual);
  }

  [Test]
  public void GetParameterFromFirstValue()
  {
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1ä#";
    string value2 = "Value2";

    string url = string.Format (
        "http://localhost/Default.html?{0}={1}&{2}={3}", 
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2, 
        HttpUtility.UrlEncode (value2, _currentEncoding));

    string actual = UrlUtility.GetParameter (url, parameter1, _currentEncoding);
    Assert.AreEqual (value1, actual);
  }

  [Test]
  public void GetParameterFromMissingValue()
  {
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1ä#";

    string url = string.Format (
        "http://localhost/Default.html?{0}={1}", 
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding));

    string actual = UrlUtility.GetParameter (url, parameter2, _currentEncoding);
    Assert.IsNull (actual);
  }

  [Test]
  public void GetParameterFromEmptyValue()
  {
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1ä#";
    string value2 = string.Empty;

    string url = string.Format (
        "http://localhost/Default.html?{0}={1}&{2}=", 
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2);

    string actual = UrlUtility.GetParameter (url, parameter2, _currentEncoding);
    Assert.AreEqual (value2, actual);
  }

  [Test]
  public void GetParameterFromFirstValueWithoutValueDelimiter()
  {
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1ä#";

    string url = string.Format (
        "http://localhost/Default.html?{0}={1}&{2}", 
        parameter1, 
        HttpUtility.UrlEncode (value1, _currentEncoding), 
        parameter2);

    string actual = UrlUtility.GetParameter (url, parameter2, _currentEncoding);
    Assert.IsNull (actual);
  }
}

}
