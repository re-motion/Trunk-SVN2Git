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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Collections;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Diagnostics.ToText
{
  using GetValueFunctionKey = Tuple<Type, FieldInfo>;

  public abstract class ToTextBuilderBase : IToTextBuilderBase
  {
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

    public IToTextBuilderBase writeIfSkeletonOrHigher
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Skeleton);
      }
    }

    public IToTextBuilderBase writeIfBasicOrHigher
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Basic);
      }
    }

    public IToTextBuilderBase writeIfMediumOrHigher
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Medium);
      }
    }

    public IToTextBuilderBase writeIfComplexOrHigher
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Complex);
      }
    }

    public IToTextBuilderBase writeIfFull
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Full);
      }
    }

    public void SetOutputComplexityToDisable () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Disable; }
    public void SetOutputComplexityToSkeleton () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Skeleton; }
    public void SetOutputComplexityToBasic () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Basic; }
    public void SetOutputComplexityToMedium () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Medium; }
    public void SetOutputComplexityToComplex () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Complex; }
    public void SetOutputComplexityToFull () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Full; }
    public abstract IToTextBuilderBase WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel complexityLevel);
    public abstract string CheckAndConvertToString ();
    protected abstract void BeforeWriteElement ();
    protected abstract void AfterWriteElement ();
    public abstract IToTextBuilderBase Flush ();


    public abstract IToTextBuilderBase WriteNewLine ();

    public IToTextBuilderBase WriteNewLine (int numberNewlines)
    {
      for (int i = 0; i < numberNewlines; ++i)
      {
        WriteNewLine();
      }
      return this;
    }

    public IToTextBuilderBase nl ()
    {
      WriteNewLine ();
      return this;
    }

    public IToTextBuilderBase nl (int numberNewlines)
    {
      return WriteNewLine (numberNewlines);
    }


    public abstract IToTextBuilderBase WriteSequenceLiteralBegin (
        string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix
    );


    protected abstract IToTextBuilderBase SequenceBegin ();

    protected virtual void BeforeNewSequence ()
    {
      BeforeWriteElement();
      PushSequenceState (SequenceState);
    }

    protected void PushSequenceState (SequenceStateHolder sequenceState)
    {
      sequenceStack.Push (sequenceState);
    }



    public IToTextBuilderBase sbLiteral ()
    {
      return WriteSequenceLiteralBegin ("", "(", "", "", ",", ")");
    }

    public IToTextBuilderBase sbLiteral (string sequencePrefix, string separator, string sequencePostfix)
    {
      return WriteSequenceLiteralBegin ("", sequencePrefix, "", "", separator, sequencePostfix);
    }

    public IToTextBuilderBase sbLiteral (string sequencePrefix, string sequencePostfix)
    {
      return WriteSequenceLiteralBegin ("", sequencePrefix, "", "", ",", sequencePostfix);
    }

    public IToTextBuilderBase sbLiteral (string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    {
      return WriteSequenceLiteralBegin ("", sequencePrefix, elementPrefix, elementPostfix, separator, sequencePostfix);
    }

    
    
    public abstract IToTextBuilderBase WriteSequenceBegin ();

    public IToTextBuilderBase sb ()
    {
      return WriteSequenceBegin();
    }



    public abstract IToTextBuilderBase WriteRawStringUnsafe (string s);

    public IToTextBuilderBase WriteRawString (string s)
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


    public abstract IToTextBuilderBase WriteRawStringEscapedUnsafe (string s);

    public IToTextBuilderBase WriteRawStringEscaped (string s) 
    {
      AssertIsInRawSequence ();
      WriteRawStringEscapedUnsafe (s);
      return this;
    }

    public IToTextBuilderBase s (string s)
    {
      return WriteRawStringUnsafe (s);
    }

    public abstract IToTextBuilderBase WriteRawCharUnsafe (char c);

    public IToTextBuilderBase WriteRawChar (char c)
    {
      AssertIsInRawSequence ();
      WriteRawCharUnsafe (c);
      return this;
    }

    // TODO: move up
    private static readonly ICache<GetValueFunctionKey, Func<object, object>> s_getValueFunctionCache = new InterlockedCache<GetValueFunctionKey, Func<object, object>> ();

    public IToTextBuilderBase WriteElement<T> (Expression<Func<T>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      try
      {
        var memberExpression = (MemberExpression) expression.Body;
        //var memberExpression = expression.Body as MemberExpression;
        //Assertion.IsNotNull (memberExpression); // TODO: Use Check

        var memberField = (FieldInfo) memberExpression.Member;
        //var memberField = memberExpression.Member as FieldInfo;
        //Assertion.IsNotNull (memberField); // TODO: Use Check

        var closureExpression = (ConstantExpression) memberExpression.Expression;
        //var closureExpression = memberExpression.Expression as ConstantExpression;
        //Assertion.IsNotNull (closureExpression); // TODO: Use Check

        object closure = closureExpression.Value;
        Assertion.DebugAssert (closure != null);
        Type closureType = closure.GetType();

        // memberField.GetValue (closure);

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
        object value = getValueFunction (closure);
      }
      catch (InvalidCastException)
      {
        throw new ArgumentException ("ToTextBuilder.WriteElement currently supports only expressions of the form () => varName. The expression: " + Environment.NewLine + expression.Body.ToString () + Environment.NewLine + " does not comply with this restriction.");
      }

      var variableName = RightUntilChar (expression.Body.ToString (), '.');
      var variableValue = expression.Compile () ();
      return WriteElement (variableName, variableValue);
    }

    public IToTextBuilderBase WriteElement (string name, Object obj)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      WriteMemberRaw (name, obj);
      return this;
    }

    protected abstract IToTextBuilderBase WriteMemberRaw (string name, Object obj);


    public IToTextBuilderBase e (Object obj)
    {
      return WriteElement (obj);
    }

    public IToTextBuilderBase e<T> (Expression<Func<T>> expression)
    {
      return WriteElement (expression);
    }

    public IToTextBuilderBase e (string name, Object obj)
    {
      return WriteElement (name, obj);
    }

    public abstract IToTextBuilderBase WriteEnumerable (IEnumerable enumerable);

    public IToTextBuilderBase array (Array array)
    {
      return WriteArray (array);
    }


    public IToTextBuilderBase enumerable (IEnumerable enumerable)
    {
      return WriteEnumerable (enumerable);
    }

    public abstract IToTextBuilderBase WriteArray (Array array);
    public abstract IToTextBuilderBase WriteSequenceArrayBegin ();



    public IToTextBuilderBase WriteRaw (string s)
    {
      return WriteRawString (s);
    }

    public abstract IToTextBuilderBase WriteRaw (Object obj);


    public abstract IToTextBuilderBase WriteInstanceBegin (Type type);

    public IToTextBuilderBase ib (Type type)
    {
      return WriteInstanceBegin (type);
    }

    public IToTextBuilderBase  ib<T>()
    {
      return WriteInstanceBegin (typeof(T));
    }

    public IToTextBuilderBase ie ()
    {
      return WriteSequenceEnd ();
    }


    public IToTextBuilderBase WriteSequenceEnd ()
    {
      SequenceEnd ();
      return this;
    }

    protected abstract void SequenceEnd ();

    public IToTextBuilderBase se ()
    {
      return WriteSequenceEnd ();
    }

    public IToTextBuilderBase WriteElement (object obj)
    {
      _toTextProvider.ToText (obj, this);
      return this;
    }

    public IToTextBuilderBase WriteSequenceElements (params object[] sequenceElements)
    {
      Assertion.IsTrue (IsInSequence);
      foreach (var obj in sequenceElements)
      {
        WriteElement (obj);
      }
      return this;
    }

    public IToTextBuilderBase elements (params object[] sequenceElements)
    {
      return WriteSequenceElements (sequenceElements);
    }

    public IToTextBuilderBase elementsNumbered (string s1, int i0, int i1)
    {
      Assertion.IsTrue (IsInSequence);
      for (int i = i0; i <= i1; ++i)
      {
        WriteElement (s1 + i);
      }
      return this;
    }

    public IToTextBuilderBase WriteSequence (params object[] sequenceElements)
    {
      WriteSequenceBegin ();
      foreach (var obj in sequenceElements)
      {
        WriteElement (obj);
      }
      WriteSequenceEnd ();
      return this;
    }

    public IToTextBuilderBase sequence (params object[] sequenceElements)
    {
      return WriteSequence (sequenceElements);
    }



    public IToTextBuilderBase WriteRawElementBegin ()
    {
      IsInRawSequence = true;
      BeforeWriteElement();
      return this;
    }

    public IToTextBuilderBase WriteRawElementEnd ()
    {
      AfterWriteElement ();
      IsInRawSequence = false;
      return this;
    }


    private static string RightUntilChar (string s, char separator)
    {
      int iSeparator = s.LastIndexOf (separator);
      if (iSeparator > 0)
      {
        return s.Substring (iSeparator + 1, s.Length - iSeparator - 1);
      }
      else
      {
        return s;
      }
    }


    public virtual IToTextBuilderBase sEsc (string s)
    {
      return WriteRawStringEscapedUnsafe (s); 
      
    }
  }
}