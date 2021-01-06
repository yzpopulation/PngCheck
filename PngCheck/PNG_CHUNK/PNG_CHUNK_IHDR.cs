using PngCheck.PNG_CHUNK_ENUM;

namespace PngCheck.PNG_CHUNK
{
    public struct PNG_CHUNK_IHDR : IPNG_CHUNK
    {
        public uint width;
        public uint height;
        public byte bits;
        public PNG_COLOR_SPACE_TYPE color_type;
        public PNG_COMPR_METHOD compr_method;
        public PNG_FILTER_METHOD filter_method;
        public PNG_INTERLACE_METHOD interlace_method;
    }
}