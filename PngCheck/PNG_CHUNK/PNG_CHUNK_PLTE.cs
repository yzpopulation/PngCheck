using PngCheck.PNG_CHUNK_ENUM;

namespace PngCheck.PNG_CHUNK
{
    public struct PNG_CHUNK_PLTE : IPNG_CHUNK
    {
        public PNG_PALETTE_PIXEL[] plteChunkData;
    }

    public struct PNG_PALETTE_PIXEL : IPNG_CHUNK
    {
        public byte btRed;
        public byte btGreen;
        public byte btBlue;
    }
}