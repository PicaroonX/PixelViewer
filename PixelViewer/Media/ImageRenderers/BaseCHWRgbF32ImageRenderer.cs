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
	/// <see cref="IImageRenderer"/> which supports rendering image with CHW_RGB_F32 based format.
	/// </summary>
    abstract class BaseCHWRgbF32ImageRenderer : SinglePlaneImageRenderer
    {
        /// <summary>
		/// Initialize new <see cref="BaseCHWRgbF32ImageRenderer"/> instance.
		/// </summary>
		/// <param name="format">Supported format.</param>
		protected BaseCHWRgbF32ImageRenderer(ImageFormat format) : base(format)
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
            var numTotalPixels = width * height;
            var numChannelBytes = numTotalPixels * sizeof(float);
            var channelR = new byte[numChannelBytes];
            var channelG = new byte[numChannelBytes];
            var channelB = new byte[numChannelBytes];
       
            imageStream.Read(channelB);
            imageStream.Read(channelG);
            imageStream.Read(channelR);

            bitmapBuffer.Memory.Pin((bitmapBaseAddress) =>
            {
                var bitmapRowPtr = (byte*)bitmapBaseAddress;
                var bitmapBasePixelPtr = (ulong*)bitmapRowPtr;

                for (int i = 0; i < numTotalPixels; ++i)
                {
                    var bitmapPixelPtr = bitmapBasePixelPtr + i;
                    var index = i * 4;
                    var component1 = BitConverter.ToSingle(channelR, index);
                    var component2 = BitConverter.ToSingle(channelG, index);
                    var component3 = BitConverter.ToSingle(channelB, index);
                    this.SelectRgb(component1, component2, component3, out var r, out var g, out var b);
                    *bitmapPixelPtr = packFunc(
                        ImageProcessing.ClipToUInt16((double) b * 65535),
                        ImageProcessing.ClipToUInt16((double) g * 65535),
                        ImageProcessing.ClipToUInt16((double) r * 65535),
                        ImageProcessing.ClipToUInt16((double) 1 * 65535));
                }
            });
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
	/// <see cref="IImageRenderer"/> which supports rendering image with CHW_RGB_F32 format.
	/// </summary>
    class CHWRgbF32ImageRenderer : BaseCHWRgbF32ImageRenderer
    {
        /// <summary>
        /// Initialize new <see cref="CHWRgbF32ImageRenderer"/> instance.
        /// </summary>
        public CHWRgbF32ImageRenderer() : base(new ImageFormat(ImageFormatCategory.RGB, "CHW_RGB_F32", true, new ImagePlaneDescriptor(12), new string[]{ "CHWRGBF32", "CHW_RGB_F32" }))
        { }


        /// <inheritdoc/>
        protected override void SelectRgb(float component1, float component2, float component3, out float r, out float g, out float b)
        {
            r = component1;
            g = component2;
            b = component3;
        }
    }

    class CHWBgrF32ImageRenderer : BaseCHWRgbF32ImageRenderer
    {
        /// <summary>
        /// Initialize new <see cref="CHWRgbF32ImageRenderer"/> instance.
        /// </summary>
        public CHWBgrF32ImageRenderer() : base(new ImageFormat(ImageFormatCategory.RGB, "CHW_BGR_F32", true, new ImagePlaneDescriptor(12), new string[] { "CHWBGRF32", "CHW_BGR_F32" }))
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