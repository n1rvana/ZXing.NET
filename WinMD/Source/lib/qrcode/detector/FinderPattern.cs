/*
* Copyright 2007 ZXing authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;

namespace ZXing.QrCode.Internal
{
   /// <summary>
   /// <p>Encapsulates a finder pattern, which are the three square patterns found in
   /// the corners of QR Codes. It also encapsulates a count of similar finder patterns,
   /// as a convenience to the finder's bookkeeping.</p>
   /// </summary>
   /// <author>Sean Owen</author>
   internal sealed class FinderPattern
   {
      private readonly float estimatedModuleSize;
      private int count;
      private readonly float x;
      private readonly float y;
      private readonly byte[] bytesX;
      private readonly byte[] bytesY;
      private String toString;

      internal FinderPattern(float posX, float posY, float estimatedModuleSize)
         : this(posX, posY, estimatedModuleSize, 1)
      {
         this.estimatedModuleSize = estimatedModuleSize;
         this.count = 1;
      }

      internal FinderPattern(float posX, float posY, float estimatedModuleSize, int count)
      {
         this.estimatedModuleSize = estimatedModuleSize;
         this.count = count;
         this.x = posX;
         this.y = posY;
      }

      /// <summary>
      /// Gets the X.
      /// </summary>
      public float X
      {
         get
         {
            return x;
         }
      }

      /// <summary>
      /// Gets the Y.
      /// </summary>
      public float Y
      {
         get
         {
            return y;
         }
      }

      /// <summary>
      /// Gets the size of the estimated module.
      /// </summary>
      /// <value>
      /// The size of the estimated module.
      /// </value>
      public float EstimatedModuleSize
      {
         get
         {
            return estimatedModuleSize;
         }
      }

      /// <summary>
      /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
      /// </summary>
      /// <param name="other">The <see cref="System.Object"/> to compare with this instance.</param>
      /// <returns>
      ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
      /// </returns>
      public sealed override bool Equals(Object other)
      {
         var otherPoint = other as FinderPattern;
         if (otherPoint == null)
            return false;
         return x == otherPoint.x && y == otherPoint.y;
      }

      /// <summary>
      /// Returns a hash code for this instance.
      /// </summary>
      /// <returns>
      /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
      /// </returns>
      public sealed override int GetHashCode()
      {
         return 31 * ((bytesX[0] << 24) + (bytesX[1] << 16) + (bytesX[2] << 8) + bytesX[3]) +
                      (bytesY[0] << 24) + (bytesY[1] << 16) + (bytesY[2] << 8) + bytesY[3];
      }

      /// <summary>
      /// Returns a <see cref="System.String"/> that represents this instance.
      /// </summary>
      /// <returns>
      /// A <see cref="System.String"/> that represents this instance.
      /// </returns>
      public sealed override String ToString()
      {
         if (toString == null)
         {
            var result = new System.Text.StringBuilder(25);
            result.AppendFormat(System.Globalization.CultureInfo.CurrentUICulture, "({0}, {1})", x, y);
            toString = result.ToString();
         }
         return toString;
      }

      internal int Count
      {
         get
         {
            return count;
         }
      }

      /*
      internal void incrementCount()
      {
         this.count++;
      }
      */

      /// <summary> <p>Determines if this finder pattern "about equals" a finder pattern at the stated
      /// position and size -- meaning, it is at nearly the same center with nearly the same size.</p>
      /// </summary>
      internal bool aboutEquals(float moduleSize, float i, float j)
      {
         if (Math.Abs(i - Y) <= moduleSize && Math.Abs(j - X) <= moduleSize)
         {
            float moduleSizeDiff = Math.Abs(moduleSize - estimatedModuleSize);
            return moduleSizeDiff <= 1.0f || moduleSizeDiff <= estimatedModuleSize;

         }
         return false;
      }

      /// <summary>
      /// Combines this object's current estimate of a finder pattern position and module size
      /// with a new estimate. It returns a new {@code FinderPattern} containing a weighted average
      /// based on count.
      /// </summary>
      /// <param name="i">The i.</param>
      /// <param name="j">The j.</param>
      /// <param name="newModuleSize">New size of the module.</param>
      /// <returns></returns>
      internal FinderPattern combineEstimate(float i, float j, float newModuleSize)
      {
         int combinedCount = count + 1;
         float combinedX = (count * X + j) / combinedCount;
         float combinedY = (count * Y + i) / combinedCount;
         float combinedModuleSize = (count * estimatedModuleSize + newModuleSize) / combinedCount;
         return new FinderPattern(combinedX, combinedY, combinedModuleSize, combinedCount);
      }

      public static implicit operator ResultPoint(FinderPattern point)
      {
         return new ResultPoint(point.X, point.Y);
      }

      /// <returns>
      /// distance between two points
      /// </returns>
      public static float distance(FinderPattern pattern1, FinderPattern pattern2)
      {
         return ZXing.Common.Detector.MathUtils.distance(pattern1.x, pattern1.y, pattern2.x, pattern2.y);
      }

      /// <summary>
      /// Orders an array of three ResultPoints in an order [A,B,C] such that AB &lt; AC and
      /// BC &lt; AC and the angle between BC and BA is less than 180 degrees.
      /// </summary>
      internal static void orderBestPatterns(FinderPattern[] patterns)
      {
         // Find distances between pattern centers
         float zeroOneDistance = distance(patterns[0], patterns[1]);
         float oneTwoDistance = distance(patterns[1], patterns[2]);
         float zeroTwoDistance = distance(patterns[0], patterns[2]);

         FinderPattern pointA, pointB, pointC;
         // Assume one closest to other two is B; A and C will just be guesses at first
         if (oneTwoDistance >= zeroOneDistance && oneTwoDistance >= zeroTwoDistance)
         {
            pointB = patterns[0];
            pointA = patterns[1];
            pointC = patterns[2];
         }
         else if (zeroTwoDistance >= oneTwoDistance && zeroTwoDistance >= zeroOneDistance)
         {
            pointB = patterns[1];
            pointA = patterns[0];
            pointC = patterns[2];
         }
         else
         {
            pointB = patterns[2];
            pointA = patterns[0];
            pointC = patterns[1];
         }

         // Use cross product to figure out whether A and C are correct or flipped.
         // This asks whether BC x BA has a positive z component, which is the arrangement
         // we want for A, B, C. If it's negative, then we've got it flipped around and
         // should swap A and C.
         if (crossProductZ(pointA, pointB, pointC) < 0.0f)
         {
            FinderPattern temp = pointA;
            pointA = pointC;
            pointC = temp;
         }

         patterns[0] = pointA;
         patterns[1] = pointB;
         patterns[2] = pointC;
      }

      /// <summary>
      /// Returns the z component of the cross product between vectors BC and BA.
      /// </summary>
      private static float crossProductZ(FinderPattern pointA, FinderPattern pointB, FinderPattern pointC)
      {
         float bX = pointB.x;
         float bY = pointB.y;
         return ((pointC.x - bX) * (pointA.y - bY)) - ((pointC.y - bY) * (pointA.x - bX));
      }
   }
}