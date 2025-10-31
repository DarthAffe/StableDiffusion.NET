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
using (DiffusionModel sd = new(DiffusionModelParameter.Create()
                                                       .WithModelPath(@"<path to model>")
											        // .WithVae(@"<optional path to vae>")
                                                       .WithMultithreading()
                                                       .WithFlashAttention()))
{
    // Create a image from a prompt
    Image<ColorRGB>? tree = sd.GenerateImage(ImageGenerationParameter.TextToImage("A beautiful tree standing on a small hill").WithSDXLDefaults());
    // (optional) Save the image (requires the HPPH System.Dawing or SkiaSharp extension)
    File.WriteAllBytes("image1.png", tree.ToPng());

    // Use the previously created image for an image-to-image creation
    treeWithTiger = sd.GenerateImage(ImageGenerationParameter.ImageToImage("A cute tiger in front of a tree on a small hill", tree).WithSDXLDefaults());
    File.WriteAllBytes("image2.png", treeWithTiger.ToPng());
}

// Load the qwen image edit model
using DiffusionModel qwenContext = new(DiffusionModelParameter.Create()
                                                              .WithDiffusionModelPath(@"<Qwen-Image-Edit-2509-path>")
                                                              .WithQwen2VLPath(@"<Qwen2.5-VL-7B-Instruct-path>")
                                                              .WithQwen2VLVisionPath(@"<Qwen2.5-VL-7B-Instruct.mmproj-path>")
                                                              .WithVae(@"<qwen_image_vae-path>")
                                                              .WithMultithreading()
                                                              .WithFlashAttention()
                                                              .WithFlowShift(3)
                                                              .WithOffloadedParamsToCPU()
                                                              .WithImmediatelyFreedParams());

// Perform an edit on the previously created image
Image<ColorRGB>? tigerOnMoon = qwenContext.GenerateImage(ImageGenerationParameter.TextToImage("Remove the background and place the tree and the tiger on the moon.")
                                                                                 .WithSize(1024, 1024)
                                                                                 .WithCfg(2.5f)
                                                                                 .WithSampler(Sampler.Euler)
                                                                                 .WithRefImages(treeWithTiger));
File.WriteAllBytes("image3.png", tigerOnMoon.ToPng());
```

To process the resulting image further you can write your own extensions or install one of the [HPPH](https://github.com/DarthAffe/HPPH)-extension sets:   
[HPPH.System.Drawing](https://www.nuget.org/packages/HPPH.System.Drawing)   
[HPPH.SkiaSharp](https://www.nuget.org/packages/HPPH.SkiaSharp)
