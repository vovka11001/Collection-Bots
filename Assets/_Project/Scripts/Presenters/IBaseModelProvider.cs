public interface IBaseModelProvider
{
   bool TryGetModel(BaseView view, out BaseModel model);
}