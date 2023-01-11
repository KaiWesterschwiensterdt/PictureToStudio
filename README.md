# PictureToStudio

The PictureConverter class provides a set of methods for converting pictures to a "studio" style, by applying a series of image processing techniques.

## Methods

- `ToStudio(Bitmap image)`: Converts the input image to a "studio" style. It perform the following steps:
  1. Convert the image to grayscale
  2. Increase the contrast of the grayscale image
  3. Adjust the brightness of the grayscale image
  4. Blur the grayscale image to reduce noise
  5. Detect edges in the grayscale image using Canny edge detector
  6. Fill small gaps in edges using morphological closing
  7. Overlay the edge image on top of the original image

## Usage

Here's an example of how to use the `PictureConverter` class:

```c#
Bitmap originalImage = new Bitmap("path/to/image.jpg");
PictureConverter converter = new PictureConverter();
Bitmap studioImage = converter.ToStudio(originalImage);
```

You can then save the `studioImage` to a file or display it in an image viewer.

Here's an example of how to use the `PictureResizer` class:

```c#
Image originalImage = Image.FromFile("path/to/image.jpg");
Bitmap bicubicInterpolatedImage = BicubicInterpolation((Bitmap)originalImage, 1.5f);
```

## Note

You'll need to add the necessary references and dependencies to the project to be able to use the functions.
