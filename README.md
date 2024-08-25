# StableDiffusion.NET
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/DarthAffe/StableDiffusion.NET?style=for-the-badge)](https://github.com/DarthAffe/StableDiffusion.NET/releases)
[![Nuget](https://img.shields.io/nuget/v/StableDiffusion.NET?style=for-the-badge)](https://www.nuget.org/packages/StableDiffusion.NET)
[![GitHub](https://img.shields.io/github/license/DarthAffe/StableDiffusion.NET?style=for-the-badge)](https://github.com/DarthAffe/StableDiffusion.NET/blob/master/LICENSE)
[![GitHub Repo stars](https://img.shields.io/github/stars/DarthAffe/StableDiffusion.NET?style=for-the-badge)](https://github.com/DarthAffe/StableDiffusion.NET/stargazers)

Based on https://github.com/leejet/stable-diffusion.cpp

## Usage
### Setup
Install the [StableDiffusion.NET](https://www.nuget.org/packages/StableDiffusion.NET)-Nuget and at least one of the [Backend-Packages](https://www.nuget.org/packages?q=StableDiffusion.NET.Backend).   
If GPU-support is available it will prefer this over CPU.   
If you want to add your own native-libraries or need more control over which backend to load, check the static `Backends` class.   

### Example
#### 1. Create a model

stable diffusion:
```csharp
using DiffusionModel model = ModelBuilder.StableDiffusion(@"<path to model")
                                         .WithVae(@"<optional path to vae>")
                                         .WithMultithreading()
                                         .Build();
```

flux:
```csharp
using DiffusionModel model = ModelBuilder.Flux(@"<path to flux-model.gguf>",
                                               @"<path to clip_l.safetensors>",
                                               @"<path to t5xxl_fp16.safetensors>",
                                               @"<path to ae.safetensors>")
                                         .WithMultithreading()
                                         .Build();
```

#### 2. create image

with default parameters:
```csharp
IImage<ColorRGB> image = model.TextToImage("<prompt>");
```

with custom parameters:
```csharp
IImage<ColorRGB> image = model.TextToImage("<prompt>", model.GetDefaultParameter().WithSeed(1234).WithSize(1344, 768));
```

#### 3. (optional) save the image (requires System.Dawing or SkiaSharp extension)
```csharp
File.WriteAllBytes("output.png", image.ToPng());
```

To process the resulting image further you can write your own extensions or install one of the [HPPH](https://github.com/DarthAffe/HPPH)-extension sets:   
[HPPH.System.Drawing](https://www.nuget.org/packages/HPPH.System.Drawing)   
[HPPH.SkiaSharp](https://www.nuget.org/packages/HPPH.SkiaSharp)
