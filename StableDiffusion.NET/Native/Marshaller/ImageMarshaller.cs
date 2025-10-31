using HPPH;
using System.Runtime.InteropServices.Marshalling;

namespace StableDiffusion.NET;

[CustomMarshaller(typeof(IImage), MarshalMode.ManagedToUnmanagedIn, typeof(ImageMarshallerIn))]
[CustomMarshaller(typeof(Image<ColorRGB>), MarshalMode.ManagedToUnmanagedOut, typeof(ImageMarshaller))]
internal static class ImageMarshaller
{
    public static Image<ColorRGB> ConvertToManaged(Native.Types.sd_image_t unmanaged) => unmanaged.ToImage();

    public static void Free(Native.Types.sd_image_t unmanaged) => unmanaged.Free();

    internal ref struct ImageMarshallerIn
    {
        private Native.Types.sd_image_t _image;

        public void FromManaged(IImage managed) => _image = managed.ToSdImage();

        public Native.Types.sd_image_t ToUnmanaged() => _image;

        public void Free() => _image.Free();
    }
}