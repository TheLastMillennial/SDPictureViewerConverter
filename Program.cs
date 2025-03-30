using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

class Program
{
    // Predefined color palette and their corresponding numbers
    //static readonly Color[] Palette = new Color[]
    //{
    //    Color.FromArgb(0, 0, 255),     // #0000ff -> 10
    //    Color.FromArgb(255, 0, 0),     // #ff0000 -> 11
    //    Color.FromArgb(0, 0, 0),       // #000000 -> 12
    //    Color.FromArgb(255, 0, 255),   // #ff00ff -> 13
    //    Color.FromArgb(0, 159, 0),     // #009f00 -> 14
    //    Color.FromArgb(255, 143, 32),  // #ff8f20 -> 15
    //    Color.FromArgb(182, 32, 0),    // #b62000 -> 16
    //    Color.FromArgb(0, 0, 134),     // #000086 -> 17
    //    Color.FromArgb(0,147,255),     // #0093ff -> 18
    //    Color.FromArgb(255, 255, 0),   // #ffff00 -> 19
    //    Color.FromArgb(255, 255, 255), // #ffffff -> 20
    //    Color.FromArgb(231, 226, 231), // #e7e2e7 -> 21
    //    Color.FromArgb(199, 195, 199), // #c7c3c7 -> 22
    //    Color.FromArgb(143, 139, 143), // #8f8b8f -> 23
    //    Color.FromArgb(81,85,81,255)   // #515551 -> 24 
    //};

    //static readonly int[] ColorMapping = new int[]
    //{
    //    10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22,23,24
    //};
    // Define the target color palette with the specified colors


    public static void Main(string[] args)
    {

        string inputImagePath;
        if (args.Length != 0)
            inputImagePath = args[0];
        else
        {
            inputImagePath = "C:\\Users\\Brian\\Pictures\\TI-84 Convert\\PuppySmall165.png";
            //inputImagePath = "C:\\Users\\Brian\\Pictures\\TI-84 Convert\\testColor.png";

        }

        //Rgba32[] globalPalette = new Rgba32[]
        //    {
        //        new Rgba32(0, 0, 255),     // #0000ff -> 10
        //        new Rgba32(255, 0, 0),     // #ff0000 -> 11
        //        new Rgba32(0, 0, 0),       // #000000 -> 12
        //        new Rgba32(255, 0, 255),   // #ff00ff -> 13
        //        new Rgba32(0, 159, 0),     // #009f00 -> 14
        //        new Rgba32(255, 143, 32),  // #ff8f20 -> 15
        //        new Rgba32(182, 32, 0),    // #b62000 -> 16
        //        new Rgba32(0, 0, 134),     // #000086 -> 17
        //        new Rgba32(0,147,255),     // #0093ff -> 18
        //        new Rgba32(255, 255, 0),   // #ffff00 -> 19
        //        new Rgba32(255, 255, 255), // #ffffff -> 20
        //        new Rgba32(231, 226, 231), // #e7e2e7 -> 21
        //        new Rgba32(199, 195, 199), // #c7c3c7 -> 22
        //        new Rgba32(143, 139, 143), // #8f8b8f -> 23
        //        new Rgba32(81,85,81,255)   // #515551 -> 24 
        //    };

        string outputImagePath = "output.png";
        // Load the original image
        Bitmap originalImage = new Bitmap(inputImagePath);

        // Resize the image to 320x240
        Bitmap resizedImage = ResizeImage(originalImage, 265, 165);

        // Save the output image
        resizedImage.Save("resized.png", ImageFormat.Png);

        // Reduce colors with dithering
        // Load your image
        Console.WriteLine("Applying dithering and quantization");

        using (var image = SixLabors.ImageSharp.Image.Load("resized.png"))
        {

            // Convert the palette to the ImageSharp specific Palette type

            SixLabors.ImageSharp.Color[] globalPalette = new SixLabors.ImageSharp.Color[]
            {
            new SixLabors.ImageSharp.Color(new Rgba32(255, 0, 0)      ),
            new SixLabors.ImageSharp.Color(new Rgba32(0, 0, 0)        ),
            new SixLabors.ImageSharp.Color(new Rgba32(255, 0, 255)    ),
            new SixLabors.ImageSharp.Color(new Rgba32(0, 159, 0)      ),
            new SixLabors.ImageSharp.Color(new Rgba32(255, 143, 32)   ),
            new SixLabors.ImageSharp.Color(new Rgba32(182, 32, 0)     ),
            new SixLabors.ImageSharp.Color(new Rgba32(0, 0, 134)      ),
            new SixLabors.ImageSharp.Color(new Rgba32(0, 147, 255)    ),
            new SixLabors.ImageSharp.Color(new Rgba32(255, 255, 0)    ),
            new SixLabors.ImageSharp.Color(new Rgba32(255, 255, 255)  ),
            new SixLabors.ImageSharp.Color(new Rgba32(231, 226, 231)  ),
            new SixLabors.ImageSharp.Color(new Rgba32(199, 195, 199)  ),
            new SixLabors.ImageSharp.Color(new Rgba32(143, 139, 143)  ),
            new SixLabors.ImageSharp.Color(new Rgba32(81, 85, 81)     )
        }
        ;

            // Apply dithering with the specified palette
            image.Mutate(x => x
                .Dither(SixLabors.ImageSharp.Processing.KnownDitherings.FloydSteinberg, 1.0F, globalPalette) // Use Floyd-Steinberg dithering algorithm
            );


            // Save the dithered image
            image.Save("output.png", new PngEncoder());


        }

        Console.WriteLine("Dithering complete!");
        Console.WriteLine("Image saved to " + outputImagePath);

        //***************

        Console.WriteLine("Converting to String");

        List<string> strVars = new List<string>();

        // Load the image
        using (Bitmap bitmap = new Bitmap("output.png"))
        {

            // Create a StreamWriter to write the characters to the file
            using (StreamWriter writer = new StreamWriter("output.txt"))
            {
                long counter = 0;
                string buffer = "";
                // Loop through all pixels in the image
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        // Get the pixel color at (x, y)
                        System.Drawing.Color pixelColor = bitmap.GetPixel(x, y);

                        // Convert the color to a hex string (RGB)
                        string hexColor = $"#{pixelColor.R:X2}{pixelColor.G:X2}{pixelColor.B:X2}".ToLower();

                        // Map the color to the corresponding character
                        char mappedChar = GetMappedCharacter(hexColor);

                        // Write the character to the file
                        writer.Write(mappedChar);
                        counter++;
                        buffer += mappedChar;
                        if (counter % 9999 == 0)
                        {
                            strVars.Add(buffer);
                            buffer = "";
                        }

                    }
                }
                strVars.Add(buffer);
            }
        }

        Console.WriteLine("Converted to string in output.txt");

        Console.WriteLine("Generating program");

        string strImgData = "";
        int strVarId = 0;
        foreach (string s in strVars)
        {
            strImgData += "\r\n\"";
            strImgData += s;
            strImgData += "→Str" + strVarId++;
        }


        using (StreamWriter writer = new StreamWriter("program.txt"))
        {
            writer.Write(
                "\"Generated by SD Picture Viewer by TheLastMillennial" +
                "\r\n\"Copy this code into the TI-Connect CE program editor then send the program to your TI-84 Plus CE" +
                "\r\nGoto D" +
                "\r\nLbl A" +
                "\r\nFnOff " +
                "\r\nPlotsOff " +
                "\r\nClrDraw" +
                "\r\nBackgroundOn WHITE" +
                "\r\n10→B" +
                "\r\n11→C" +
                "\r\n12→D" +
                "\r\n13→F" +
                "\r\n14→G" +
                "\r\n" +
                "\r\n0→I" +
                "\r\n0→J" +

                "\r\nlength(Str0→L" +
                "\r\nFor(T,0,164" +
                "\r\nFor(S,0,264" +

                "\r\nIf I≥L" +
                "\r\nThen" +
                "\r\n0→I" +
                "\r\nJ+1→J" +
                "\r\nIf J=1" +
                "\r\nStr1→Str0" +
                "\r\nIf J=2" +
                "\r\nStr2→Str0" +
                "\r\nIf J=3" +
                "\r\nStr3→Str0" +
                "\r\nIf J=4" +
                "\r\nStr4→Str0" +
                "\r\nIf J=5" +
                "\r\nStop" +
                "\r\nlength(Str0→L" +
                "\r\nEnd" +

                "\r\n1+I→I" +
                "\r\nexpr(sub(Str0,Ans,1" +
                "\r\nIf Ans≠B" +
                "\r\nPxl-On(T,S,Ans+10" +
                "\r\nEnd" +
                "\r\nEnd" +
                "\r\nLbl D" +
                "\r\n" + strImgData +
                "\r\nGoto A");
        }

        Console.WriteLine("Generated program to program.txt");


    }

    // Resize the image to the specified width and height
    private static Bitmap ResizeImage(Bitmap originalImage, int width, int height)
    {
        Bitmap resizedImage = new Bitmap(width, height);
        using (Graphics g = Graphics.FromImage(resizedImage))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(originalImage, 0, 0, width, height);
        }
        return resizedImage;
    }


    // Map the color to the corresponding character
    static char GetMappedCharacter(string hexColor)
    {
        switch (hexColor)
        {
            case "#0000ff": return '0'; // Blue
            case "#ff0000": return '1'; // Red
            case "#000000": return '2'; // Black
            case "#ff00ff": return '3'; // Magenta
            case "#009f00": return '4'; // Green
            case "#ff8f20": return '5'; // Orange
            case "#b62000": return '6'; // Dark Red
            case "#000086": return '7'; // Dark Blue
            case "#0093ff": return '8'; // Light Blue
            case "#ffff00": return '9'; // Yellow
            case "#ffffff": return 'B'; // White
            case "#e7e2e7": return 'C'; // Light Purple
            case "#c7c3c7": return 'D'; // Gray
            case "#8f8b8f": return 'F'; // Dark Gray
            case "#515551": return 'G'; // Olive
            default: return 'B'; // Default to white for unknown colors
        }
    }


}


