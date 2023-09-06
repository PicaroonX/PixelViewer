using CarinaStudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Aspose.Svg.Drawing;

namespace Carina.PixelViewer.Media.ImageRenderers
{
    /// <summary>
	/// <see cref="IImageRenderer"/> which supports rendering image with HWC_RGB_F32 based format.
	/// </summary>
    abstract class BaseLabImageRenderer : SinglePlaneImageRenderer
    {
        /// <summary>
		/// Initialize new <see cref="BaseLabImageRenderer"/> instance.
		/// </summary>
		/// <param name="format">Supported format.</param>
		protected BaseLabImageRenderer(ImageFormat format) : base(format)
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
                            this.SelectRgb(component1, component2, component3, out var l, out var a, out var b);

                            var rgb = Color.FromLab(l, a, b);
                            *bitmapPixelPtr = packFunc(
                                ImageProcessing.ClipToUInt16((double)rgb.Blue * 65535),
                                ImageProcessing.ClipToUInt16((double)rgb.Green * 65535),
                                ImageProcessing.ClipToUInt16((double)rgb.Red * 65535),
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
        public override BitmapFormat RenderedFormat => BitmapFormat.Bgra64;


        /// <summary>
		/// Select R, G, B components.
		/// </summary>
		/// <param name="component1">1st component read from source.</param>
		/// <param name="component2">2nd component read from source.</param>
		/// <param name="component3">3rd component read from source.</param>
		/// <param name="r">Selected R.</param>
		/// <param name="g">Selected G.</param>
		/// <param name="b">Selected B.</param>
		protected abstract void SelectRgb(float component1, float component2, float component3, out float l, out float a, out float b);
    }

    /// <summary>
	/// <see cref="IImageRenderer"/> which supports rendering image with HWC_RGB_F32 format.
	/// </summary>
    class LabImageRenderer : BaseLabImageRenderer
    {
        /// <summary>
        /// Initialize new <see cref="HWCRgbF32ImageRenderer"/> instance.
        /// </summary>
        public LabImageRenderer() : base(new ImageFormat(ImageFormatCategory.RGB, "LAB", true, new ImagePlaneDescriptor(12), new string[]{ "LAB", "LAB_" }))
        { }


        /// <inheritdoc/>
        protected override void SelectRgb(float component1, float component2, float component3, out float l, out float a, out float b)
        {
            l = component1;
            a = component2;
            b = component3;
        }
    }
}