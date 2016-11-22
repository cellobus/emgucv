﻿//----------------------------------------------------------------------------
//  Copyright (C) 2004-2016 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Diagnostics;
//using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Emgu.CV.Structure;
using Emgu.Util;

namespace Emgu.CV
{
   /// <summary>
   /// The base class for camera response calibration algorithms.
   /// </summary>
   public abstract class CalibrateCRF : UnmanagedObject
   {
      /// <summary>
      /// The pointer the the calibrateCRF object
      /// </summary>
      protected IntPtr _calibrateCRFPtr;

      /// <summary>
      /// Recovers inverse camera response.
      /// </summary>
      /// <param name="src">Vector of input images</param>
      /// <param name="dst">256x1 matrix with inverse camera response function</param>
      /// <param name="times">Vector of exposure time values for each image</param>
      public void Process(IInputArray src, IOutputArray dst, IInputArray times)
      {
         using (InputArray iaSrc = src.GetInputArray())
         using (OutputArray oaDst = dst.GetOutputArray())
         using (InputArray iaTimes = times.GetInputArray())
         {
            CvInvoke.cveCalibrateCRFProcess(_calibrateCRFPtr, iaSrc, oaDst, iaTimes);
         }
      }

      /// <summary>
      /// Reset the pointer that points to the CalibrateCRF object.
      /// </summary>
      protected override void DisposeObject()
      {
         _calibrateCRFPtr = IntPtr.Zero;
      }
   }

   public class CalibrateDebevec : CalibrateCRF
   {
      public CalibrateDebevec(int samples, float lambda, bool random)
      {
         _ptr = CvInvoke.cveCalibrateDebevecCreate(samples, lambda, random, ref _calibrateCRFPtr);
      }

      /// <summary>
      /// Release the unmanaged memory associated with this CalibrateCRF object
      /// </summary>
      protected override void DisposeObject()
      {
         if (IntPtr.Zero != _ptr)
         {
            CvInvoke.cveCalibrateDebevecRelease(ref _ptr);
         }
         base.DisposeObject();
      }
   }

   /// <summary>
   /// Inverse camera response function is extracted for each brightness value by minimizing an objective function as linear system. This algorithm uses all image pixels.
   /// </summary>
   public class CalibrateRobertson : CalibrateCRF
   {
      /// <summary>
      /// Creates CalibrateRobertson object.
      /// </summary>
      /// <param name="maxIter">maximal number of Gauss-Seidel solver iterations.</param>
      /// <param name="threshold">get difference between results of two successive steps of the minimization.</param>
      public CalibrateRobertson(int maxIter, float threshold)
      {
         _ptr = CvInvoke.cveCalibrateRobertsonCreate(maxIter, threshold, ref _calibrateCRFPtr);
      }

      /// <summary>
      /// Release the unmanaged memory associated with this CalibrateCRF object
      /// </summary>
      protected override void DisposeObject()
      {
         if (IntPtr.Zero != _ptr)
         {
            CvInvoke.cveCalibrateRobertsonRelease(ref _ptr);
         }
         base.DisposeObject();
      }
   }

   public static partial class CvInvoke
   {
      [DllImport(CvInvoke.ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveCalibrateCRFProcess(
         IntPtr calibrateCRF, IntPtr src, IntPtr dst, IntPtr times);

      [DllImport(CvInvoke.ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern IntPtr cveCalibrateDebevecCreate(
         int samples, 
         float lambda, 
         [MarshalAs(CvInvoke.BoolMarshalType)]
         bool random, 
         ref IntPtr calibrateCRF);

      [DllImport(CvInvoke.ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveCalibrateDebevecRelease(ref IntPtr calibrateDebevec);

      [DllImport(CvInvoke.ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern IntPtr cveCalibrateRobertsonCreate(int maxIter, float threshold, ref IntPtr calibrateCRF);

      [DllImport(CvInvoke.ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveCalibrateRobertsonRelease(ref IntPtr calibrateRobertson);
   }
}
