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
using System.IO;
using System.Linq.Expressions;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText
{
  public abstract class ToTextBuilderBase : IToTextBuilderBase
  {
    private ToTextProvider _toTextProvider;
    protected readonly Stack<SequenceStateHolder> sequenceStack = new Stack<SequenceStateHolder> (16);
    //protected DisableableWriter _disableableWriter;
    //protected bool _allowNewline = true;

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

    public IToTextBuilderBase cSkeleton
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Skeleton);
      }
    }

    public IToTextBuilderBase cBasic
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Basic);
      }
    }

    public IToTextBuilderBase cMedium
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Medium);
      }
    }

    public IToTextBuilderBase cComplex
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Complex);
      }
    }

    public IToTextBuilderBase cFull
    {
      get
      {
        return WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Full);
      }
    }

    //public abstract IToTextBuilderBase ToTextString (string s);
    public void OutputDisable () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Disable; }
    public void OutputSkeleton () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Skeleton; }
    public void OutputBasic () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Basic; }
    public void OutputMedium () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Medium; }
    public void OutputComplex () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Complex; }
    public void OutputFull () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Full; }
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

    //public abstract virtual IToTextBuilderBase AppendSeperator ();
    

    //protected abstract IToTextBuilderBase WriteObjectToString (object obj);

    //public IToTextBuilderBase ts (object obj)
    //{
    //  return WriteObjectToString (obj);
    //}


    public abstract IToTextBuilderBase WriteSequenceLiteralBegin (
        string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix
    );


    //public IToTextBuilderBase WriteSequenceLiteralBegin (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    //{
    //  return SequenceBegin (name, sequencePrefix, elementPrefix, elementPostfix, separator, sequencePostfix);
    //}

    //protected abstract IToTextBuilderBase SequenceBegin (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix);

    //protected IToTextBuilderBase SequenceBegin (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    //{
    //  BeforeNewSequence();
    //  SequenceState = new SequenceStateHolder (name, sequencePrefix, elementPrefix, elementPostfix, separator, sequencePostfix);
    //  SequenceBeginWritePart (SequenceState);
    //  return this;
    //}

    protected abstract IToTextBuilderBase SequenceBegin ();

    protected virtual void BeforeNewSequence ()
    {
      //if (!SequenceIsElement)
      //{
      //  BeforeWriteElement();
      //}
      
      BeforeWriteElement();
      //sequenceStack.Push (SequenceState);
      PushSequenceState (SequenceState);
    }

    protected void PushSequenceState (SequenceStateHolder sequenceState)
    {
      sequenceStack.Push (sequenceState);
    }


    //protected abstract void SequenceBeginWritePart (SequenceStateHolder sequenceState);


    public IToTextBuilderBase sbLiteral ()
    {
      return WriteSequenceLiteralBegin ("", "(", "", "", ",", ")");
    }

    //public IToTextBuilderBase sbLiteral (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    //{
    //  return WriteSequenceLiteralBegin ("", sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);
    //}

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

    private void AssertIsInRawSequence ()
    {
      Assertion.IsTrue(IsInRawSequence);
    }

    protected bool IsInRawSequence
    {
      get; set;
    }


    //public abstract IToTextBuilderBase AppendRawString (string s);

    public abstract IToTextBuilderBase WriteRawStringEscapedUnsafe (string s);

    public IToTextBuilderBase WriteRawStringEscaped (string s) 
    {
      AssertIsInRawSequence ();
      WriteRawStringEscapedUnsafe (s);
      return this;
    }

    //public abstract IToTextBuilderBase AppendRawEscapedString (string s);

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

    //public abstract IToTextBuilderBase AppendRawChar (char c);
    //public abstract IToTextBuilderBase WriteElement (string name, Object obj);

    public IToTextBuilderBase WriteElement<T> (Expression<Func<object, T>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      var variableName = RightUntilChar (expression.Body.ToString (), '.');
      var variableValue = expression.Compile ().Invoke (null);
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

    public IToTextBuilderBase e<T> (Expression<Func<object, T>> expression)
    {
      return WriteElement (expression);
    }

    public IToTextBuilderBase e (string name, Object obj)
    {
      return WriteElement (name, obj);
    }

    public abstract IToTextBuilderBase WriteEnumerable (IEnumerable collection);
    //public abstract IToTextBuilderBase array (Array array);

    public IToTextBuilderBase array (Array array)
    {
      return WriteArray (array);
    }


    public IToTextBuilderBase collection (IEnumerable collection)
    {
      return WriteEnumerable (collection);
    }

    public abstract IToTextBuilderBase WriteArray (Array array);
    public abstract IToTextBuilderBase WriteSequenceArrayBegin ();


    //public IToTextBuilderBase WriteElement (Object obj)
    //{
    //  toTextProvider.ToText (obj, this);
    //  return this;
    //}

    //protected IToTextBuilderBase AppendToTextRaw (Object obj)
    //{
    //  toTextProvider.ToText (obj, this);
    //  return this;
    //}

    //public IToTextBuilderBase e (Object obj)
    //{
    //  return WriteElement (obj);
    //}

    //public IToTextBuilderBase WriteElement (Object obj)
    //{
    //  toTextProvider.ToText (obj, this);
    //  return this;
    //}


    public IToTextBuilderBase LowLevelWrite (string s)
    {
      return WriteRawString (s);
    }

    public abstract IToTextBuilderBase LowLevelWrite (Object obj);

    //public IToTextBuilderBase WriteInstanceBegin (Type type)
    //{
    //  SequenceBegin ("", "[" + type.Name + "  ", "", "", ",", "]");
    //  return this;
    //}

    public abstract IToTextBuilderBase WriteInstanceBegin (Type type);

    public IToTextBuilderBase ib (Type type)
    {
      return WriteInstanceBegin (type);
    }


    public IToTextBuilderBase WriteInstanceEnd ()
    {
      //return AppendInstanceEnd ();
      SequenceEnd ();
      return this;
    }

    public IToTextBuilderBase ie ()
    {
      return WriteInstanceEnd ();
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
      //Assertion.IsTrue (IsInSequence);
      _toTextProvider.ToText (obj, this);
      return this;
    }

    //public IToTextBuilderBase e (object obj)
    //{
    //  return WriteElement (obj);
    //}

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
      for (int i = i0; i <= i1; ++i)
      {
        WriteElement (s1 + i);
      }
      return this;
    }

    public void WriteRawElementBegin ()
    {
      IsInRawSequence = true;
      BeforeWriteElement();
    }

    public void WriteRawElementEnd ()
    {
      AfterWriteElement ();
      IsInRawSequence = false;
    }



    //public abstract IToTextBuilderBase EmitNamedSequenceBegin ();
    //public abstract IToTextBuilderBase EmitNamedSequenceEnd ();

    //public override IToTextBuilderBase EmitNamedSequenceBegin ()
    //{
    //  BeforeWriteElement ();
    //  return SequenceBegin (sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);
    //}

    //public override IToTextBuilderBase EmitNamedSequenceEnd ()
    //{
    //  throw new System.NotImplementedException ();
    //}



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

    //public virtual IToTextBuilderBase WriteElement (object obj)
    //{
    //  toTextProvider.ToText (obj, this);
    //  return this;
    //}
    public virtual IToTextBuilderBase sEsc (string s)
    {
      return WriteRawStringEscapedUnsafe (s); 
      
    }
  }
}