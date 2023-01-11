// See https://aka.ms/new-console-template for more information
using PictureToStudio;
using System.Drawing;
using System.Drawing.Imaging;

var converter = new PictureConverter();
var resizer = new PictureResizer();

Console.WriteLine("Hello and welcome to our personal image enhancer!\r");
Console.WriteLine("First enter the path to the image:");
string inputPath = Console.ReadLine();
Console.WriteLine();
Console.WriteLine("Thank you. Now enter the path to the output directory:");
string outputPath = Console.ReadLine();
Console.WriteLine();
Console.WriteLine("Last step. What should we call your new image?");
string newFileName = Console.ReadLine();
Console.WriteLine();
Console.WriteLine("Sit back and wait for your new picture in the studio!");


Image originalImage = Image.FromFile(inputPath);

float scaleFactor = 1.5f;
var resizedImage = resizer.BicubicInterpolation((Bitmap)originalImage, scaleFactor);

Image enhancedImage = converter.ToStudio(resizedImage);
enhancedImage.Save($"{outputPath}\\{newFileName}.png", ImageFormat.Png);

Console.WriteLine($"Your {newFileName} is no available at {outputPath}!");
Console.ReadLine();