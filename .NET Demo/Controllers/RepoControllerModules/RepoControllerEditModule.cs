using Demo.DataAccess.IRepository;
using Demo.Models;

namespace ASP.NET_Debut.Controllers.RepoControllerModules
{
    public class RepoControllerEditModule<TModel, TRepo> : RepositoryControllerModule<TModel, TRepo>
        where TModel : class, IModelBase, new()
        where TRepo : IRepository<TModel>
    {
        public RepoControllerEditModule(IRepoControllerModuleContainer<TModel, TRepo> c) : base()
        {
            GetWithId = id =>
            {
                if (!c.Find(id, out TModel model))
                {
                    return c.AsController.NotFound();
                }

                return c.AsController.View(model);
            };
            
            Post = model =>
            {
                if (!c.Find(model.Id, out TModel _))
                {
                    return c.AsController.NotFound();
                }

                if (c.AsController.ModelState.IsValid)
                {
                    return c.UpdateRepo(
                        model, 
                        c.Repo.Update, 
                        feedback: "updated");
                }

                return c.AsController.View(model);
            };
        }
    }
}