﻿using CarinaStudio;
using CarinaStudio.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Carina.PixelViewer.Media.ImageFilters
{
    /// <summary>
    /// Methods for generating color Loookup Table (LUT).
    /// </summary>
    static class ColorLut
    {
        /// <summary>
        /// Apply arctangent transformation.
        /// </summary>
        /// <param name="lut">LUT.</param>
        /// <param name="intensity">Intensity.</param>
        public static void ArctanTransform(IList<double> lut, double intensity) =>
            ArctanTransform(lut, 0, lut.Count, intensity);

        
        /// <summary>
        /// Apply arctangent transformation on specific range of LUT.
        /// </summary>
        /// <param name="lut">LUT.</param>
        /// <param name="start">Inclusive start of range of LUT.</param>
        /// <param name="end">Exclusive end of range of LUT.</param>
        /// <param name="intensity">Intensity.</param>
        public static void ArctanTransform(IList<double> lut, int start, int end, double intensity)
        {
            // check parameter
            var count = (end - start - 1.0);
            if (count < -0.1)
                return;
            if (Math.Abs(intensity) < 0.001)
                return;
            
            // apply
            var sensitivity = Math.Max(0.1, App.CurrentOrNull?.Configuration?.GetValueOrDefault(ConfigurationKeys.ArctanTransformationSensitivity) ?? ConfigurationKeys.ArctanTransformationSensitivity.DefaultValue);
            var startColor = lut[start];
            var colorThreshold = (1 / count);
            if (intensity >= 0)
            {
                intensity = Math.Pow(intensity, sensitivity);
                var coeff = 1 / Math.Atan(intensity);
                for (var i = start; i < end; ++i)
                {
                    var color = (lut[i] - startColor) / count;
                    if (Math.Abs(color) < colorThreshold)
                        color = color >= 0 ? colorThreshold : -colorThreshold;
                    lut[i] = startColor + (Math.Atan(intensity * color) * coeff * count);
                }
            }
            else
            {
                intensity = Math.Pow(-intensity, sensitivity);
                var coeff = -1 / Math.Atan(-intensity);
                for (var i = start; i < end; ++i)
                {
                    var color = (lut[i] - startColor) / count;
                    if (Math.Abs(color - 1) < colorThreshold)
                        color = color > 1 ? 1 + colorThreshold : 1 - colorThreshold;
                    lut[i] = startColor + ((Math.Atan(intensity * (color - 1)) * coeff + 1) * count);
                }
            }
        }


        /// <summary>
        /// Transform brightness with given transformation function.
        /// </summary>
        /// <param name="histograms">Histograms of original image.</param>
        /// <param name="lut">LUT.</param>
        /// <param name="targetEv">Target brightness in EV.</param>
        /// <param name="function">Transformation function.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task of transformation.</returns>
        public static async Task BrightnessTransformAsync(BitmapHistograms histograms, IList<double> lut, double targetEv, BrightnessTransformationFunction function, CancellationToken cancellationToken = default)
        {
            if (Math.Abs(targetEv) < 0.01)
                return;
            switch (function)
            {
                case BrightnessTransformationFunction.Arctan:
                    {
                        var intensity = await SelectArctanIntensityForBrightnessAsync(histograms, targetEv, cancellationToken);
                        if (double.IsFinite(intensity))
                            ArctanTransform(lut, intensity);
                    }
                    break;
                case BrightnessTransformationFunction.Gamma:
                    {
                        var gamma = await SelectGammaForBrightnessAsync(histograms, targetEv, cancellationToken);
                        if (double.IsFinite(gamma))
                            GammaTransform(lut, gamma);
                    }
                    break;
                case BrightnessTransformationFunction.Linear:
                    Multiply(lut, Math.Pow(2, targetEv));
                    break;
                default:
                    throw new ArgumentException();
            }
        }


        /// <summary>
        /// Build LUT with identity transformation.
        /// </summary>
        /// <param name="targetFormat">Target bitmap format.</param>
        /// <returns>LUT.</returns>
        public static IList<double> BuildIdentity(BitmapFormat targetFormat)
        {
            return (targetFormat switch
            {
                BitmapFormat.Bgra32 => new double[256],
                BitmapFormat.Bgra64 => new double[65536],
                _ => throw new ArgumentException($"Unsupported format: {targetFormat}"),
            }).Also(it => ResetToIdentity(it));
        }


        /// <summary>
        /// Transform contrast with given transformation function.
        /// </summary>
        /// <param name="lut">LUT.</param>
        /// <param name="intensity">Intensity of contrast. Range is [-1.0, 1.0].</param>
        /// <param name="function">Transformation function.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task of transformation.</returns>
        public static Task ContrastTransformAsync(IList<double> lut, double intensity, ContrastTransformationFunction function, CancellationToken cancellationToken = default)
        {
            if (Math.Abs(intensity) < 0.01)
                return Task.CompletedTask;
            if (!double.IsFinite(intensity))
                throw new ArgumentException();
            if (intensity < -1)
                intensity = -1;
            else if (intensity > 1)
                intensity = 1;
            switch (function)
            {
                case ContrastTransformationFunction.Arctan:
                    {
                        var intensityL = intensity * -30;
						var intensityR = intensity * 30;
						ArctanTransform(lut, 0, lut.Count / 2, intensityL);
						ArctanTransform(lut, lut.Count / 2, lut.Count, intensityR);
                    }
                    break;
                case ContrastTransformationFunction.Linear:
                    {
                        var middleColor = (lut.Count - 1) / 2.0;
						var factor = intensity.Let(it => it >= 0 ? it + 1 : -1 / (it - 1));
						Multiply(lut, factor);
						Translate(lut, (1 - factor) * middleColor);
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
            return Task.CompletedTask;
        }


        /// <summary>
        /// Apply gamma transformation.
        /// </summary>
        /// <param name="lut">LUT.</param>
        /// <param name="gamma">Gamma.</param>
        public static void GammaTransform(IList<double> lut, double gamma) =>
            GammaTransform(lut, 0, lut.Count, gamma);


        /// <summary>
        /// Apply gamma transformation on specific range of LUT.
        /// </summary>
        /// <param name="lut">LUT.</param>
        /// <param name="start">Inclusive start of range of LUT.</param>
        /// <param name="end">Exclusive end of range of LUT.</param>
        /// <param name="gamma">Gamma.</param>
        public static void GammaTransform(IList<double> lut, int start, int end, double gamma)
        {
            // check parameter
            var count = (end - start - 1.0);
            if (count < -0.1)
                return;
            if (Math.Abs(gamma - 1) < 0.001)
                return;

            // apply
            var baseColor = lut[start];
            var colorThreshold = (1 / count);
            if (gamma < 1)
            {
                for (var n = start; n < end; ++n)
                {
                    var input = (lut[n] - baseColor) / count;
                    if (Math.Abs(input) < colorThreshold)
                        input = (input >= 0) ? colorThreshold : -colorThreshold;
                    lut[n] = baseColor + (Math.Pow(input, gamma) * count);
                }
            }
            else
            {
                for (var n = start; n < end; ++n)
                {
                    var input = (lut[n] - baseColor) / count;
                    if (Math.Abs(input - 1) < colorThreshold)
                        input = (input >= 1) ? 1 + colorThreshold : 1 - colorThreshold;
                    lut[n] = baseColor + (Math.Pow(input, gamma) * count);
                }
            }
        }


        /// <summary>
        /// Apply multiplication.
        /// </summary>
        /// <param name="lut">LUT.</param>
        /// <param name="factor">Factor.</param>
        public static void Multiply(IList<double> lut, double factor)
        {
            for (var n = lut.Count - 1; n >= 0; --n)
                lut[n] *= factor;
        }


        /// <summary>
        /// Reset LUT to identity transformation.
        /// </summary>
        /// <param name="lut">LUT.</param>
        public static void ResetToIdentity(IList<double> lut)
        {
            for (var n = lut.Count - 1; n >= 0; --n)
                lut[n] = n;
        }


        /// <summary>
        /// Select intensity of arctangen transformation for brightness adjustment by given EV.
        /// </summary>
        /// <param name="histograms">Histograms of original image.</param>
        /// <param name="ev">EV.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Selected intensity.</returns>
        public static Task<double> SelectArctanIntensityForBrightnessAsync(BitmapHistograms histograms, double ev, CancellationToken cancellationToken = default) => Task.Run(() =>
        {
            // check parameter
            if (Math.Abs(ev) < 0.001)
                return 0;
            
            // calculate original luminance
            var colorCount = histograms.ColorCount;
            var pixelCount = histograms.EffectivePixelCount;
            var maxColor = (colorCount - 1.0);
            if (maxColor <= 0.01)
                return double.NaN;
            var histogram = histograms.Luminance;
            var originalLuminance = 0.0;
            for (var i = colorCount - 1; i >= 0; --i)
                originalLuminance += (i / maxColor) * histogram[i] / pixelCount;
            
            // find target intensity
            var sensitivity = Math.Max(0.1, App.CurrentOrNull?.Configuration?.GetValueOrDefault(ConfigurationKeys.ArctanTransformationSensitivity) ?? ConfigurationKeys.ArctanTransformationSensitivity.DefaultValue);
            var targetLuminance = originalLuminance * Math.Pow(2, ev);
            var min = 0.0;
            var max = 0.0;
            if (ev >= 0)
                max = 1000;
            else
                min = -1000;
            while (Math.Abs(max - min) >= 0.01)
            {
                var intensity = (min + max) / 2;
                var luminance = 0.0;
                if (intensity >= 0)
                {
                    var expoIntensity = Math.Pow(intensity, sensitivity);
                    var coeff = 1 / Math.Atan(expoIntensity);
                    for (var i = colorCount - 1; i >= 0; --i)
                    {
                        var color = i / maxColor;
                        luminance += (Math.Atan(expoIntensity * color) * coeff) * histogram[i] / pixelCount;
                    }
                }
                else
                {
                    var expoIntensity = Math.Pow(-intensity, sensitivity);
                    var coeff = -1 / Math.Atan(-expoIntensity);
                    for (var i = colorCount - 1; i >= 0; --i)
                    {
                        var color = i / maxColor;
                        luminance += (Math.Atan(expoIntensity * (color - 1)) * coeff + 1) * histogram[i] / pixelCount;
                    }
                }
                if (Math.Abs(luminance - targetLuminance) <= 0.01)
                    return intensity;
                if (luminance < targetLuminance)
                    min = intensity;
                else
                    max = intensity;
            }
            return min;
        });


        /// <summary>
        /// Select gamma for brightness adjustment by given EV.
        /// </summary>
        /// <param name="histograms">Histograms of original image.</param>
        /// <param name="ev">EV.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Selected Gamma.</returns>
        public static Task<double> SelectGammaForBrightnessAsync(BitmapHistograms histograms, double ev, CancellationToken cancellationToken = default) => Task.Run(() =>
        {
            // check parameter
            if (Math.Abs(ev) < 0.001)
                return 1.0;
            
            // calculate original luminance
            var colorCount = histograms.ColorCount;
            var pixelCount = histograms.EffectivePixelCount;
            var maxColor = (colorCount - 1.0);
            if (maxColor <= 0.01)
                return double.NaN;
            var histogram = histograms.Luminance;
            var originalLuminance = 0.0;
            for (var i = colorCount - 1; i >= 0; --i)
                originalLuminance += (i / maxColor) * histogram[i] / pixelCount;
            
            // find target gamma
            var targetLuminance = originalLuminance * Math.Pow(2, ev);
            var min = 0.0;
            var max = 0.0;
            if (ev >= 0)
            {
                min = 0.01;
                max = 1.0;
            }
            else
            {
                min = 1.0;
                max = 100.0;
            }
            while (Math.Abs(max - min) >= 0.01)
            {
                var gamma = (min + max) / 2;
                var luminance = 0.0;
                for (var i = colorCount - 1; i >= 0; --i)
                    luminance += Math.Pow((i / maxColor), gamma) * histogram[i] / pixelCount;
                if (Math.Abs(luminance - targetLuminance) <= 0.01)
                    return gamma;
                if (luminance < targetLuminance)
                    max = gamma;
                else
                    min = gamma;
            }
            return min;
        });


        /// <summary>
        /// Apply translation.
        /// </summary>
        /// <param name="lut">LUT.</param>
        /// <param name="offset">Offset.</param>
        public static void Translate(IList<double> lut, double offset)
        {
            for (var n = lut.Count - 1; n >= 0; --n)
                lut[n] += offset;
        }
    }


    /// <summary>
    /// Function to transform brightness.
    /// </summary>
    enum BrightnessTransformationFunction
    {
        /// <summary>
        /// Linear transformation.
        /// </summary>
        Linear,
        /// <summary>
        /// Gamma transformation.
        /// </summary>
        Gamma,
        /// <summary>
        /// Arctangen transformation.
        /// </summary>
        Arctan,
    }


    /// <summary>
    /// Function to transform contrast.
    /// </summary>
    enum ContrastTransformationFunction
    {
        /// <summary>
        /// Linear transformation.
        /// </summary>
        Linear,
        /// <summary>
        /// Arctangen transformation.
        /// </summary>
        Arctan,
    }
}
