using System;

[Serializable]
public abstract class SimpleSaveModel : ICloneable
{
    public object Clone()
    {
        return MemberwiseClone();
    }

    public abstract void Set(object obj);

    public abstract T Get<T>();
}
