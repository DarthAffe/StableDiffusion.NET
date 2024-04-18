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
```csharp
using StableDiffusionModel sd = new(@"<path_to_model>", new ModelParameter());
using StableDiffusionImage image = sd.TextToImage("<prompt>", new StableDiffusionParameter());
```
