using Demo.DataAccess.IRepository;
using Demo.Models;

namespace ASP.NET_Debut.Controllers.RepoControllerModules
{
    public class RepoControllerUpsertModule<TModel, TRepo> : RepositoryControllerModule<TModel, TRepo>
        where TModel : class, IModelBase, new()
        where TRepo : IRepository<TModel>
    {
        public RepoControllerUpsertModule(IRepoControllerModuleContainer<TModel, TRepo> c) : base()
        {
            GetWithId = id =>
            {
                if(id == null || id == 0)
                {
                    return c.AsController.View(new TModel());
                }

                return c.AsController.View(c.Repo.GetById(id));
            };
            Post = model =>
            {
                if(c.AsController.ModelState.IsValid && c.ValidateForUpsert(model))
                {
                    c.UpdateRepo(
                        model, 
                        c.Repo.AddOrUpdate, 
                        feedback: model.Id == 0 ? "created" : "updated");
                }
                return c.AsController.RedirectToAction("Index");
            };
        }
    }
}