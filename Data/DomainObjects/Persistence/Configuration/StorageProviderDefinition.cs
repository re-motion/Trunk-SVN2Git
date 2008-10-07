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
using System.Linq.Expressions;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration.StorageProviders;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.Linq.Parsing.Details;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  public abstract class StorageProviderDefinition: ExtendedProviderBase
  {
    // types

    // static members and constants

    // member fields

    private Type _storageProviderType;
    private TypeConversionProvider _typeConversionProvider;
    private TypeProvider _typeProvider;
    private ISqlGenerator _linqSqlGenerator;

    // construction and disposing

    protected StorageProviderDefinition (string name, NameValueCollection config)
        : base (name, config)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("config", config);

      string storageProviderTypeName = GetAndRemoveNonEmptyStringAttribute (config, "providerType", name, true);
      Initialize (TypeUtility.GetType (storageProviderTypeName, true));
    }

    protected StorageProviderDefinition (string name, Type storageProviderType)
        : base (name, new NameValueCollection())
    {
      ArgumentUtility.CheckNotNull ("storageProviderType", storageProviderType);

      Initialize (storageProviderType);
    }

    private void Initialize(Type storageProviderType)
    {
      _storageProviderType = storageProviderType;
      _typeConversionProvider = TypeConversionProvider.Create ();
      _typeProvider = new TypeProvider();
      
      // TODO: let concrete provider definition instantiate ISqlGenerator
      ResetLinqSqlGenerator();
    }

    // abstract methods and properties

    public abstract bool IsIdentityTypeSupported (Type identityType);

    // methods and properties

    public void CheckIdentityType (Type identityType)
    {
      if (!IsIdentityTypeSupported (identityType))
        throw new IdentityTypeNotSupportedException (_storageProviderType, identityType);
    }

    public Type StorageProviderType
    {
      get { return _storageProviderType; }
    }

    public TypeConversionProvider TypeConversionProvider
    {
      get { return _typeConversionProvider; }
    }

    public TypeProvider TypeProvider
    {
      get { return _typeProvider; }
    }

    public ISqlGenerator LinqSqlGenerator
    {
      get { return _linqSqlGenerator; }
    }

    public void ResetLinqSqlGenerator ()
    {
      _linqSqlGenerator = ObjectFactory.Create<SqlServerGenerator> ().With (DatabaseInfo.Instance);

      WhereConditionParserRegistry whereConditionParserRegistry = _linqSqlGenerator.DetailParserRegistries.WhereConditionParser;
      whereConditionParserRegistry.RegisterParser (typeof (MethodCallExpression), new ContainsObjectParser (whereConditionParserRegistry));
    }
  }
}
