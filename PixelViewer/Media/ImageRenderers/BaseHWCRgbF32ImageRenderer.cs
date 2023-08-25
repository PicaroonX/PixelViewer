using CarinaStudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Carina.PixelViewer.Media.ImageRenderers
{
    /// <summary>
	/// <see cref="IImageRenderer"/> which supports rendering image with HWC_RGB_F32 based format.
	/// </summary>
    abstract class BaseHWCRgbF32ImageRenderer : SinglePlaneImageRenderer
    {
        /// <summary>
		/// Initialize new <see cref="BaseHWCRgbF32ImageRenderer"/> instance.
		/// </summary>
		/// <param name="format">Supported format.</param>
		protected BaseHWCRgbF32ImageRenderer(ImageFormat format) : base(format)
		{ }


        /// <inheritdoc/>
        protected override unsafe ImageRenderingResult OnRender(IImageDataSource source, Stream imageStream, IBitmapBuffer bitmapBuffer, ImageRenderingOptions renderingOptions, IList<ImagePlaneOptions> planeOptions, CancellationToken cancellationToken)
		{
			// get parameters
			var width = bitmapBuffer.Width;
			var height = bitmapBuffer.Height;
			var pixelStride = planeOptions[0].PixelStride;
			var rowStride = planeOptions[0].RowStride;
			if (pixelStride <= 0 || rowStride <= 0)
				throw new ArgumentException($"Invalid pixel/row stride: {pixelStride}/{rowStride}.");

			// prepare packing function
			// var extractComponentFunc = renderingOptions.ByteOrdering == ByteOrdering.LittleEndian
			// 	? new Func<byte, byte, float>((b1, b2) => BitConverter.UInt16BitsTofloat((ushort)((b2 << 8) | b1)))
			// 	: new Func<byte, byte, float>((b1, b2) => BitConverter.UInt16BitsTofloat((ushort)((b1 << 8) | b2)));
			var packFunc = ImageProcessing.SelectBgra64Packing();

			// render
			var srcRow = new byte[rowStride];
			fixed (byte* srcRowAddress = srcRow)
			{
				var srcRowPtr = srcRowAddress;
				bitmapBuffer.Memory.Pin((bitmapBaseAddress) =>
				{
					var bitmapRowPtr = (byte*)bitmapBaseAddress;
					var bitmapRowStride = bitmapBuffer.RowBytes;
					for (var y = height; ; --y, bitmapRowPtr += bitmapRowStride)
					{
                        
                        var isLastRow = (imageStream.Read(srcRow) < rowStride || y == 1);
						var srcPixelPtr = srcRowPtr;
						var bitmapPixelPtr = (ulong*)bitmapRowPtr;
						for (var x = width; x > 0; --x, srcPixelPtr += pixelStride, ++bitmapPixelPtr)
						{
                            byte[] bytes = new byte[pixelStride];
                            for (int i = 0; i < pixelStride; i++)
                                bytes[i] = Marshal.ReadByte((nint) srcPixelPtr, i);

                            var component1 = BitConverter.ToSingle(bytes, 0);
                            var component2 = BitConverter.ToSingle(bytes, 4);
                            var component3 = BitConverter.ToSingle(bytes, 8);
                            this.SelectRgb(component1, component2, component3, out var r, out var g, out var b);
							*bitmapPixelPtr = packFunc(
                                ImageProcessing.ClipToUInt16((double)b * 65535), 
                                ImageProcessing.ClipToUInt16((double)g * 65535), 
                                ImageProcessing.ClipToUInt16((double)r * 65535), 
                                ImageProcessing.ClipToUInt16((double)1 * 65535));
						}
						if (isLastRow || cancellationToken.IsCancellationRequested)
							break;
						Array.Clear(srcRow, 0, rowStride);
					}
				});
			}

			// complete
			return new ImageRenderingResult();
		}


        /// <inheritdoc/>
        public override Task<BitmapFormat> SelectRenderedFormatAsync(IImageDataSource source, ImageRenderingOptions renderingOptions, IList<ImagePlaneOptions> planeOptions, CancellationToken cancellationToken = default) =>
            Task.FromResult(BitmapFormat.Bgra64);


        /// <summary>
		/// Select R, G, B components.
		/// </summary>
		/// <param name="component1">1st component read from source.</param>
		/// <param name="component2">2nd component read from source.</param>
		/// <param name="component3">3rd component read from source.</param>
		/// <param name="r">Selected R.</param>
		/// <param name="g">Selected G.</param>
		/// <param name="b">Selected B.</param>
		protected abstract void SelectRgb(float component1, float component2, float component3, out float r, out float g, out float b);
    }

    /// <summary>
	/// <see cref="IImageRenderer"/> which supports rendering image with HWC_RGB_F32 format.
	/// </summary>
    class HWCRgbF32ImageRenderer : BaseHWCRgbF32ImageRenderer
    {
        /// <summary>
        /// Initialize new <see cref="HWCRgbF32ImageRenderer"/> instance.
        /// </summary>
        public HWCRgbF32ImageRenderer() : base(new ImageFormat(ImageFormatCategory.RGB, "HWC_RGB_F32", true, new ImagePlaneDescriptor(12), new string[]{ "HWCRGBF32", "HWC_RGB_F32" }))
        { }


        /// <inheritdoc/>
        protected override void SelectRgb(float component1, float component2, float component3, out float r, out float g, out float b)
        {
            r = component1;
            g = component2;
            b = component3;
        }
    }

    class HWCBgrF32ImageRenderer : BaseHWCRgbF32ImageRenderer
    {
        /// <summary>
        /// Initialize new <see cref="HWCRgbF32ImageRenderer"/> instance.
        /// </summary>
        public HWCBgrF32ImageRenderer() : base(new ImageFormat(ImageFormatCategory.RGB, "HWC_BGR_F32", true, new ImagePlaneDescriptor(12), new string[] { "HWCBGRF32", "HWC_BGR_F32" }))
        { }


        /// <inheritdoc/>
        protected override void SelectRgb(float component1, float component2, float component3, out float r, out float g, out float b)
        {
            r = component3;
            g = component2;
            b = component1;
        }
    }
}