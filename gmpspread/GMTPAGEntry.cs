using gmpspread.Assets;
using gmpspread.Base_Classes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace gmpspread
{
    public class GMTPAGEntry
    {
        public short X;
        public short Y;
        public short Width;
        public short Height;
        public short XOffset;
        public short YOffset;
        public short CropWidth;
        public short CropHeight;
        public short OWidth;
        public short OHeight;

        // pointer.
        public GMTextureBlob Texture;
        public Image TextureItem;
        public short TexID;

        public GMTPAGEntry(BinaryReader binaryReader, GMWAD w)
        {
            X = binaryReader.ReadInt16();
            Y = binaryReader.ReadInt16();
            Width = binaryReader.ReadInt16();
            Height = binaryReader.ReadInt16();
            XOffset = binaryReader.ReadInt16();
            YOffset = binaryReader.ReadInt16();
            CropWidth = binaryReader.ReadInt16();
            CropHeight = binaryReader.ReadInt16();
            OWidth = binaryReader.ReadInt16();
            OHeight = binaryReader.ReadInt16();
            TexID = binaryReader.ReadInt16();

            //Does when calling Crop()
            //Texture = w.TextureBlobs.Items[TexID];
        }

        /// <summary>
        /// This will update the Texture reference and set TextureItem.
        /// </summary>
        public void Crop()
        {
            int exportWidth = OWidth;
            int exportHeight = OHeight;

            Bitmap resultImage = null;

            resultImage = Texture.TexturePage.Clone(new Rectangle(X, Y, Width, Height), Texture.TexturePage.PixelFormat);

            if ((Width != CropWidth) || (Height != CropHeight))
            {
                Output.Print("Resizing texture item!!!");
                resultImage = ResizeImage(resultImage, CropWidth, CropHeight);
            }

            Bitmap returnImage = resultImage;
            returnImage = new Bitmap(exportWidth, exportHeight);
            Graphics g = Graphics.FromImage(returnImage);
            g.DrawImage(resultImage, new Rectangle(XOffset, YOffset, resultImage.Width, resultImage.Height), new Rectangle(0, 0, resultImage.Width, resultImage.Height), GraphicsUnit.Pixel);
            g.Dispose();

            TextureItem = returnImage;
        }

        // This should perform a high quality resize.
        // Grabbed from https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
