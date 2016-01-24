//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace WindowsStoreCppDemo;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace ZXing;
using namespace ZXing::Common;
using namespace ZXing::QrCode;
using namespace ZXing::Rendering;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Storage::Streams ;
using namespace Windows::Graphics::Imaging;
using namespace Microsoft::WRL;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

MainPage::MainPage()
{
	InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	(void) e;	// Unused parameter
}


void WindowsStoreCppDemo::MainPage::btnName_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	
	String^ strName =txtInputText->Text;
	if (strName->IsEmpty())
	{
		auto dialog = ref new Windows::UI::Popups::MessageDialog("Plz Enter Some Text ");
		dialog-> ShowAsync();
		txtInputText->Focus(Windows::UI::Xaml::FocusState::Pointer);	
		return;
	}
	BarcodeWriter^ barcodeWriter = ref new BarcodeWriter();
	barcodeWriter->Format = BarcodeFormat::QR_CODE;
   PixelData^ pixelData = barcodeWriter->Write(strName);
   WriteableBitmap^ wbitmap = (WriteableBitmap^)pixelData->ToBitmap();
   /*
   // alternative way
   WriteableBitmap^ wbitmap = ref new WriteableBitmap(pixelData->Width, pixelData->Heigth);

   byte* pDstPixels;
   // Get access to the pixels
   IBuffer^ buffer = wbitmap->PixelBuffer;

   // Obtain IBufferByteAccess
   ComPtr<IBufferByteAccess> pBufferByteAccess;
   ComPtr<IUnknown> pBuffer((IUnknown*)buffer);
   pBuffer.As(&pBufferByteAccess);
    
   // Get pointer to pixel bytes
   pBufferByteAccess->Buffer(&pDstPixels);
   // very slow, but I have no better idea at the moment
   for (int index = 0; index < pixelData->Pixel->Length; index++)
   {
      pDstPixels[index] = pixelData->Pixel[index];
   }
   */
   this->lastBitmap = wbitmap;
	imgPlaceHolder->Source = lastBitmap;
}


void WindowsStoreCppDemo::MainPage::btnDecode_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
   if (this->lastBitmap == nullptr)
   {
      txtDecodedText->Text = "Please generate a barcode first.";
      return;
   }

	BarcodeReader^ barcodeReader = ref new BarcodeReader();
   
   // restrict to one or more supported types, if necessary
   auto possibleFormats = ref new Array<BarcodeFormat>(1);
   possibleFormats[0] = BarcodeFormat::QR_CODE;
   barcodeReader->Options->PossibleFormats = possibleFormats;
   
   Result^ result = barcodeReader->Decode(this->lastBitmap);
   if (result != nullptr)
   {
      txtDecodedText->Text = result->Text;
   }
   else
   {
      txtDecodedText->Text = "No barcode found";
   }
}
