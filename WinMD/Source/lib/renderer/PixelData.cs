/*
 * Copyright 2012 ZXing.Net authors
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
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ZXing.Rendering
{
   public sealed class PixelData
   {
      internal PixelData(int width, int heigth, byte[] pixel)
      {
         Heigth = heigth;
         Width = width;
         Pixel = pixel;
      }

      public byte[] Pixel { get; private set; }
      public int Width { get; private set; }
      public int Heigth { get; private set; }

      public object ToBitmap()
      {
         var bmp = new WriteableBitmap(Width, Heigth);

         // Copy data back
         using (var stream = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.AsStream(bmp.PixelBuffer))
         {
            stream.Write(Pixel, 0, Pixel.Length);
         }
         bmp.Invalidate();

         return bmp;
      }
   }
}
