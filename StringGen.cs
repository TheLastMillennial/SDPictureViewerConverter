using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDPictureViewerConverter
{
    class TI84PlusStringFileGenerator
    {
        // Method to generate .8xs file from string
        public static void Generate8xsFile(string inputText, string filePath)
        {
            // Convert the string to a byte array using ASCII encoding
            byte[] inputBytes = Encoding.ASCII.GetBytes(inputText);

            // File header, this is an example, and the actual header might need to be adjusted
            byte[] header = new byte[] { 0x57, 0x01, 0x00, 0x00, 0x00 }; // Arbitrary header for example

            // Combine header and the string data
            byte[] fileData = new byte[header.Length + inputBytes.Length];
            Array.Copy(header, 0, fileData, 0, header.Length);
            Array.Copy(inputBytes, 0, fileData, header.Length, inputBytes.Length);

            // Write the file data to the .8xs file
            try
            {
                File.WriteAllBytes(filePath, fileData);
                Console.WriteLine($"File generated successfully at: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while generating the file: {ex.Message}");
            }
        }

        //// Main entry point
        //static void Main(string[] args)
        //{
        //    // Get the text input and the file path from the user
        //    Console.Write("Enter the text to be stored in the .8xs file: ");
        //    string inputText = Console.ReadLine();

        //    Console.Write("Enter the output file path (e.g., output.8xs): ");
        //    string filePath = Console.ReadLine();

        //    // Generate the .8xs file
        //    Generate8xsFile(inputText, filePath);
        //}
    }

}
