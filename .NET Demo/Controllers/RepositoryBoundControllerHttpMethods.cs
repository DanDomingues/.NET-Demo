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
    //TODO-1: Consider moving those to modules, instead of shoving it to a partial. Partials are sometimes described as code smells 
    public abstract partial class RepositoryBoundController<TModel, TRepo> : Controller
        where TModel : class, IModelBase, new()
        where TRepo : IRepository<TModel>
    {
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