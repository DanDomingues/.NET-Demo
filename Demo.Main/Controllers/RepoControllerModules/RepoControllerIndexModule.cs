using Demo.DataAccess.IRepository;
using Demo.Models;

namespace Demo.Main.Controllers.RepoControllerModules
{
    public class RepoControllerIndexModule<TModel, TRepo> : RepositoryControllerModule<TModel, TRepo>
        where TModel : class, IModelBase, new()
        where TRepo : IRepository<TModel>
    {
        public RepoControllerIndexModule(
            IRepoControllerModuleContainer<TModel, TRepo> c,
            string? includeProperties) : base()
        {
            Get = () => 
            {
                var all = c.Repo.GetAll(includeProperties: includeProperties);
                return c.AsController.View(all);
            };
        }
    }
}