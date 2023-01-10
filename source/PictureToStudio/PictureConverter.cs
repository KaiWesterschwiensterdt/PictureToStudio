using System.Drawing;

public class PictureConverter
{
    public PictureConverter() { }

    public Bitmap ToStudio(Bitmap image)
    {
        // Convert image to grayscale
        Bitmap grayscaleImage = Grayscale(image);

        // Increase contrast of grayscale image
        grayscaleImage = AdjustContrast(grayscaleImage, 1.5);

        // Adjust brightness of grayscale image
        grayscaleImage = AdjustBrightness(grayscaleImage, 20);

        // Blur grayscale image to reduce noise
        grayscaleImage = Blur(grayscaleImage, 3);

        // Detect edges in grayscale image using Canny edge detector
        Bitmap edgeImage = CannyEdgeDetector(grayscaleImage, 50, 150);

        // Fill small gaps in edges using morphological closing
        edgeImage = MorphologicalClosing(edgeImage, 3);

        // Overlay edge image on top of original image
        Bitmap finalImage = OverlayImage(image, edgeImage);

        return finalImage;
    }

    private Bitmap Grayscale(Bitmap image)
    {
        // Create a blank bitmap with the same dimensions as the input image
        Bitmap grayscaleImage = new Bitmap(image.Width, image.Height);

        // Iterate over each pixel in the image
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                // Get the pixel at the current position
                Color pixel = image.GetPixel(x, y);

                // Calculate the grayscale value using the formula:
                // gray = (r * 0.3) + (g * 0.59) + (b * 0.11)
                byte gray = (byte)((pixel.R * 0.3) + (pixel.G * 0.59) + (pixel.B * 0.11));

                // Set the grayscale value as the R, G, and B values of the pixel
                grayscaleImage.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
            }
        }

        return grayscaleImage;
    }

    private Bitmap AdjustContrast(Bitmap image, double contrast)
    {
        Bitmap adjustedImage = new Bitmap(image.Width, image.Height);

        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                Color color = image.GetPixel(x, y);
                int r = Clamp((int)(((color.R / 255.0 - 0.5) * contrast + 0.5) * 255), 0, 255);
                int g = Clamp((int)(((color.G / 255.0 - 0.5) * contrast + 0.5) * 255), 0, 255);
                int b = Clamp((int)(((color.B / 255.0 - 0.5) * contrast + 0.5) * 255), 0, 255);
                adjustedImage.SetPixel(x, y, Color.FromArgb(r, g, b));
            }
        }

        return adjustedImage;
    }

    private int Clamp(int value, int min, int max)
    {
        return Math.Min(Math.Max(value, min), max);
    }

    private Bitmap AdjustBrightness(Bitmap image, int brightness)
    {
        Bitmap adjustedImage = new Bitmap(image.Width, image.Height);

        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                Color color = image.GetPixel(x, y);
                int r = Math.Max(0, Math.Min(255, color.R + brightness));
                int g = Math.Max(0, Math.Min(255, color.G + brightness));
                int b = Math.Max(0, Math.Min(255, color.B + brightness));
                adjustedImage.SetPixel(x, y, Color.FromArgb(r, g, b));
            }
        }

        return adjustedImage;
    }

    private Bitmap Blur(Bitmap image, int kernelSize)
    {
        Bitmap blurredImage = new Bitmap(image.Width, image.Height);

        // Create kernel for Gaussian blur
        float[,] kernel = CreateGaussianKernel(kernelSize);

        // Convolve kernel with image to blur it
        blurredImage = Convolve(image, kernel);

        return blurredImage;
    }

    private float[,] CreateGaussianKernel(int size)
    {
        float[,] kernel = new float[size, size];

        float sigma = size / 3f;
        float sum = 0;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int xDistance = x - size / 2;
                int yDistance = y - size / 2;
                float value = (float)Math.Exp(-(xDistance * xDistance + yDistance * yDistance) / (2 * sigma * sigma));
                kernel[x, y] = value;
                sum += value;
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                kernel[x, y] /= sum;
            }
        }

        return kernel;
    }

    private Bitmap Convolve(Bitmap image, float[,] kernel)
    {
        Bitmap convolvedImage = new Bitmap(image.Width, image.Height);

        int kernelWidth = kernel.GetLength(0);
        int kernelHeight = kernel.GetLength(1);
        int kernelCenterX = kernelWidth / 2;
        int kernelCenterY = kernelHeight / 2;

        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                float sumR = 0;
                float sumG = 0;
                float sumB = 0;
                for (int i = 0; i < kernelWidth; i++)
                {
                    for (int j = 0; j < kernelHeight; j++)
                    {
                        int imageX = x + i - kernelCenterX;
                        int imageY = y + j - kernelCenterY;
                        if (imageX >= 0 && imageX < image.Width && imageY >= 0 && imageY < image.Height)
                        {
                            Color pixelColor = image.GetPixel(imageX, imageY);
                            sumR += pixelColor.R * kernel[i, j];
                            sumG += pixelColor.G * kernel[i, j];
                            sumB += pixelColor.B * kernel[i, j];
                        }
                    }
                }
                int resultR = Math.Min(Math.Max((int)sumR, 0), 255);
                int resultG = Math.Min(Math.Max((int)sumG, 0), 255);
                int resultB = Math.Min(Math.Max((int)sumB, 0), 255);
                convolvedImage.SetPixel(x, y, Color.FromArgb(resultR, resultG, resultB));
            }
        }

        return convolvedImage;
    }

    private Bitmap CannyEdgeDetector(Bitmap image, int lowThreshold, int highThreshold)
    {
        // Detect edges in image using Canny edge detector
        Bitmap result = new Bitmap(image.Width, image.Height);

        // Loop through each pixel in the image
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                // Get the pixel value at this position
                Color pixelColor = image.GetPixel(x, y);
                int pixelValue = pixelColor.R;

                // If the pixel value is within the specified threshold range, set it to black in the result image
                if (pixelValue >= lowThreshold && pixelValue <= highThreshold)
                    result.SetPixel(x, y, Color.Black);
                // Otherwise, set it to white
                else
                    result.SetPixel(x, y, Color.White);
            }
        }

        return image;
    }

    private Bitmap MorphologicalClosing(Bitmap image, int kernelSize)
    {
        // Create a new Bitmap object to store the result
        Bitmap result = new Bitmap(image.Width, image.Height);

        // Loop through each pixel in the image
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                // Get the pixel value at this position
                Color pixelColor = image.GetPixel(x, y);

                // Check if the pixel is black (representing an edge)
                if (pixelColor == Color.Black)
                {
                    // Dilate the edge by setting surrounding pixels to black as well
                    for (int i = -kernelSize; i <= kernelSize; i++)
                    {
                        for (int j = -kernelSize; j <= kernelSize; j++)
                        {
                            // Check if the surrounding pixel is within the image bounds
                            if (x + i >= 0 && x + i < image.Width && y + j >= 0 && y + j < image.Height)
                            {
                                // Set the surrounding pixel to black
                                result.SetPixel(x + i, y + j, Color.Black);
                            }
                        }
                    }
                }
                // If the pixel is not an edge, set it to white in the result image
                else
                {
                    result.SetPixel(x, y, Color.White);
                }
            }
        }
        // Erode the result image by setting any black pixel surrounded by white pixels to white
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                // Check if the pixel is black (representing an edge)
                if (result.GetPixel(x, y) == Color.Black)
                {
                    bool surroundedByWhite = true;
                    // Check if all surrounding pixels are white
                    for (int i = -kernelSize; i <= kernelSize; i++)
                    {
                        for (int j = -kernelSize; j <= kernelSize; j++)
                        {
                            // Check if the surrounding pixel is within the image bounds
                            if (x + i >= 0 && x + i < image.Width && y + j >= 0 && y + j < image.Height)
                            {
                                // If any surrounding pixel is black, set surroundedByWhite to false
                                if (result.GetPixel(x + i, y + j) == Color.Black)
                                {
                                    surroundedByWhite = false;
                                    break;
                                }
                            }
                        }
                        if (!surroundedByWhite)
                            break;
                    }
                    // If the pixel is surrounded by white pixels, set it to white in the result image
                    if (surroundedByWhite)
                        result.SetPixel(x, y, Color.White);
                }
            }
        }
        return image;
    }

    private Bitmap OverlayImage(Bitmap baseImage, Bitmap overlayImage)
    {
        // Create a new Bitmap object to store the result
        Bitmap result = new Bitmap(baseImage.Width, baseImage.Height);

        // Loop through each pixel in the base image
        for (int y = 0; y < baseImage.Height; y++)
        {
            for (int x = 0; x < baseImage.Width; x++)
            {
                // Get the pixel values at this position for both the base and overlay images
                Color basePixelColor = baseImage.GetPixel(x, y);
                Color overlayPixelColor = overlayImage.GetPixel(x, y);

                // If the overlay pixel is black, set the result pixel to black
                if (overlayPixelColor.R == 0 && overlayPixelColor.G == 0 && overlayPixelColor.B == 0)
                    result.SetPixel(x, y, Color.Black);
                // Otherwise, set the result pixel to the base image pixel value
                else
                    result.SetPixel(x, y, basePixelColor);
            }
        }
        // Return the resulting image
        return baseImage;
    }
}
