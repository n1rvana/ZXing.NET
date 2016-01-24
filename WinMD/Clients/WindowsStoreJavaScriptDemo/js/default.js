// Eine Einführung zur leeren Vorlage finden Sie in der folgenden Dokumentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: Diese Anwendung wurde neu eingeführt. Die Anwendung
               // hier initialisieren.

               // capture an image ...
               //var capture = new Windows.Media.Capture.CameraCaptureUI();
               // ...
               //capture.captureFileAsync(Windows.Media.Capture.CameraCaptureUIMode.photo)

               // ... or open it from the file system
               var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
               openPicker.viewMode = Windows.Storage.Pickers.PickerViewMode.thumbnail;
               openPicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;
               openPicker.fileTypeFilter.replaceAll([".png", ".jpg", ".jpeg"]);
               openPicker.pickSingleFileAsync()
               .then(function (file) {
                  if (file) {
                     // draw the image
                     var canvas = document.getElementById('canvasDecode')
                     var ctx = canvas.getContext('2d');
                     var img = new Image;
                     img.onload = function () {
                        canvas.width = img.width;
                        canvas.height = img.height;
                        ctx.drawImage(img, 0, 0, img.width, img.height);
                     }
                     img.src = URL.createObjectURL(file);
                     // open a stream from the image
                     return file.openAsync(Windows.Storage.FileAccessMode.readWrite);
                  }
               })
               .then(function (stream) {
                  if (stream) {
                     // create a decoder from the image stream
                     return Windows.Graphics.Imaging.BitmapDecoder.createAsync(stream);
                  }
               })
               .done(function (decoder) {
                  if (decoder) {
                     // get the raw pixel data from the decoder
                     decoder.getPixelDataAsync().then(function (pixelDataProvider) {
                        var rawPixels = pixelDataProvider.detachPixelData();
                        var pixels, format; // Assign these in the below switch block.

                        switch (decoder.bitmapPixelFormat) {
                           case Windows.Graphics.Imaging.BitmapPixelFormat.rgba16:
                              // Allocate a typed array with the raw pixel data
                              var pixelBufferView_U8 = new Uint8Array(rawPixels);

                              // Uint16Array provides a typed view into the raw 8 bit pixel data.
                              pixels = new Uint16Array(pixelBufferView_U8.buffer);
                              if (decoder.bitmapAlphaMode == Windows.Graphics.Imaging.BitmapAlphaMode.straight)
                                 format = ZXing.BitmapFormat.rgba32;
                              else
                                 format = ZXing.BitmapFormat.rgb32;
                              break;

                           case Windows.Graphics.Imaging.BitmapPixelFormat.rgba8:
                              // For 8 bit pixel formats, just use the returned pixel array.
                              pixels = rawPixels;
                              if (decoder.bitmapAlphaMode == Windows.Graphics.Imaging.BitmapAlphaMode.straight)
                                 format = ZXing.BitmapFormat.rgba32;
                              else
                                 format = ZXing.BitmapFormat.rgb32;
                              break;

                           case Windows.Graphics.Imaging.BitmapPixelFormat.bgra8:
                              // For 8 bit pixel formats, just use the returned pixel array.
                              pixels = rawPixels;
                              if (decoder.bitmapAlphaMode == Windows.Graphics.Imaging.BitmapAlphaMode.straight)
                                 format = ZXing.BitmapFormat.bgra32;
                              else
                                 format = ZXing.BitmapFormat.bgr32;
                              break;
                        }
                        // create a barcode reader
                        var reader = new ZXing.BarcodeReader();
                        // restrict to one or more supported types, if necessary
                        //reader.options.possibleFormats = [ZXing.BarcodeFormat.qr_CODE];

                        reader.onresultpointfound = function(resultPoint) {
                           // do something with the resultpoint location
                        };
                        // try to decode the raw pixel data
                        var result = reader.decode(pixels, decoder.pixelWidth, decoder.pixelHeight, format);
                        // show the result
                        if (result) {
                           document.getElementById("result").innerText = result.text;
                        }
                        else {
                           document.getElementById("result").innerText = "no barcode found";
                        }
                     });
                  }
               });
            } else {
                // TODO: Diese Anwendung war angehalten und wurde reaktiviert.
                // Anwendungszustand hier wiederherstellen.
            }
            args.setPromise(WinJS.UI.processAll());
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: Diese Anwendung wird gleich angehalten. Jeden Zustand,
        // der über Anhaltevorgänge hinweg beibehalten muss, hier speichern. Dazu kann das
        // WinJS.Application.sessionState-Objekt verwendet werden, das automatisch
        // über ein Anhalten hinweg gespeichert und wiederhergestellt wird. Wenn ein asynchroner
        // Vorgang vor dem Anhalten der Anwendung abgeschlossen werden muss,
        // args.setPromise() aufrufen.
    };
    
    app.start();
})();

function generate_barcode() {
   // get the content which the user puts into the textbox
   var content = document.getElementById("input").value;

   // create the barcode writer and set some options
   var writer = new ZXing.BarcodeWriter();
   writer.options = new ZXing.Common.EncodingOptions();
   writer.options.height = 200;
   writer.options.width = 200;
   writer.format = ZXing.BarcodeFormat.qr_CODE;

   // encode the content to a byte array with 4 byte per pixel as BGRA
   var imagePixelData = writer.write(content);

   // draw the pixel data to the canvas
   var ctx = document.getElementById('canvasEncode').getContext('2d');
   var imageData = ctx.createImageData(imagePixelData.width, imagePixelData.heigth);
   var pixel = imagePixelData.pixel
   for (var index = 0; index < pixel.length; index++) {
      imageData.data[index] = pixel[index];
   }
   ctx.putImageData(imageData, 0, 0);
}

