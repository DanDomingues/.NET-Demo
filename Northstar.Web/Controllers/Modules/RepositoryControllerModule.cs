using Northstar.DataAccess.IRepository;
using Northstar.Models;
using Microsoft.AspNetCore.Mvc;

namespace Northstar.Web.Controllers.Modules
{
    public abstract class RepositoryControllerModule<TModel, TRepo>
        where TModel : class, IModelBase, new()
        where TRepo : IRepository<TModel>
    {
        public Func<IActionResult> Get { get; protected set; } = null!;
        public Func<int?, IActionResult> GetWithId { get; protected set; } = null!;
        public Func<TModel, IActionResult> Post { get; protected set; } = null!;
        public Func<int?, IActionResult> PostWithId { get; protected set; } = null!;
    }
}