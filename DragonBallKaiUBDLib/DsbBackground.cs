﻿using AuroraLib.Compression.Algorithms;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonBallKaiUBDLib
{
    public class DsbBackground
    {
        private const string MAGIC = "DSB";

        public byte[] Data { get; set; }
        public byte[] PixelData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BitDepth { get; set; }
        public int PaletteLength { get; set; }

        public List<SKColor> Palette { get; set; } = [];
        public DsbBackground(IEnumerable<byte> data, string name)
        {
            if (!IO.ReadAsciiString(data, 0x00).Equals(MAGIC))
            {
                throw new ArgumentException("Not a DSO texture!");
            }

            Width = 1 << data.ElementAt(0x0D);
            Height = 1 << data.ElementAt(0x0E);
            PaletteLength = IO.ReadInt(data, 0x14);
            BitDepth = PaletteLength == 0x200 ? 8 : 4;

            LZ10 lz10 = new();
            Data = [.. data.Take(0x40), .. lz10.Decompress([.. data.Skip(0x40)])];
            for (int i = 0x40; i < PaletteLength + 0x40; i += 2)
            {
                short color = IO.ReadShort(Data, i);
                Palette.Add(new SKColor((byte)((color & 0x1F) << 3), (byte)((color >> 5 & 0x1F) << 3), (byte)((color >> 10 & 0x1F) << 3)));
            }
            PixelData = [.. Data.Skip(0x240)];
        }

        public SKBitmap GetImage()
        {
            if (PixelData.Length == 0)
            {
                return new SKBitmap(0, 0);
            }
            SKBitmap tileBitmap = new(Width, Height);
            SKBitmap tiles = GetImage(8);
            SKCanvas tileCanvas = new(tileBitmap);
            int currentTile = 0;
            for (int y = 0; y < Height; y += 32)
            {
                for (int x = 0; x < Width; x += 64)
                {
                    for (int yy = 0; yy < 32; yy += 8)
                    {
                        for (int xx = 0; xx < 64; xx += 8)
                        {
                            SKRect crop = new(0, currentTile * 8, 8, (currentTile + 1) * 8);
                            SKRect dest = new(x + xx, y + yy, x + xx + 8, y + yy + 8);
                            tileCanvas.DrawBitmap(tiles, crop, dest);
                            currentTile++;
                        }
                    }
                }
            }
            tileCanvas.Flush();

            return tileBitmap;
        }

        private SKBitmap GetImage(int width, int transparentIndex = -1, int paletteOffset = 0)
        {
            SKColor originalColor = SKColors.Black;
            if (transparentIndex >= 0)
            {
                originalColor = Palette[transparentIndex];
                Palette[transparentIndex] = SKColors.Transparent;
            }
            int height;
            if (width == -1)
            {
                width = Width;
                height = Height;
            }
            else
            {
                height = PixelData.Length / (width / (BitDepth == 4 ? 2 : 1));
            }
            SKBitmap bitmap = new(width, height);
            int pixelIndex = 0;
            for (int row = 0; row < height / 8 && pixelIndex < PixelData.Length; row++)
            {
                for (int col = 0; col < width / 8 && pixelIndex < PixelData.Length; col++)
                {
                    for (int ypix = 0; ypix < 8 && pixelIndex < PixelData.Length; ypix++)
                    {
                        if (BitDepth == 4)
                        {
                            for (int xpix = 0; xpix < 4 && pixelIndex < PixelData.Length; xpix++)
                            {
                                for (int xypix = 0; xypix < 2 && pixelIndex < PixelData.Length; xypix++)
                                {
                                    bitmap.SetPixel(col * 8 + xpix * 2 + xypix, row * 8 + ypix,
                                        Palette[(PixelData[pixelIndex] >> xypix * 4 & 0xF) + paletteOffset]);
                                }
                                pixelIndex++;
                            }
                        }
                        else
                        {
                            for (int xpix = 0; xpix < 8 && pixelIndex < PixelData.Length; xpix++)
                            {
                                bitmap.SetPixel(col * 8 + xpix, row * 8 + ypix,
                                    Palette[PixelData[pixelIndex++]]);
                            }
                        }
                    }
                }
            }
            if (transparentIndex >= 0)
            {
                Palette[transparentIndex] = originalColor;
            }
            return bitmap;
        }
    }
}
