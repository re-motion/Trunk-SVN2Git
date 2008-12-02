/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocBooleanValue
{
  /// <summary>
  /// The <see cref="BocBooleanValueResourceSet"/> type is a value object that holds the information required 
  /// for displaying the <see cref="BocBooleanValue"/>'s three states.
  /// </summary>
  public class BocBooleanValueResourceSet
  {
    private readonly string _resourceKey;
    private readonly string _trueIconUrl;
    private readonly string _falseIconUrl;
    private readonly string _nullIconUrl;
    private readonly string _defaultTrueDescription;
    private readonly string _defaultFalseDescription;
    private readonly string _defaultNullDescription;

    public BocBooleanValueResourceSet (
        string resourceKey,
        string trueIconUrl,
        string falseIconUrl,
        string nullIconUrl,
        string defaultTrueDescription,
        string defaultFalseDescription,
        string defaultNullDescription)
    {
      ArgumentUtility.CheckNotNull ("resourceKey", resourceKey);
      ArgumentUtility.CheckNotNullOrEmpty ("trueIconUrl", trueIconUrl);
      ArgumentUtility.CheckNotNullOrEmpty ("falseIconUrl", falseIconUrl);
      ArgumentUtility.CheckNotNullOrEmpty ("nullIconUrl", nullIconUrl);
      ArgumentUtility.CheckNotNullOrEmpty ("defaultTrueDescription", defaultTrueDescription);
      ArgumentUtility.CheckNotNullOrEmpty ("defaultFalseDescription", defaultFalseDescription);
      ArgumentUtility.CheckNotNullOrEmpty ("defaultNullDescription", defaultNullDescription);

      _resourceKey = resourceKey;
      _trueIconUrl = trueIconUrl;
      _falseIconUrl = falseIconUrl;
      _nullIconUrl = nullIconUrl;
      _defaultTrueDescription = defaultTrueDescription;
      _defaultFalseDescription = defaultFalseDescription;
      _defaultNullDescription = defaultNullDescription;
    }

    public string ResourceKey
    {
      get { return _resourceKey; }
    }

    public string TrueIconUrl
    {
      get { return _trueIconUrl; }
    }

    public string FalseIconUrl
    {
      get { return _falseIconUrl; }
    }

    public string NullIconUrl
    {
      get { return _nullIconUrl; }
    }

    public string DefaultTrueDescription
    {
      get { return _defaultTrueDescription; }
    }

    public string DefaultFalseDescription
    {
      get { return _defaultFalseDescription; }
    }

    public string DefaultNullDescription
    {
      get { return _defaultNullDescription; }
    }
  }
}