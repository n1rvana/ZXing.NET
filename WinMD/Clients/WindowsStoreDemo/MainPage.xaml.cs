using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;

using ZXing;
using ZXing.Common;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace WindowsStoreDemo
{
   /// <summary>
   /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
   /// </summary>
   public sealed partial class MainPage : Page
   {
      public MainPage()
      {
         this.InitializeComponent();
      }

      /// <summary>
      /// Wird aufgerufen, wenn diese Seite in einem Rahmen angezeigt werden soll.
      /// </summary>
      /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde. Die
      /// Parametereigenschaft wird normalerweise zum Konfigurieren der Seite verwendet.</param>
      protected override void OnNavigatedTo(NavigationEventArgs e)
      {
         DecodeStaticResource();
      }

      private async System.Threading.Tasks.Task DecodeStaticResource()
      {
         var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\1.png");
         var stream = await file.OpenReadAsync();
         // initialize with 1,1 to get the current size of the image
         var writeableBmp = new WriteableBitmap(1, 1);
         writeableBmp.SetSource(stream);
         // and create it again because otherwise the WB isn't fully initialized and decoding
         // results in a IndexOutOfRange
         writeableBmp = new WriteableBitmap(writeableBmp.PixelWidth, writeableBmp.PixelHeight);
         stream.Seek(0);
         writeableBmp.SetSource(stream);

         var result = ScanBitmap(writeableBmp);
         if (result != null)
         {
            ScanResult.Text += result.Text;
         }
         return;
      }

      private Result ScanBitmap(WriteableBitmap writeableBmp)
      {
         var barcodeReader = new BarcodeReader
         {
            AutoRotate = true,
            Options = new DecodingOptions
               {
                  TryHarder = true,
                  // restrict to one or more supported types, if necessary
                  //PossibleFormats = new []
                  //   {
                  //      BarcodeFormat.QR_CODE
                  //   }
               }
         };
         var result = barcodeReader.Decode(writeableBmp);

         if (result != null)
         {
            CaptureImage.Source = writeableBmp;
         }

         return result;
      }
   }
}