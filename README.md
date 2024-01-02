# StableDiffusion.NET

Based on https://github.com/leejet/stable-diffusion.cpp

## Usage
### Setup
Run `build.bat` to build the native libs (modify params like CUDA-builds if needed)

### Example
```csharp
using StableDiffusionModel sd = new(@"<path_to_model>", new ModelParameter());
using StableDiffusionImage image = sd.TextToImage("<prompt>", new StableDiffusionParameter());
```
