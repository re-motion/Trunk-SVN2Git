using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remotion.Text.Diagnostic
{
  /// <summary>
  /// Allows a class implementing the IProcessor interface to visit each member of an outer product of a variable number of independently sized tuples.
  /// From a programmer's view the class supplies "variable number of nested for loops"-functionality.
  /// </summary>
  /// <include file='doc\include\Text\Diagnostic\OuterProduct.xml' path='OuterProduct/ClassExample1/*' />
  /// <include file='doc\include\Text\Diagnostic\OuterProduct.xml' path='OuterProduct/ClassExample2/*' />
  
  public class OuterProduct 
  {
    /// <summary>
    /// Interface a "Processor" class which can be passed to OuterProduct.ProcessOuterProduct needs to implement.
    /// </summary>
    public interface IProcessor {
      bool DoBeforeLoop ();
      bool DoAfterLoop ();
      void SetProcessingState (ProcessingState processingState);
      OuterProduct.ProcessingState ProcessingState { get; }
    }

    /// <summary>
    /// Convenience class to derive OuterProduct-processors from. ALready supplies ProcessingState-functionality.
    /// </summary>
    public class ProcessorBase : IProcessor 
    { 
      private OuterProduct.ProcessingState _processingState;
      public OuterProduct.ProcessingState ProcessingState
      {
        get { return _processingState; }
      }

      public virtual bool DoBeforeLoop ()
      {
        return true;
      }

      public virtual bool DoAfterLoop ()
      {
        return true;
      }
      
      public void SetProcessingState (OuterProduct.ProcessingState processingState)
      {
        _processingState = processingState;
      } 
    }

    /// <summary>
    /// The current state of the outer product / nested for loops traversal.
    /// DimensionIndices supplies the current permutation of indices (array with an entry for each for-loop).
    /// DimensionIndex is the currently running for-loop; ElementIndex (=DimensionIndices[DimensionIndex]) is the value of the loop-variable of the currently running for-loop.
    /// IsInnermostLoop, IsOutermostLoop can be queried to treat the innermost and outermost loop differently, if so required.
    /// </summary>
    public struct ProcessingState
    {
      public ProcessingState (OuterProduct outerProduct, int dimensionIndex)
      {
        _outerProduct = outerProduct;
        _dimensionIndex = dimensionIndex;
      }

      private readonly OuterProduct _outerProduct;
      private readonly int _dimensionIndex;

      /// <summary>
      /// The outer product dimension which is currently processed (i.e. the index of the currently running for-loop).
      /// </summary>
      public int DimensionIndex
      {
        get { return _dimensionIndex; }
      }

      /// <summary>
      /// Integer array containing the number of elements in each outer product dimension.
      /// </summary>
      public int[] NumberElementsPerDimension
      {
        get { return _outerProduct.NumberElementsPerDimension; }
      }

      /// <summary>
      /// Integer array containing the current permutation of outer product indices (i.e. each array entry is the current value of each for-loop variable;
      /// <see cref="ElementIndex"/>).
      /// </summary>
      public int[] DimensionIndices
      {
        get { return _outerProduct.DimensionIndices; }
      }

      /// <summary>
      /// The overall number of elements in the outer product.
      /// </summary>
      public int NumberElementsOverall
      {
        get { return _outerProduct.NumberElementsOverall; }
      }

      /// <summary>
      /// ElementIndex (=DimensionIndices[DimensionIndex]) is the value of the loop-variable of the currently running for-loop.
      /// </summary>
      public int ElementIndex
      {
        get { return _outerProduct.DimensionIndices[_dimensionIndex]; }
      }

      /// <summary>
      /// Whether the element is the first element in the current for-loop.
      /// </summary>
      public bool IsFirstLoopElement
      {
        get { return ElementIndex == 0; }
      }

      /// <summary>
      /// Whether the element is the last element in the current for-loop.
      /// </summary>
      public bool IsLastLoopElement
      {
        get { return ElementIndex == (NumberElementsPerDimension[DimensionIndex] - 1); }
      }

      /// <summary>
      /// Whether the current for-loop is the innermost loop.
      /// </summary>
      public bool IsInnermostLoop
      {
        get { return DimensionIndex == (NumberElementsPerDimension.Length - 1); }
      }

      /// <summary>
      /// Whether the current for-loop is the outermost loop.
      /// </summary>
      public bool IsOutermostLoop
      {
        get { return DimensionIndex == 0; }
      }

      /// <summary>
      /// Used internally by OuterProduct.ProcessOuterProduct. 
      /// </summary>
      public void SetCurrentElementIndex (int elementIndex)
      {
        DimensionIndices[_dimensionIndex] = elementIndex;
      }

      /// <summary>
      /// The overall elements of the outer product which have already been processed.
      /// </summary>
      public int NumberElementsProcessed
      {
        get { return _outerProduct.NumberElementsProcessed; }
      }

      public int[] GetDimensionIndicesCopy ()
      {
        return (int[]) DimensionIndices.Clone();
      }
    }



    //-------------------------------------------------------------------------------
    // Class members
    //-------------------------------------------------------------------------------

    private int _numberElementsProcessed;
    private int[] _numberElementsPerDimension;
    private int _numberElementsOverall;
    private int[] _currentDimensionIndices;



    public int NumberElementsProcessed
    {
      get { return _numberElementsProcessed; }
    }

    public int[] NumberElementsPerDimension
    {
      get { return _numberElementsPerDimension; }
      private set { _numberElementsPerDimension = value; }
    }

    public int NumberElementsOverall
    {
      get { return _numberElementsOverall; }
    }

    public int[] DimensionIndices
    {
      get { return _currentDimensionIndices; }
    }

    /// <summary>
    /// The total number of combinations in the outer product.
    /// </summary>
    public int Length { get { return NumberElementsOverall; } }



    /// <summary>
    /// Ctor to initialize OuterProduct from an integer array, where each array entry gives the number of elements along its
    /// corresponding dimension. In programers terms: The number of times each nested for-loop will loop.
    /// </summary>
    /// <param name="numberElementsPerDimension"></param>
    public OuterProduct (int[] numberElementsPerDimension)
    {
      Init ((int[]) numberElementsPerDimension.Clone () );
    }

    /// <summary>
    /// Ctor to initialize OuterProduct from an (rectangular) array.
    /// </summary>
    /// <param name="array"></param>
    public OuterProduct (Array array)
    {
      Init (array);
    }

    private void Init (Array array)
    {
      int numberDimensions = array.Rank;
      int[] numberElementsPerDimension = new int[numberDimensions];
      for (int iDimension = 0; iDimension < numberDimensions; ++iDimension)
      {
        numberElementsPerDimension[iDimension] = array.GetLength (iDimension);
      }
      Init (numberElementsPerDimension);
    }

    private void Init (int[] numberElementsPerDimension)
    {
      _numberElementsPerDimension = numberElementsPerDimension;
      InitProcessing();
    }

    private void InitProcessing ()
    {
      int rank = _numberElementsPerDimension.Length;
      _currentDimensionIndices = new int[rank];
      _numberElementsOverall = CalcOuterProductNrElementsOverall (_numberElementsPerDimension);
      _numberElementsProcessed = 0;
    }


    /// <summary>
    /// Calcs the number of elements in an outer product. 
    /// </summary>
    /// <param name="numberElementsPerDimension">The array giving the number of elements along each dimension of the outer product.</param>
    /// <returns>The product of the numbers in the passed array of integers.</returns>
    public static int CalcOuterProductNrElementsOverall (int[] numberElementsPerDimension)
    {
      if (numberElementsPerDimension.Length <= 0)
      {
        return 0;
      }
      else
      {
        int numberStateCombinations = 1;
        foreach (var numberElements in numberElementsPerDimension)
        {
          numberStateCombinations *= numberElements;
        }
        return numberStateCombinations;
      }
    }

 
    /// <summary>
    /// The recursive method which implements the variable number of for-loops together with processing callbacks to the outerProductProcessor.
    /// </summary>
    /// <param name="dimensionIndex"></param>
    /// <param name="outerProductProcessor"></param>
    private void ProcessOuterProductRecursive (int dimensionIndex, IProcessor outerProductProcessor)
    {
      if (dimensionIndex >= _numberElementsPerDimension.Length)
      {
        return;
      }

      var processingState = new ProcessingState (this, dimensionIndex);


      for (int iCurrentForLoop = 0; iCurrentForLoop < _numberElementsPerDimension[dimensionIndex]; ++iCurrentForLoop)
      {
        processingState.SetCurrentElementIndex (iCurrentForLoop);
        
        outerProductProcessor.SetProcessingState (processingState);
        bool continueProcessingBeforeLoop = outerProductProcessor.DoBeforeLoop ();
        if (!continueProcessingBeforeLoop)
        {
          break;
        }

        ProcessOuterProductRecursive (dimensionIndex + 1, outerProductProcessor);

        outerProductProcessor.SetProcessingState (processingState);
        bool continueProcessingAfterLoop = outerProductProcessor.DoAfterLoop ();
        if (!continueProcessingAfterLoop)
        {
          break;
        }

        ++_numberElementsProcessed;
      }
    }

    /// <summary>
    /// Call to start the processing of each OuterProduct-element.
    /// </summary>
    /// <param name="outerProductProcessor">An OuterProduct-processor which needs to implement the IProcessor interface.</param>
    public void ProcessOuterProduct (IProcessor outerProductProcessor)
    {
      //Init (_numberElementsPerDimension);
      InitProcessing();
      ProcessOuterProductRecursive (0, outerProductProcessor);
    }


  }
}
