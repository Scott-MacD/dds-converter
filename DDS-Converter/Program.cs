using System.Runtime.InteropServices;
using DirectXTexNet;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

class DDS_Converter {

    static void Main(string[] args) {

        if (args.Length != 2) {
            Console.WriteLine("Usage: DDS-Converter <input.dds> <output.png>");
            return;
        }

        string input = args[0];
        string output = args[1];

        ConvertDDSToPNG(input, output);
    }

    static void ConvertDDSToPNG(string input, string output) {

        try {
            using (var image = TexHelper.Instance.LoadFromDDSFile(input, DDS_FLAGS.NONE)) {
            
                var metaData = image.GetMetadata();

                // Decompress BC3_UNORM to R8G8B8A8_UNORM to ensure compatibility with ImageSharp
                ScratchImage decompressedImage = null;
                if (metaData.Format == DXGI_FORMAT.BC3_UNORM || metaData.Format == DXGI_FORMAT.BC1_UNORM) {
                    try {
                        decompressedImage = image.Decompress(0, DXGI_FORMAT.R8G8B8A8_UNORM);
                    } catch (COMException comEx) {
                        Console.WriteLine($"Decompression failed: {comEx.Message} (HRESULT: {comEx.HResult})");
                    }
                } else if (metaData.Format != DXGI_FORMAT.R8G8B8A8_UNORM) {
                    try {
                        decompressedImage = image.Convert(0, DXGI_FORMAT.R8G8B8A8_UNORM, TEX_FILTER_FLAGS.DEFAULT, 0.5f);
                    } catch (COMException comEx) {
                        Console.WriteLine($"Conversion to R8G8B8A8_UNORM failed (format: {metaData.Format}): {comEx.Message} (HRESULT: {comEx.HResult})");
                    }
                } else {
                    decompressedImage = image;
                }

                if (decompressedImage == null) {
                    throw new Exception("Decompression or conversion to a compatible format failed.");
                }

                var imageData = decompressedImage.GetImage(0);
                int width = metaData.Width;
                int height = metaData.Height;
                int rowPitch = (int)imageData.RowPitch;
                int slicePitch = (int)imageData.SlicePitch;

                // Calculate the expected size
                int expectedSize = width * height * 4;

                if (slicePitch < expectedSize) {
                    throw new Exception($"Pixel data length {slicePitch} is less than expected size {expectedSize}");
                }

                // Allocate managed array for pixel data
                byte[] pixelData = new byte[slicePitch];

                // Copy pixel data to managed array
                IntPtr pixelPointer = imageData.Pixels;
                Marshal.Copy(pixelPointer, pixelData, 0, slicePitch);

                // Create ImageSharp image from pixel data
                using var imageSharpImage = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(pixelData, width, height);
                imageSharpImage.Save(output);
            }

            Console.WriteLine($"Conversion successful: {input} -> {output}");

        } catch (COMException comEx) {
            Console.WriteLine($"COM Exception during conversion: {comEx.Message} (HRESULT: {comEx.HResult})");
        } catch (Exception ex) {
            Console.WriteLine($"Error during conversion: {ex.Message}");
        }
    }
}
