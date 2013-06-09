using System.Web.Mvc;

namespace EPiGlue.Framework
{
    public interface IModelProcessor
    {
        void Process(ResultExecutingContext context);
    }
}