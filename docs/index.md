[![](https://img.shields.io/github/release-date-pre/carina-studio/PixelViewer?style=flat-square)](https://github.com/carina-studio/PixelViewer/releases/tag/1.104.0.1123) [![](https://img.shields.io/github/last-commit/carina-studio/PixelViewer?style=flat-square)](https://github.com/carina-studio/PixelViewer/commits/master) [![](https://img.shields.io/github/license/carina-studio/PixelViewer?style=flat-square)](https://github.com/carina-studio/PixelViewer/blob/master/LICENSE.md)

PixelViewer is a [.NET](https://dotnet.microsoft.com/) based cross-platform image viewer written by C# which supports reading raw Luminance/YUV/RGB/ARGB/Bayer pixels data from file and rendering it.

[![Release](https://img.shields.io/github/v/release/carina-studio/PixelViewer?include_prereleases&style=for-the-badge&color=cyan&label=Preview)](https://github.com/carina-studio/PixelViewer/releases/1.104.0.1123)

&nbsp;    | Windows 10/11 | Linux | macOS
:--------:|:-------------:|:-----:|:-----:
Download  |[x64](https://github.com/carina-studio/PixelViewer/releases/download/1.104.0.1123/PixelViewer-1.104.0.1123-win-x64.zip)|[x64](https://github.com/carina-studio/PixelViewer/releases/download/1.104.0.1123/PixelViewer-1.104.0.1123-linux-x64.zip)|[x64](https://github.com/carina-studio/PixelViewer/releases/download/1.104.0.1123/PixelViewer-1.104.0.1123-osx-x64.zip)
Screenshot|<img src="https://carina-studio.github.io/PixelViewer/Screenshot_MainWindow_Windows_Thumb.png" alt="Main window (Windows)" width="250"/>|<img src="https://carina-studio.github.io/PixelViewer/Screenshot_MainWindow_Ubuntu_Thumb.png" alt="Main window (Ubuntu)" width="250"/>|<img src="https://carina-studio.github.io/PixelViewer/Screenshot_MainWindow_macOS_Thumb.png" alt="Main window (macOS)" width="250"/>

## ⭐Supported formats
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
* Bayer
  * BGGR_16
  * GBRG_16
  * GRBG_16
  * RGGB_16
  
## ⭐Supported color spaces (v1.104+)
* sRGB
* DCI-P3
* Adobe RGB
* ITU-R BT.601
* ITU-R BT.2020

## ⭐Supported functions
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

## 📔Topics
- [How to Install and Upgrade PixelViewer](installation_and_upgrade.md)

## 📜Privacy Policy
- [English](privacy_policy.md)
- [正體中文 (台灣)](privacy_policy_zh-TW.md)
