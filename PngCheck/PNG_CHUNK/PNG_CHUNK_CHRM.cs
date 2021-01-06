using System.Drawing;

namespace PngCheck.PNG_CHUNK
{
    public struct PNG_CHUNK_CHRM : IPNG_CHUNK
    {
        public Point white;
        public Point red;
        public Point green;
        public Point blue;
    }
}