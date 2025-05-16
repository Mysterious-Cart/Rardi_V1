public interface IEntityBuilder<T, Model>
{
    public T Build();
    public static abstract Model ToModel(T Entity);
    public static abstract IEntityBuilder<T,Model> FromModel(Model Model);
}