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
   /// <summary> <p>Encapsulates an alignment pattern, which are the smaller square patterns found in
   /// all but the simplest QR Codes.</p>
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   internal sealed class AlignmentPattern
   {
      private float estimatedModuleSize;
      private readonly float x;
      private readonly float y;
      private readonly byte[] bytesX;
      private readonly byte[] bytesY;
      private String toString;

      internal AlignmentPattern(float posX, float posY, float estimatedModuleSize)
      {
         this.estimatedModuleSize = estimatedModuleSize;
         this.x = posX;
         this.y = posY;
         // calculate only once for GetHashCode
         bytesX = BitConverter.GetBytes(x);
         bytesY = BitConverter.GetBytes(y);
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
      /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
      /// </summary>
      /// <param name="other">The <see cref="System.Object"/> to compare with this instance.</param>
      /// <returns>
      ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
      /// </returns>
      public sealed override bool Equals(Object other)
      {
         var otherPoint = other as AlignmentPattern;
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

      public static implicit operator ResultPoint(AlignmentPattern point)
      {
         if (point == null)
            return null;
         return new ResultPoint(point.X, point.Y);
      }

      /// <summary> <p>Determines if this alignment pattern "about equals" an alignment pattern at the stated
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
      /// with a new estimate. It returns a new {@code FinderPattern} containing an average of the two.
      /// </summary>
      /// <param name="i">The i.</param>
      /// <param name="j">The j.</param>
      /// <param name="newModuleSize">New size of the module.</param>
      /// <returns></returns>
      internal AlignmentPattern combineEstimate(float i, float j, float newModuleSize)
      {
         float combinedX = (X + j) / 2.0f;
         float combinedY = (Y + i) / 2.0f;
         float combinedModuleSize = (estimatedModuleSize + newModuleSize) / 2.0f;
         return new AlignmentPattern(combinedX, combinedY, combinedModuleSize);
      }
   }
}