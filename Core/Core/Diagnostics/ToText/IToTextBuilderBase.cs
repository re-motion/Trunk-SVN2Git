using System;
using System.Collections;
using System.Linq.Expressions;

namespace Remotion.Diagnostics.ToText
{
  public interface IToTextBuilderBase
  {
    bool AllowNewline { get; set; }

    bool Enabled { get; set; }
    bool IsInSequence { get; }

    //SequenceStateHolder SequenceState { get; protected set; }
    ToTextBuilderBase.ToTextBuilderOutputComplexityLevel OutputComplexity { get; }
    ToTextProvider ToTextProvider { get; set; }
    IToTextBuilderBase cSkeleton { get; }
    IToTextBuilderBase cBasic { get; }
    IToTextBuilderBase cMedium { get; }
    IToTextBuilderBase cComplex { get; }
    IToTextBuilderBase cFull { get; }
    IToTextBuilderBase WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderBase.ToTextBuilderOutputComplexityLevel complexityLevel);

    void OutputDisable ();
    void OutputSkeleton ();
    void OutputBasic ();
    void OutputMedium ();
    void OutputComplex ();
    void OutputFull ();



    string CheckAndConvertToString ();
    //IToTextBuilderBase ToTextString (string s);
    IToTextBuilderBase Flush ();


    void WriteRawElementBegin ();
    void WriteRawElementEnd ();
    
    IToTextBuilderBase WriteNewLine ();
    IToTextBuilderBase nl ();

    IToTextBuilderBase WriteRawString (string s);
    IToTextBuilderBase WriteRawStringEscaped (string s);
    IToTextBuilderBase sEsc (string s);

    IToTextBuilderBase WriteRawChar (char c);


    /// <summary>
    /// Writes the given string to the output stream.
    /// </summary>
    IToTextBuilderBase s (string s);

    // raw string: guaranteed no processing ?
    //IToTextBuilderBase rs (string s);

    ///// <summary>
    ///// Applies <see cref="Object.ToString"/> to the passed <see cref="object"/> and writes the resulting string to the output stream.
    ///// </summary>
    //IToTextBuilderBase ts (object obj);

    /// <summary>
    /// Passes the object argument to the stream writer to be written to the output stream, without any processing.
    /// </summary>
    IToTextBuilderBase LowLevelWrite (Object obj);
    IToTextBuilderBase LowLevelWrite (string s);



    IToTextBuilderBase WriteSequenceLiteralBegin (string name, string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix);
    IToTextBuilderBase sbLiteral ();
    IToTextBuilderBase sbLiteral (string sequencePrefix, string sequencePostfix);
    IToTextBuilderBase sbLiteral (string sequencePrefix, string separator, string sequencePostfix);
    IToTextBuilderBase sbLiteral (string sequencePrefix, string elementPostfix, string elementPrefix, string separator, string sequencePostfix);


    IToTextBuilderBase WriteSequenceBegin ();
    IToTextBuilderBase sb ();


    /// <summary>
    /// <para>Applies <see cref="ToText"/> to the passed argument and emits the result through the TextBuilder.</para>
    /// <para>Shorthand notation: <see cref="e(object)"/>.</para>
    /// </summary>
    IToTextBuilderBase WriteElement (object obj);
    IToTextBuilderBase WriteElement (string name, Object obj);
    IToTextBuilderBase WriteElement<T> (Expression<Func<object, T>> expression);
    IToTextBuilderBase e (Object obj);
    IToTextBuilderBase e<T> (Expression<Func<object, T>> expression);
    IToTextBuilderBase e (string name, Object obj);
    

    IToTextBuilderBase WriteSequenceEnd ();
    IToTextBuilderBase se ();

   

    IToTextBuilderBase WriteSequenceElements (params object[] sequenceElements);
    IToTextBuilderBase elements (params object[] sequenceElements);
    IToTextBuilderBase elementsNumbered (string s1, int i0, int i1);

    IToTextBuilderBase WriteInstanceBegin (Type type);
    IToTextBuilderBase ib (Type type);

    IToTextBuilderBase WriteInstanceEnd ();
    IToTextBuilderBase ie ();


    IToTextBuilderBase WriteSequenceArrayBegin ();
    // TODO: Check that if ToTextprovider holds ToTextBuilderBase, exposing these through the interface is still necessary.
    IToTextBuilderBase WriteArray (Array array);
    IToTextBuilderBase array (Array array);
    
    IToTextBuilderBase WriteEnumerable (IEnumerable collection);
    IToTextBuilderBase collection (IEnumerable collection);
  }
}