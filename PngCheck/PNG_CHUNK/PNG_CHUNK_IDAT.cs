using PngCheck.PNG_CHUNK_ENUM;

namespace PngCheck.PNG_CHUNK
{
    public struct PNG_CHUNK_IDAT : IPNG_CHUNK
    {
        public byte[] bytes;
    }
}