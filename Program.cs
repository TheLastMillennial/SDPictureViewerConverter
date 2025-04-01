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
        int strVarId = 1;
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
                "\r\nGoto D\r\nLbl A\r\nFnOff \r\nPlotsOff \r\nClrDraw\r\n\r\n0→I\r\n0→J\r\n0→M\r\n­1→L" +
                "\r\nFor(T,0,164" +
                "\r\nFor(S,0,264" +
                "\r\n" +
                "\r\nIf I≥L\r\nThen" +
                "\r\nIf J=0" +
                "\r\nStr1→Str6" +
                "\r\nIf J=1" +
                "\r\nStr2→Str6" +
                "\r\nIf J=2" +
                "\r\nStr3→Str6" +
                "\r\nIf J=3" +
                "\r\nStr4→Str6" +
                "\r\nIf J=4" +
                "\r\nStr5→Str6" +
                "\r\nIf J≥5" +
                "\r\nStop" +
                "\r\n" +
                "\r\n0→I\r\nJ+.05→J\r\nM+.05→M" +
                "\r\nIf Ans=1.05\r\n.05→M" +
                "\r\n" +
                "\r\nint((Ans-.05)*length(Str6))" +
                "\r\nsub(Str6,Ans+1,int(M*length(Str6))-(Ans)→Str0" +
                "\r\nlength(Str0→L" +
                "\r\nEnd" +
                "\r\n" +
                "\r\nI+1→I" +
                "\r\nPxl-On(T,S,25-inString(\"0123456789BCDFG\",sub(Str0,Ans,1))" +
                "\r\n" +
                "\r\nEnd" +
                "\r\nEnd" +
                "\r\nDelVar Str0\r\nDelVar Str1\r\nDelVar Str2\r\nDelVar Str3\r\nDelVar Str4\r\nDelVar Str5\r\nDelVar Str6" +
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
            case "#0000ff": return 'G'; // Blue
            case "#ff0000": return 'F'; // Red
            case "#000000": return 'D'; // Black
            case "#ff00ff": return 'C'; // Magenta
            case "#009f00": return 'B'; // Green
            case "#ff8f20": return '9'; // Orange
            case "#b62000": return '8'; // Brown
            case "#000086": return '7'; // Navy
            case "#0093ff": return '6'; // Light Blue
            case "#ffff00": return '5'; // Yellow
            case "#ffffff": return '4'; // White
            case "#e7e2e7": return '3'; // Light Grey
            case "#c7c3c7": return '2'; // Medium Gray
            case "#8f8b8f": return '1'; // Gray
            case "#515551": return '0'; // Dark Grey
            default: return '4'; // Default to white for unknown colors
        }
    }
}


