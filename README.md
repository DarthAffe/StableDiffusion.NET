# StableDiffusion.NET

Based on https://github.com/leejet/stable-diffusion.cpp

> At least for me the current version of stable-diffusion.cpp has really bad tiling issues.   
If you experience them too, I'd recommend using https://github.com/DarthAffe/StableDiffusion.NET/releases/tag/c6071fa until that's fixed.

## Usage
### Setup
Run `build.bat` to build the native libs (modify params like CUDA-builds if needed)

### Example
```csharp
using StableDiffusionModel sd = new(@"<path_to_model>", new ModelParameter());
using StableDiffusionImage image = sd.TextToImage("<prompt>", new StableDiffusionParameter());
```
