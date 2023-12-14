using System;

namespace StableDiffusion.NET;

[AttributeUsage(AttributeTargets.Field)]
internal class NativeName : Attribute
{
    #region Properties & Fields

    public string Name { get; set; }

    #endregion

    #region Constructors

    public NativeName(string name)
    {
        this.Name = name;
    }

    #endregion
}