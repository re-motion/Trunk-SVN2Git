// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Collections;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Diagnostics.ToText.Infrastructure
{
  public abstract class ToTextBuilderBase : IToTextBuilder
  {
    private static readonly ICache<Tuple<Type, FieldInfo>, Func<object, object>> s_getValueFunctionCache = new InterlockedCache<Tuple<Type, FieldInfo>, Func<object, object>> ();
    private ToTextProvider _toTextProvider;
    protected readonly Stack<SequenceStateHolder> sequenceStack = new Stack<SequenceStateHolder> (16);

    public enum ToTextBuilderOutputComplexityLevel
    {
      Disable,
      Skeleton,
      Basic,
      Medium,
      Complex,
      Full,
    };


    protected ToTextBuilderBase (ToTextProvider toTextProvider)
    {
      _toTextProvider = toTextProvider;
      OutputComplexity = ToTextBuilderOutputComplexityLevel.Basic;
      SequenceState = null;
      AllowNewline = true;
    }


    public SequenceStateHolder SequenceState { get; protected set; }
    public bool AllowNewline { get; set; }
    public abstract bool Enabled { get; set; }


    public ToTextBuilderOutputComplexityLevel OutputComplexity { get; protected set; }

    public bool IsInSequence
    {
      get { return SequenceState != null; }
    }

    public ToTextProvider ToTextProvider
    {
      get { return _toTextProvider; }
      set { _toTextProvider = value; }
    }

    public IToTextBuilder writeIfSkeletonOrHigher
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Skeleton);
      }
    }

    public IToTextBuilder writeIfBasicOrHigher
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Basic);
      }
    }

    public IToTextBuilder writeIfMediumOrHigher
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Medium);
      }
    }

    public IToTextBuilder writeIfComplexOrHigher
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Complex);
      }
    }

    public IToTextBuilder writeIfFull
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Full);
      }
    }

    public IToTextBuilder SetOutputComplexityToDisable () { 
      OutputComplexity = ToTextBuilderOutputComplexityLevel.Disable;
      return this; 
    }

    public IToTextBuilder SetOutputComplexityToSkeleton ()
    {
      OutputComplexity = ToTextBuilderOutputComplexityLevel.Skeleton;
      return this;
    }

    public IToTextBuilder SetOutputComplexityToBasic ()
    {
      OutputComplexity = ToTextBuilderOutputComplexityLevel.Basic;
      return this;
    }

    public IToTextBuilder SetOutputComplexityToMedium ()
    {
      OutputComplexity = ToTextBuilderOutputComplexityLevel.Medium;
      return this;
    }

    public IToTextBuilder SetOutputComplexityToComplex ()
    {
      OutputComplexity = ToTextBuilderOutputComplexityLevel.Complex;
      return this;
    }

    public IToTextBuilder SetOutputComplexityToFull ()
    {
      OutputComplexity = ToTextBuilderOutputComplexityLevel.Full;
      return this;
    }

    public abstract IToTextBuilder WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel complexityLevel);
    public abstract string CheckAndConvertToString ();
    protected abstract void BeforeWriteElement ();
    protected abstract void AfterWriteElement ();
    public abstract IToTextBuilder Flush ();


    public abstract IToTextBuilder WriteNewLine ();

    public IToTextBuilder WriteNewLine (int numberNewlines)
    {
      for (int i = 0; i < numberNewlines; ++i)
      {
        WriteNewLine();
      }
      return this;
    }

    public IToTextBuilder nl ()
    {
      WriteNewLine ();
      return this;
    }

    public IToTextBuilder nl (int numberNewlines)
    {
      return WriteNewLine (numberNewlines);
    }


    public abstract IToTextBuilder WriteSequenceLiteralBegin (
        string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix
        );


    protected abstract IToTextBuilder SequenceBegin ();

    protected virtual void BeforeNewSequence ()
    {
      BeforeWriteElement();
      PushSequenceState (SequenceState);
    }

    protected void PushSequenceState (SequenceStateHolder sequenceState)
    {
      sequenceStack.Push (sequenceState);
    }



    public IToTextBuilder sbLiteral ()
    {
      return WriteSequenceLiteralBegin ("", "(", "", "", ",", ")");
    }

    public IToTextBuilder sbLiteral (string sequencePrefix, string separator, string sequencePostfix)
    {
      return WriteSequenceLiteralBegin ("", sequencePrefix, "", "", separator, sequencePostfix);
    }

    public IToTextBuilder sbLiteral (string sequencePrefix, string sequencePostfix)
    {
      return WriteSequenceLiteralBegin ("", sequencePrefix, "", "", ",", sequencePostfix);
    }

    public IToTextBuilder sbLiteral (string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    {
      return WriteSequenceLiteralBegin ("", sequencePrefix, elementPrefix, elementPostfix, separator, sequencePostfix);
    }

    
    
    public abstract IToTextBuilder WriteSequenceBegin ();

    public IToTextBuilder sb ()
    {
      return WriteSequenceBegin();
    }



    public abstract IToTextBuilder WriteRawStringUnsafe (string s);

    public IToTextBuilder WriteRawString (string s)
    {
      AssertIsInRawSequence ();
      WriteRawStringUnsafe (s);
      return this;
    }

    protected void AssertIsInRawSequence ()
    {
      Assertion.IsTrue(IsInRawSequence);
    }

    protected bool IsInRawSequence
    {
      get; set;
    }


    public abstract IToTextBuilder WriteRawStringEscapedUnsafe (string s);

    public IToTextBuilder WriteRawStringEscaped (string s) 
    {
      AssertIsInRawSequence ();
      WriteRawStringEscapedUnsafe (s);
      return this;
    }

    public IToTextBuilder s (string s)
    {
      return WriteRawStringUnsafe (s);
    }

    public abstract IToTextBuilder WriteRawCharUnsafe (char c);

    public IToTextBuilder WriteRawChar (char c)
    {
      AssertIsInRawSequence ();
      WriteRawCharUnsafe (c);
      return this;
    }



    // Note: The creation of the passed lambda expression (()=>varName) for WriteElement takes around 4 microseconds.
    // Overall performance is around 11 microseconds for the evaluation of the cached lambda and call to 
    // WriteElement (variableName, variableValue).
    public IToTextBuilder WriteElement<T> (Expression<Func<T>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      try
      {
        var memberExpression = (MemberExpression) expression.Body;
        var memberField = (FieldInfo) memberExpression.Member;
        var closureExpression = (ConstantExpression) memberExpression.Expression;

        object closure = closureExpression.Value;

        Assertion.DebugAssert (closure != null);
        Type closureType = closure.GetType();
        
        // The following code caches the call to  memberField.GetValue(closure)
        Func<object,object> getValueFunction;
        var key = new Tuple<Type, FieldInfo> (closureType, memberField);
        if (! s_getValueFunctionCache.TryGetValue (key, out getValueFunction))
        {
          getValueFunction = s_getValueFunctionCache.GetOrCreateValue (key, 
                                                                       delegate 
                                                                       {
                                                                         //
                                                                         // The following code builds this expression:
                                                                         // Expression<Func<object,object> = (object closure) => (object) ((TClosure) closure).<memberField>;
                                                                         //
                                                                         var param = Expression.Parameter (typeof (object), "closure");
                                                                         var closureAccess = Expression.Convert (param, closureType);
                                                                         var body = Expression.Field (closureAccess, memberField);
                                                                         var bodyAsObject = Expression.Convert (body, typeof (object));
                                                                         var newExpression = Expression.Lambda (bodyAsObject, param);
                                                                         return (Func<object,object>) newExpression.Compile();
                                                                       }
              );
        }
        object variableValue = getValueFunction (closure);
        string variableName = memberExpression.Member.Name;
        return WriteElement (variableName, variableValue);
      }
      catch (InvalidCastException)
      {
        throw new ArgumentException ("ToTextBuilder.WriteElement currently supports only expressions of the form () => varName. The expression: " + Environment.NewLine + expression.Body.ToString () + Environment.NewLine + " does not comply with this restriction.");
      }
    }


    public IToTextBuilder WriteElement (string name, Object obj)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      WriteMemberRaw (name, obj);
      return this;
    }

    protected abstract IToTextBuilder WriteMemberRaw (string name, Object obj);


    public IToTextBuilder e (Object obj)
    {
      return WriteElement (obj);
    }

    public IToTextBuilder e<T> (Expression<Func<T>> expression)
    {
      return WriteElement (expression);
    }

    public IToTextBuilder e (string name, Object obj)
    {
      return WriteElement (name, obj);
    }


    public IToTextBuilder eIfNotNull (string name, Object obj)
    {
      if (obj == null)
      {
        return this;
      }
      return WriteElement (name, obj);
    }

    public IToTextBuilder eIfNotNull (Object obj)
    {
      if (obj == null)
      {
        return this;
      }
      return WriteElement (obj);
    }

    public IToTextBuilder eIfNotEqualTo (string name, Object obj, Object notEqualObj)
    {
      if ((obj == null && notEqualObj == null) || (obj != null && obj.Equals (notEqualObj)))
      {
        return this;
      }
      return WriteElement (name, obj);
    }


    public abstract IToTextBuilder WriteEnumerable (IEnumerable enumerable);

    public IToTextBuilder array (Array array)
    {
      return WriteArray (array);
    }


    public IToTextBuilder enumerable (IEnumerable enumerable)
    {
      return WriteEnumerable (enumerable);
    }



    public abstract IToTextBuilder WriteDictionary (IDictionary dictionary);

    public IToTextBuilder dictionary (IDictionary dictionary)
    {
      return WriteDictionary (dictionary);
    }



    public abstract IToTextBuilder WriteArray (Array array);
    public abstract IToTextBuilder WriteSequenceArrayBegin ();
    


    public IToTextBuilder WriteRaw (string s)
    {
      return WriteRawString (s);
    }

    public abstract IToTextBuilder WriteRaw (Object obj);


    public abstract IToTextBuilder WriteInstanceBegin (Type type, string shortTypeName);

    public IToTextBuilder ib (Type type)
    {
      return WriteInstanceBegin (type, null);
    }

    public IToTextBuilder  ib<T>()
    {
      return WriteInstanceBegin (typeof(T), null);
    }

    public IToTextBuilder ib<T> (string shortTypeName)
    {
      return WriteInstanceBegin (typeof (T), shortTypeName);
    }


    public IToTextBuilder ie ()
    {
      return WriteSequenceEnd ();
    }


    public IToTextBuilder WriteSequenceEnd ()
    {
      SequenceEnd ();
      return this;
    }

    protected abstract void SequenceEnd ();

    public IToTextBuilder se ()
    {
      return WriteSequenceEnd ();
    }

    public IToTextBuilder WriteElement (object obj)
    {
      _toTextProvider.ToText (obj, this);
      return this;
    }

    public IToTextBuilder WriteSequenceElements (params object[] sequenceElements)
    {
      Assertion.IsTrue (IsInSequence);
      foreach (var obj in sequenceElements)
      {
        WriteElement (obj);
      }
      return this;
    }

    public IToTextBuilder elements (params object[] sequenceElements)
    {
      return WriteSequenceElements (sequenceElements);
    }

    public IToTextBuilder elementsNumbered (string s1, int i0, int i1)
    {
      Assertion.IsTrue (IsInSequence);
      for (int i = i0; i <= i1; ++i)
      {
        WriteElement (s1 + i);
      }
      return this;
    }

    public IToTextBuilder WriteSequence (params object[] sequenceElements)
    {
      WriteSequenceBegin ();
      foreach (var obj in sequenceElements)
      {
        WriteElement (obj);
      }
      WriteSequenceEnd ();
      return this;
    }

    public IToTextBuilder sequence (params object[] sequenceElements)
    {
      return WriteSequence (sequenceElements);
    }



    public IToTextBuilder WriteRawElementBegin ()
    {
      IsInRawSequence = true;
      BeforeWriteElement();
      return this;
    }

    public IToTextBuilder WriteRawElementEnd ()
    {
      AfterWriteElement ();
      IsInRawSequence = false;
      return this;
    }



    public virtual IToTextBuilder sEsc (string s)
    {
      return WriteRawStringEscapedUnsafe (s); 
      
    }

    void IDisposable.Dispose ()
    {
      Close();
    }

    public abstract void Close ();
  }
}