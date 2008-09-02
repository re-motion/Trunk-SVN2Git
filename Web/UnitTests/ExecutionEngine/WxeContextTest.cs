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
using System.Collections.Specialized;
using System.Web;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.Web.Utilities;
using TestFunction=Remotion.Web.UnitTests.ExecutionEngine.TestFunctions.TestFunction;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

[TestFixture]
public class WxeContextTest
{
  private HttpContext _currentHttpContext;
  private WxeContextMock _currentWxeContext;
  private Type _functionType;
  private string _functionTypeName;
  private string _resource;

  [SetUp]
  public virtual void SetUp()
  {
    NameValueCollection queryString = new NameValueCollection ();
    queryString.Add (WxeHandler.Parameters.ReturnUrl, "/Root.wxe");

    _currentHttpContext = WxeContextMock.CreateHttpContext (queryString);

    _functionType = typeof (TestFunction);
    _functionTypeName = WebTypeUtility.GetQualifiedName (_functionType);
    _resource = "~/Test.wxe";

    UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (_functionType, _resource));

    _currentWxeContext = new WxeContextMock (_currentHttpContext, queryString);
    WxeContext.SetCurrent(_currentWxeContext);

    WebConfigurationMock.Current = new Remotion.Web.Configuration.WebConfiguration();
    WebConfigurationMock.Current.ExecutionEngine.MaximumUrlLength = 100;

  }

  [TearDown]
  public virtual void TearDown()
  { 
    WebConfigurationMock.Current = null;
    Remotion.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
  }

	[Test]
  public void GetStaticPermanentUrlWithDefaultWxeHandlerWithoutMappingForFunctionType()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineWithDefaultWxeHandler();
    Remotion.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);

    string wxeHandler = Remotion.Web.Configuration.WebConfiguration.Current.ExecutionEngine.DefaultWxeHandler;
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, wxeHandler);
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add (WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = WxeContext.GetPermanentUrl (new HttpContextWrapper (_currentHttpContext), _functionType, new NameValueCollection ());
    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

	[Test]
  public void GetStaticPermanentUrlWithDefaultWxeHandlerForMappedFunctionType()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineWithDefaultWxeHandler();

    string wxeHandler = Remotion.Web.Configuration.WebConfiguration.Current.ExecutionEngine.DefaultWxeHandler;
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    string permanentUrl = WxeContext.GetPermanentUrl (new HttpContextWrapper (_currentHttpContext), _functionType, new NameValueCollection ());
    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

	[Test]
  public void GetStaticPermanentUrlWithEmptyQueryString()
  {
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    string permanentUrl = WxeContext.GetPermanentUrl (new HttpContextWrapper (_currentHttpContext), _functionType, new NameValueCollection ());
    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  public void GetStaticPermanentUrlWithQueryString()
  {
    string parameterName = "Param";
    string parameterValue = "Hello World!";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add (queryString);
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = WxeContext.GetPermanentUrl (new HttpContextWrapper(_currentHttpContext), _functionType, queryString);

    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  [ExpectedException (typeof (WxePermanentUrlTooLongException))]
  public void GetStaticPermanentUrlWithQueryStringExceedingMaxLength()
  {
    string parameterName = "Param";
    string parameterValue = "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 ";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    WxeContext.GetPermanentUrl (new HttpContextWrapper (_currentHttpContext), _functionType, queryString);
  }

	[Test]
  [ExpectedException (typeof (WxeException))]
  public void GetStaticPermanentUrlWithoutWxeHandler()
  {
    WebConfigurationMock.Current = null;
    Remotion.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
    WxeContext.GetPermanentUrl (new HttpContextWrapper (_currentHttpContext), _functionType, new NameValueCollection ());
  }

	[Test]
  public void GetPermanentUrlWithEmptyQueryString()
  {
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    string permanentUrl = _currentWxeContext.GetPermanentUrl (_functionType, new NameValueCollection(), false);
    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  public void GetPermanentUrlWithQueryString()
  {
    string parameterName = "Param";
    string parameterValue = "Hello World!";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add (queryString);
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = _currentWxeContext.GetPermanentUrl (_functionType, queryString, false);

    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  [ExpectedException (typeof (WxePermanentUrlTooLongException))]
  public void GetPermanentUrlWithQueryStringExceedingMaxLength()
  {
    string parameterName = "Param";
    string parameterValue = "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 ";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    _currentWxeContext.GetPermanentUrl (_functionType, queryString, false);
  }

  [Test]
  public void GetPermanentUrlWithQueryStringAndParentPermanentUrl()
  {
    string parameterName = "Param";
    string parameterValue = "Hello World!";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add (queryString);
    
    string parentUrl = _currentHttpContext.Request.Url.AbsolutePath;
    parentUrl += UrlUtility.FormatQueryString (_currentHttpContext.Request.QueryString);
    expectedQueryString.Add (WxeHandler.Parameters.ReturnUrl, parentUrl);
    
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = _currentWxeContext.GetPermanentUrl (_functionType, queryString, true);

    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  public void GetPermanentUrlWithParentPermanentUrlAndRemoveBothReturnUrls()
  {
    string parameterName = "Param";
    string parameterValue = "123456789 123456789 123456789 123456789 123456789 123456789 ";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add (queryString);
        
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = _currentWxeContext.GetPermanentUrl (_functionType, queryString, false);

    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  public void GetPermanentUrlWithParentPermanentUrlAndRemoveInnermostReturnUrl()
  {
    string parameterName = "Param";
    string parameterValue = "123456789 123456789 123456789 123456789 ";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add (queryString);
    
    string parentUrl = _currentHttpContext.Request.Url.AbsolutePath;
    parentUrl += UrlUtility.FormatQueryString (_currentHttpContext.Request.QueryString);
    parentUrl = UrlUtility.DeleteParameter (parentUrl, WxeHandler.Parameters.ReturnUrl);
    expectedQueryString.Add (WxeHandler.Parameters.ReturnUrl, parentUrl);
    
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = _currentWxeContext.GetPermanentUrl (_functionType, queryString, true);

    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void GetPermanentUrlWithExistingReturnUrl()
  {
    string parameterName = "Param";
    string parameterValue = "Hello World!";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);
    queryString.Add (WxeHandler.Parameters.ReturnUrl, "");
    
    _currentWxeContext.GetPermanentUrl (_functionType, queryString, true);
  }
}

}
