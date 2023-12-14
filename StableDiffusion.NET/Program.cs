using System.Drawing;
using System.Drawing.Imaging;
using StableDiffusion.NET;
using Image = StableDiffusion.NET.Image;

using StableDiffusionModel sd = new(@"N:\StableDiffusion\stable-diffusion-webui\models\Stable-diffusion\ghostmix_v20Bakedvae.safetensors",
                                    new ModelParameter
                                    {
                                        Quantization = Quantization.Q5_1,
                                        Schedule = Schedule.Karras
                                    });

using StableDiffusionParameter parameter = new StableDiffusionParameter { SampleMethod = Sampler.DPMPP2M };
parameter.Width = 768;

using Image image = sd.TextToImage("der ohne gesicht wo im aquarium schwimmt", parameter);
using Bitmap bitmap = ToBitmap2(image);
bitmap.Save("test.jpg");


//unsafe
//{
//    Console.WriteLine(Native.stable_diffusion_get_system_info());

//    Native.stable_diffusion_set_log_level("DEBUG");

//    Native.stable_diffusion_ctx* ctx = Native.stable_diffusion_init(16, false, string.Empty, false, string.Empty, "STD_DEFAULT_RNG");

//    Native.stable_diffusion_load_from_file(ctx, @"N:\StableDiffusion\stable-diffusion-webui\models\Stable-diffusion\ghostmix_v20Bakedvae.safetensors", string.Empty, "Q5_1", "KARRAS");

//    Native.stable_diffusion_full_params* @params = Native.stable_diffusion_full_default_params_ref();

//    Native.stable_diffusion_full_params_set_cfg_scale(@params, 7.5f);
//    Native.stable_diffusion_full_params_set_width(@params, 512);
//    Native.stable_diffusion_full_params_set_height(@params, 512);
//    Native.stable_diffusion_full_params_set_batch_count(@params, 1);
//    Native.stable_diffusion_full_params_set_sample_steps(@params, 30);
//    Native.stable_diffusion_full_params_set_sample_method(@params, "DPMPP2M");

//    byte* result = Native.stable_diffusion_predict_image(ctx, @params, "a wizard in a purple t-shirt casting a spell that causes a mountain to explode");

//    Span<byte> image = new(result, 512 * 512 * 3);

//    using Bitmap bitmap = ToBitmap(image, 512, 512);
//    bitmap.Save("test.jpg");

//    Native.stable_diffusion_free_buffer(result);
//    Native.stable_diffusion_free_full_params(@params);
//    Native.stable_diffusion_free(ctx);
//}


static Bitmap ToBitmap2(Image image) => ToBitmap(image.Data, image.Width, image.Height);

static unsafe Bitmap ToBitmap(ReadOnlySpan<byte> image, int width, int height)
{
    Bitmap output = new(width, height, PixelFormat.Format24bppRgb);
    Rectangle rect = new(0, 0, width, height);
    BitmapData bmpData = output.LockBits(rect, ImageLockMode.ReadWrite, output.PixelFormat);

    nint ptr = bmpData.Scan0;
    image.CopyTo(new Span<byte>((void*)ptr, width * height * 3));

    output.UnlockBits(bmpData);
    return output;
}