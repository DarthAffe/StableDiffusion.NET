# StableDiffusion.NET
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/DarthAffe/StableDiffusion.NET?style=for-the-badge)](https://github.com/DarthAffe/StableDiffusion.NET/releases)
[![Nuget](https://img.shields.io/nuget/v/StableDiffusion.NET?style=for-the-badge)](https://www.nuget.org/packages/StableDiffusion.NET)
[![GitHub](https://img.shields.io/github/license/DarthAffe/StableDiffusion.NET?style=for-the-badge)](https://github.com/DarthAffe/StableDiffusion.NET/blob/master/LICENSE)
[![GitHub Repo stars](https://img.shields.io/github/stars/DarthAffe/StableDiffusion.NET?style=for-the-badge)](https://github.com/DarthAffe/StableDiffusion.NET/stargazers)

Based on https://github.com/leejet/stable-diffusion.cpp

## Usage
### Setup
Install the [StableDiffusion.NET](https://www.nuget.org/packages/StableDiffusion.NET)-Nuget and at least one of the [Backend-Packages](https://www.nuget.org/packages?q=StableDiffusion.NET.Backend).   
StableDiffusion.NET is using semantic versioning. Backend-packages are compatible as long as the version does only differ in the last digit.   
If GPU-support is available it will prefer this over CPU.   
If you want to add your own native-libraries or need more control over which backend to load, check the static `Backends` class.   

### Example
#### 1. Create a model

stable diffusion:
```csharp
// Enable the Log- and Progress-events
StableDiffusionCpp.InitializeEvents();

// Register the Log and Progress-events to capture stable-diffusion.cpp output
StableDiffusionCpp.Log += (_, args) => Console.WriteLine($"LOG [{args.Level}]: {args.Text}");
StableDiffusionCpp.Progress += (_, args) => Console.WriteLine($"PROGRESS {args.Step} / {args.Steps} ({(args.Progress * 100):N2} %) {args.IterationsPerSecond:N2} it/s ({args.Time})");

Image<ColorRGB>? treeWithTiger;
// Load a StableDiffusion model in a using block to unload it again after the two images are created
using (DiffusionModel sd = ModelBuilder.StableDiffusion(@"<path to model") 
                                    // .WithVae(@"<optional path to vae>")
                                       .WithMultithreading()
                                       .WithFlashAttention()
                                       .Build())
{
    // Create a image from a prompt
    Image<ColorRGB>? tree = sd.GenerateImage(ImageGenerationParameter.TextToImage("A beautiful tree standing on a small hill").WithSDXLDefaults());
    // (optional) Save the image (requires the HPPH System.Dawing or SkiaSharp extension)
    File.WriteAllBytes("image1.png", tree.ToPng());

    // Use the previously created image for an image-to-image creation
    treeWithTiger = sd.GenerateImage(ImageGenerationParameter.ImageToImage("A cute tiger in front of a tree on a small hill", tree).WithSDXLDefaults());
    File.WriteAllBytes("image2.png", treeWithTiger.ToPng());
}

// Load a flux kontext model
using DiffusionModel flux = ModelBuilder.Flux(@"<path to flux-model.gguf>",
                                              @"<path to clip_l.safetensors>",
                                              @"<path to t5xxl_fp16.safetensors>",
                                              @"<path to ae.safetensors>")
                                        .WithMultithreading()
                                        .WithFlashAttention()
                                        .Build();

// Perform an edit on the previosly created image
Image<ColorRGB>? tigerOnMoon = flux.GenerateImage(ImageGenerationParameter.TextToImage("Remove the hill with the grass and place the tree with the tiger on the moon").WithFluxDefaults().WithRefImages(treeWithTiger));
File.WriteAllBytes("image3.png", tigerOnMoon.ToPng());
```

To process the resulting image further you can write your own extensions or install one of the [HPPH](https://github.com/DarthAffe/HPPH)-extension sets:   
[HPPH.System.Drawing](https://www.nuget.org/packages/HPPH.System.Drawing)   
[HPPH.SkiaSharp](https://www.nuget.org/packages/HPPH.SkiaSharp)
