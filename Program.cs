/* SD Picture Viewer Converter
 * By: TheLastMillennial
 * Github: https://github.com/TheLastMillennial/SDPictureViewerConverter
 * */

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
    public static void Main(string[] args)
    {
        Console.WriteLine("Standard Definition Picture Viewer CE\n\n");
        string inputImagePath;
        if (args.Length != 0)
            inputImagePath = args[0];
        else
        {
            Console.WriteLine("Drag and drop a PNG picture onto this exe file.\nOpen the ReadMe.txt file for more instructions.");
            return;
        }

        // Load the original image
        Bitmap originalImage = new Bitmap(inputImagePath);

        // Resize the image to 265x165 since that's the resolution of the TI-84 Plus CE graph screen
        Bitmap resizedImage = ResizeImage(originalImage, 265, 165);

        // Save the resized image
        resizedImage.Save("temp_resized.png", ImageFormat.Png);

        // Reduce colors and apply dithering
        Console.WriteLine("Applying dithering and quantization...");

        using (var image = SixLabors.ImageSharp.Image.Load("temp_resized.png"))
        {
            // Create a ImageSharp palette using only TI-84 Plus CE colors
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
            };

            // Apply dithering with the specified palette
            image.Mutate(x => x
                .Dither(SixLabors.ImageSharp.Processing.KnownDitherings.FloydSteinberg, 1.0F, globalPalette) // Use Floyd-Steinberg dithering algorithm
            );

            // Save the dithered image
            image.Save("temp_output.png", new PngEncoder());
        }
        //***************

        Console.WriteLine("Converting picture to TI Basic string...");

        List<string> strVars = new List<string>();

        // Load the image
        using (Bitmap bitmap = new Bitmap("temp_output.png"))
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

        Console.WriteLine("Generating TI-Basic program...");

        // TI Basic commands can only handle up to 9999 characters. 
        // This section saves each chunk of 9999 characters to its own Str variable
        string strImgData = "";
        int strVarId = 0;
        foreach (string s in strVars)
        {
            strImgData += "\r\n\"";
            strImgData += s;
            strImgData += "→Str" + strVarId++;
        }

        // Generate the TI-Basic code
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
                "\r\nPxl-On(T,S,expr(sub(Str0,Ans,1))+10" +
                "\r\nEnd" +
                "\r\nEnd" +
                "\r\nStop" +
                "\r\nLbl D" +
                "\r\n" + strImgData +
                "\r\nGoto A");
        }

        Console.WriteLine("Code saved to program.txt");
        Console.WriteLine("Copy the code to the TI-Connect CE program editor. Then send the program to your calculator.");
        Console.WriteLine("I ran out of time to make this project generate a .8xp file.");
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


