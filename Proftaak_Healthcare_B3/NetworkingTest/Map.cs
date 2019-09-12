using Networking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTest
{
    public class Map
    {
        private Bitmap bitmap;
        private byte[] heights;

        private int width;
        private int depth;
        private int maxHeight;

        public Map(int width, int depth, int maxHeight)
        {
            this.width = width;
            this.depth = depth;
            this.maxHeight = maxHeight;

            this.heights = new byte[width * depth];
        }

        public Map(int width, int depth, int maxHeight, string imageFilePath)
            : this(width, depth, maxHeight)
        {
            ConvertImageToMap(imageFilePath);
        }

        public void ConvertImageToMap(string imageFilePath)
        {
            if(File.Exists(imageFilePath))
            {
                this.bitmap = new Bitmap(imageFilePath);

                for(int y = 0; y < this.depth; y++)
                {
                    for (int x = 0; x < this.width; x++)
                    {
                        int xPos = x * (this.bitmap.Width / this.width);
                        int yPos = y * (this.bitmap.Height / this.depth);
                        this.heights[(y * this.width) + x] = (byte)((this.maxHeight / 255) * this.bitmap.GetPixel(xPos, yPos).B);
                    }
                }
            }
        }

        public Bitmap GetBitmap()
        {
            return this.bitmap;
        }

        public byte[] GetHeights()
        {
            return this.heights;
        }

        public int GetWidth()
        {
            return this.width;
        }

        public int GetDepth()
        {
            return this.depth;
        }
    }
}
