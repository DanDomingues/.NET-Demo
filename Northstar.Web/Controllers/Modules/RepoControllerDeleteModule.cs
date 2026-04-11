using Northstar.DataAccess.IRepository;
using Northstar.Models;

namespace Northstar.Web.Controllers.Modules
{
    public class RepoControllerDeleteModule<TModel, TRepo> : RepositoryControllerModule<TModel, TRepo>
        where TModel : class, IModelBase, new()
        where TRepo : IRepository<TModel>
    {
        public RepoControllerDeleteModule(IRepoControllerModuleContainer<TModel, TRepo> c) : base()
        {
            GetWithId = id =>
            {
                if (!c.Find(id, out TModel model))
                {
                    return c.AsController.NotFound();
                }

                return c.AsController.View(model);                
            };
            
            PostWithId = id =>
            {
                if (!c.Find(id, out var model))
                {
                    return c.AsController.NotFound();
                }

                return c.UpdateRepo(
                    model, 
                    c.Repo.Remove, 
                    feedback: "deleted");
            };
        }
    }
}