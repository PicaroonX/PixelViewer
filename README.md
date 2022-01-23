# PixelViewer [![](https://img.shields.io/github/release-date-pre/carina-studio/PixelViewer?style=flat-square)](https://github.com/carina-studio/PixelViewer/releases/tag/1.105.0.1220) [![](https://img.shields.io/github/last-commit/carina-studio/PixelViewer?style=flat-square)](https://github.com/carina-studio/PixelViewer/commits/master) [![](https://img.shields.io/github/license/carina-studio/PixelViewer?style=flat-square)](https://github.com/carina-studio/PixelViewer/blob/master/LICENSE.md)

PixelViewer is a [.NET](https://dotnet.microsoft.com/) based cross-platform image viewer written by C# which supports reading raw Luminance/YUV/RGB/ARGB/Bayer pixels data from file and rendering it.

## 📥 Download

Operating System                      | Download | Version | Screenshot
:------------------------------------:|:--------:|:-------:|:----------:
Windows 8/10/11                       |[x86](https://github.com/carina-studio/PixelViewer/releases/download/1.105.0.1220/PixelViewer-1.105.0.1220-win-x86.zip) &#124; [x64](https://github.com/carina-studio/PixelViewer/releases/download/1.105.0.1220/PixelViewer-1.105.0.1220-win-x64.zip)|1.105.0.1220 (Preview)|[<img src="https://carina-studio.github.io/PixelViewer/Screenshot_MainWindow_Windows_Thumb.png" alt="Main window (Windows)" width="150"/>](https://carina-studio.github.io/PixelViewer/Screenshot_MainWindow_Windows.png)
Windows 7<br/>*(.NET Runtime needed)* |[x86](https://github.com/carina-studio/PixelViewer/releases/download/1.105.0.1220/PixelViewer-1.105.0.1220-win-x86-fx-dependent.zip) &#124; [x64](https://github.com/carina-studio/PixelViewer/releases/download/1.105.0.1220/PixelViewer-1.105.0.1220-win-x64-fx-dependent.zip)|1.105.0.1220 (Preview)|[<img src="https://carina-studio.github.io/PixelViewer/Screenshot_MainWindow_Windows7_Thumb.png" alt="Main window (Windows 7)" width="150"/>](https://carina-studio.github.io/PixelViewer/Screenshot_MainWindow_Windows7.png)
macOS 11/12                           |[x64](https://github.com/carina-studio/PixelViewer/releases/download/1.105.0.1220/PixelViewer-1.105.0.1220-osx-x64.zip)|1.105.0.1220 (Preview)|[<img src="https://carina-studio.github.io/PixelViewer/Screenshot_MainWindow_macOS_Thumb.png" alt="Main window (macOS)" width="150"/>](https://carina-studio.github.io/PixelViewer/Screenshot_MainWindow_macOS.png)
Linux                                 |[x64](https://github.com/carina-studio/PixelViewer/releases/download/1.105.0.1220/PixelViewer-1.105.0.1220-linux-x64.zip)|1.105.0.1220 (Preview)|[<img src="https://carina-studio.github.io/PixelViewer/Screenshot_MainWindow_Ubuntu_Thumb.png" alt="Main window (Ubuntu)" width="150"/>](https://carina-studio.github.io/PixelViewer/Screenshot_MainWindow_Ubuntu.png)

- [How to Install and Upgrade PixelViewer](https://carina-studio.github.io/PixelViewer/installation_and_upgrade.html)

## 📣 What's Change in 1.105.0.1220
- Support rendering ```10/12-bit MIPI Bayer Pattern``` images.
- Support rendering ```ABGR_16161616```/```ARGB_16161616```/```BGRA_16161616```/```RGBA_16161616``` images.
- Support saving image as ```Raw 32/64-bit BGRA pixels```.
- Upgrade to [.NET 6](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-6).
- Update sampling mode to improve quality of rendering image.
- Automatically change quality of rendering image according to scale of image for better image quality and performance.
- Support running on ```Windows 7``` and ```Windows 8```.
- Support running on ```x86``` PC.
- UX improvement.
- Merge ```BGGR_16```/```GBRG_16```/```GRBG_16```/```RGGB_16``` into single format ```Bayer Pattern (16-bit)``` which allows changing Bayer Pattern.
- Bug fixing.

## ⭐ Supported formats
* Luminance
  * L8
  * L16
* YUV
  * YUV444p
  * P410 (v1.99+)
  * P416 (v1.99+)
  * YUV422p
  * P210 (v1.99+)
  * P216 (v1.99+)
  * UYVY
  * YUVY
  * NV12
  * NV21
  * Y010 (v1.99+)
  * Y016 (v1.99+)
  * I420
  * YV12
  * P010 (v1.99+)
  * P016 (v1.99+)
* RGB
  * BGR_888
  * RGB_565
  * RGB_888
  * BGRX_8888
  * RGBX_8888
  * XBGR_8888
  * XRGB_8888
* ARGB
  * ARGB_8888
  * ABGR_8888
  * BGRA_8888
  * RGBA_8888
  * ARGB_16161616 (v1.105+)
  * ABGR_16161616 (v1.105+)
  * BGRA_16161616 (v1.105+)
  * RGBA_16161616 (v1.105+)
* Bayer Pattern
  * 10-bit MIPI (v1.105+)
  * 12-bit MIPI (v1.105+)
  * 16-bit
  
## ⭐ Supported color spaces (v1.104+)
* sRGB
* DCI-P3
* Adobe RGB
* ITU-R BT.601
* ITU-R BT.2020

## ⭐ Supported functions
* Rendering image from raw pixel file.
* Evaluate image dimensions according to file name, file size and format.
* Specify pixel-stride and row-stride for each plane.
* Specify data offset to image in file. (v1.99+)
* Specify color space of image and screen. (v1.104+)
* Rotate and scale rendered image.
* Navigate to specific image frame in file. (v1.99+)
* Adjust brightness/contrast and color balance. (v1.104+)
* Show histograms of R/G/B and luminance. (v1.102+)
* Demosaicing for Bayer Pattern formats. (v1.103+)
* Save rendered image as PNG file.
* Save rendered image as JPEG/BGRA file. (v1.105+)

## 🤝 Dependencies
* [.NET](https://dotnet.microsoft.com/)
* [AppBase](https://github.com/carina-studio/AppBase)
* [AppSuiteBase](https://github.com/carina-studio/AppSuiteBase)
* [Avalonia](https://github.com/AvaloniaUI/Avalonia)
* [Colourful](https://github.com/tompazourek/Colourful)
* [NLog](https://github.com/NLog/NLog)
* [NUnit](https://github.com/nunit/nunit)
* [ReactiveUI](https://github.com/reactiveui/ReactiveUI)
