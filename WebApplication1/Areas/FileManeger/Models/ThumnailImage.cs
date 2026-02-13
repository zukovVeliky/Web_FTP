using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;



namespace FileManager.Models
{
    public class ThumnailImage
    {

        public byte[] ImageNahled;
        public int maxPixels { get; set; } = 200;


        public ThumnailImage(string imgNahled)
        {
            //this.INIT(imgNahled);
        }
        
        public ThumnailImage(string imgNahled, int MaxPixels)
        {
            //this.maxPixels = MaxPixels;
           // this.INIT(imgNahled);
        }
        /*
        private void INIT (string imgNahled)
        {
            // Load image.

            Image image = Image.FromFile(imgNahled);

            // Compute thumbnail size.
            Size thumbnailSize = GetThumbnailSize(image);


            Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);

            // Get thumbnail.
            Image thumbnail = image.GetThumbnailImage(thumbnailSize.Width, thumbnailSize.Height, myCallback, IntPtr.Zero);

            // Save thumbnail.
            //thumbnail.Save(output);

            using (var ms = new MemoryStream())
            {
                thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ImageNahled = ms.ToArray();
            }
        }
        public Size GetThumbnailSize(Image original)
        {
            // Maximum size of any dimension.


            // Width and height.
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)this.maxPixels / originalWidth;
            }
            else
            {
                factor = (double)this.maxPixels / originalHeight;
            }

            // Return thumbnail size.
            return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }
        public bool ThumbnailCallback()
        {
            return true;
        }*/
    }


}



    



