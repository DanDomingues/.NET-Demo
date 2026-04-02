using Demo.DataAccess.IRepository;
using Demo.Models;

namespace Demo.Main.Controllers.Modules
{
    public class RepoControllerCreateModule<TModel, TRepo> : RepositoryControllerModule<TModel, TRepo>
        where TModel : class, IModelBase, new()
        where TRepo : IRepository<TModel>
    {
        public RepoControllerCreateModule(IRepoControllerModuleContainer<TModel, TRepo> c) : base()
        {
            Get = () => c.AsController.View(new TModel());
            Post = model =>
            {
                if(c.AsController.ModelState.IsValid)
                {
                    //TODO: Change structure to access given methods through an interface
                    return c.UpdateRepo(model, c.Repo.Add, feedback: "created");
                }
                return c.AsController.View();
            };
        }
    }
}