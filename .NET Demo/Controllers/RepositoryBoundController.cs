using Demo.DataAccess.Repository;
using Demo.DataAccess.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using ASP.NET_Debut.Controllers.RepoControllerModules;

namespace ASP.NET_Debut.Controllers
{
    public abstract class RepositoryBoundController<TModel, TRepo> : Controller, IRepoControllerModuleContainer<TModel, TRepo>
        where TModel : class, IModelBase, new()
        where TRepo : IRepository<TModel>
    {
        protected IUnitOfWork unitOfWork;
        protected abstract TRepo Repo { get; }
        protected abstract string DefaultFeedbackName { get; }
        protected virtual string? DefaultIncludeProperties => null;

        TRepo IRepoControllerModuleContainer<TModel, TRepo>.Repo => Repo;
        Controller IRepoControllerModuleContainer<TModel, TRepo>.AsController => this;


        protected readonly Dictionary<string, RepositoryControllerModule<TModel, TRepo>> Modules;

        public RepositoryBoundController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            //TODO-1: After functioning is validated, create appropriate modules on each class
            //Very innocous at this point, as these are simply action holders
            //Therefore, more of an engineering principle refactor than a necessity
            Modules = new()
            {
                { "Index", new RepoControllerIndexModule<TModel, TRepo>(this, DefaultIncludeProperties) },
                { "Upsert", new RepoControllerUpsertModule<TModel, TRepo>(this) },
                { "Delete", new RepoControllerDeleteModule<TModel, TRepo>(this) },
                { "Create", new RepoControllerCreateModule<TModel, TRepo>(this) },
                { "Edit", new RepoControllerEditModule<TModel, TRepo>(this) }
            };
        }

        protected IActionResult UpdateRepo(
            TModel obj, 
            Action<TModel> action,
            string feedback = "success",
            string redirection = "Index",
            object? redirectionArgs = null)
        {
            action(obj);
            unitOfWork.Save();
            this.AddOperationFeedback(feedback, objName: DefaultFeedbackName);
            return redirectionArgs != null ? RedirectToAction(redirection, redirectionArgs) : RedirectToAction(redirection);
        }

        protected bool Find(
            int? id, 
            out TModel output, 
            bool track = false)
        {
            if (id == null || id == 0)
            {
                output = new TModel();
                return false;
            }

            output = Repo.GetById(id, track: track) ?? new();

            if (output == null)
            {
                return false;
            }

            return true;
        }

        protected bool CheckForDuplicates(
            TModel model,
            Expression<Func<TModel, bool>> expression,
            Func<TModel, string> getPropertyAsString,
            string propertyName,
            string feedbackName = "entry",
            bool addModelError = true,
            bool ignoreNonZeroIds = true)
        {
            if(model.Id != 0 && ignoreNonZeroIds)
            {
                return false;
            }

            var duplicate = Repo.GetFirstOrDefault(expression);

            if (duplicate != null)
            {
                if(addModelError)
                {
                    var prop = getPropertyAsString(model);
                    ModelState.AddModelError(
                        "Name",
                        $"A {feedbackName} with the {propertyName} '{prop}' was already added.");
                }
                return true;
            }

            return false;
        }

        protected bool CheckForDuplicatesByName<T>(
            T model,
            string feedbackName = "entry",
            bool addModelError = true,
            bool ignoreNonZeroIds = true) where T : TModel, INamedModel
        {
            return CheckForDuplicates(
                model,
                e => (e as INamedModel).Name == model.Name,
                m => (m as INamedModel).Name,
                "Name",
                feedbackName: feedbackName,
                addModelError: addModelError,
                ignoreNonZeroIds: ignoreNonZeroIds);
        }

        protected virtual bool ValidateForUpsert(TModel model)
        {
            return true;
        }

        bool IRepoControllerModuleContainer<TModel, TRepo>.Find(
            int? id, 
            out TModel output, 
            bool track)
        {
            return Find(id, out output, track);
        }

        IActionResult IRepoControllerModuleContainer<TModel, TRepo>.UpdateRepo(
            TModel obj, Action<TModel> action, 
            string feedback, string redirection, 
            object? redirectionArgs)
        {
            return UpdateRepo(obj, action, feedback, redirection, redirectionArgs);
        }

        bool IRepoControllerModuleContainer<TModel, TRepo>.ValidateForUpsert(TModel model)
        {
            return ValidateForUpsert(model);
        }

        #region APICalls
        [HttpGet]
        public virtual IActionResult GetAll()
        {
            var all = Repo.GetAll(includeProperties: DefaultIncludeProperties);
            return Json(new { data = all });
        }

        [HttpDelete]
        public virtual IActionResult DeleteAt(int id)
        {
            var obj = Repo.GetById(id);
            if (obj == null)
            {
                return Json(new 
                { 
                    success = false, 
                    message = "Error while deleting" 
                });
            }

            Repo.Remove(obj);
            unitOfWork.Save();

            return Json(new 
            { 
                success = true, 
                message = "Delete Successful" 
            });
        }
        #endregion
    }

    public interface IRepoControllerModuleContainer<TModel, TRepo>
        where TModel : class, IModelBase, new()
        where TRepo : IRepository<TModel>
    {
        TRepo Repo { get; }
        Controller AsController { get; }

        IActionResult UpdateRepo(
            TModel obj,
            Action<TModel> action,
            string feedback = "success",
            string redirection = "Index",
            object? redirectionArgs = null);

        bool Find(
            int? id, 
            out TModel output, 
            bool track = false);

        bool ValidateForUpsert(TModel model);
    }
}
