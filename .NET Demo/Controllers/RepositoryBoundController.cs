using Demo.DataAccess.Repository;
using Demo.DataAccess.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace ASP.NET_Debut.Controllers
{
    public abstract class RepositoryBoundController<TModel, TRepo>(IUnitOfWork unitOfWork) : Controller
        where TModel : class, IModelBase, new()
        where TRepo : IRepository<TModel>
    {
        protected IUnitOfWork unitOfWork = unitOfWork;
        protected abstract TRepo Repo { get; }
        protected abstract string DefaultFeedbackName { get; }
        protected virtual string? DefaultIncludeProperties => null;

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

        private bool Find(int? id, out TModel output, bool track = false)
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

        //TODO-5: Possibly move this to a partial class
        #region HTTP GET

        protected IActionResult IndexInternal()
        {
            return View(Repo.GetAll(includeProperties: DefaultIncludeProperties));
        }

        protected IActionResult CreateInternal()
        {
            return View(new TModel());
        }

        protected IActionResult EditInternal(int? id)
        {
            if (!Find(id, out TModel model))
            {
                return NotFound();
            }

            return View(model);
        }

        protected IActionResult DeleteInternal(int? id)
        {
            if (!Find(id, out TModel model))
            {
                return NotFound();
            }

            return View(model);
        }

        protected IActionResult UpsertInternal(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new TModel());
            }

            return View(Repo.GetById(id));
        }

        #endregion

        #region HTTP POST

        protected IActionResult CreateInternalOnPost(TModel obj)
        {
            if (ModelState.IsValid)
            {
                return UpdateRepo(obj, Repo.Add, feedback : "created");
            }

            return View();
        }

        protected IActionResult EditInternalOnPost(TModel model)
        {
            if(!Find(model.Id, out TModel _))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                return UpdateRepo(model, Repo.Update, feedback: "updated");
            }

            return View();
        }

        protected IActionResult DeleteInternalOnPost(TModel model)
        {
            if (!Find(model.Id, out var foundModel))
            {
                return NotFound();
            }

            return UpdateRepo(foundModel, Repo.Remove, feedback: "deleted");
        }

        protected IActionResult UpsertInternalOnPost(TModel model)
        {
            if(ModelState.IsValid && ValidateForUpsert(model))
            {
                UpdateRepo(model, Repo.AddOrUpdate, feedback: model.Id == 0 ? "created" : "updated");
            }
            return RedirectToAction(nameof(Index));
        }
        
        #endregion

        #region APICalls
        [HttpGet]
        public virtual IActionResult GetAll()
        {
            var all = Repo.GetAll(includeProperties: DefaultIncludeProperties);
            return Json(new { data = all });
        }

        [HttpDelete]
        public virtual IActionResult Delete(int id)
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
}
