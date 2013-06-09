namespace EPiGlue.Framework
{
    public interface IModelPropertyHandler
    {
        bool CanHandle(ModelPropertyContext context);

        void Process(ModelPropertyContext context);
    }
}