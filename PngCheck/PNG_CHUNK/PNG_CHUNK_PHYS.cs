using PngCheck.PNG_CHUNK_ENUM;

namespace PngCheck.PNG_CHUNK
{
    public struct PNG_CHUNK_PHYS : IPNG_CHUNK
    {
        public uint physPixelPerUnitX;
        public uint physPixelPerUnitY;
        public PNG_PHYS_UNITSPEC physUnitSpec;
    }
}