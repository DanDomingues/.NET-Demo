using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Debut.Areas.Admin.Controllers
{
    public abstract class RepositoryBoundController<TModel, TRepo>(IUnitOfWork unitOfWork) : Controller
        where TModel : ModelBase, new()
        where TRepo : IRepository<TModel>
    {
        protected IUnitOfWork unitOfWork = unitOfWork;
        protected abstract TRepo Repo { get; }
        protected abstract string DefaultFeedbackName { get; }
        protected virtual string? IncludedApiProperties => null;

        protected void AddOperationFeedback(
            string name,
            string? objName = null)
        {
            objName ??= DefaultFeedbackName;
            TempData["success"] = $"{objName} {name} successfully";
        }

        private bool Find(int? id, out TModel output)
        {
            if (id == null || id == 0)
            {
                output = new TModel();
                return false;
            }

            output = Repo.GetById(id) ?? new();

            if (output == null)
            {
                return false;
            }

            return true;
        }

        public virtual IActionResult Index()
        {
            return View(Repo.GetAll());
        }

        protected IActionResult UpdateRepo(
            TModel obj, 
            Action<TModel> action,
            string feedback = "success",
            string redirection = "Index")
        {
            action(obj);
            unitOfWork.Save();
            AddOperationFeedback(feedback);
            return RedirectToAction(redirection);
        }

        protected bool CheckForDuplicates(
            TModel model, 
            string feedbackName = "entry",
            bool addModelError = true)
        {
            var duplicate = Repo.GetFirstOrDefault(c => string.Equals(c.Name.ToLower(), model.Name.ToLower()));
            if (duplicate != null)
            {
                if(addModelError)
                {
                    ModelState.AddModelError(
                        "Name",
                        $"A {feedbackName} with the name '{model.Name}' was already added.");
                    ModelState.AddModelError(
                        "",
                        $"A {feedbackName} with the name '{model.Name}' was already added");
                }
                return false;
            }

            return true;
        }

        [HttpPost]
        public virtual IActionResult Create(TModel obj)
        {
            if (ModelState.IsValid)
            {
                return UpdateRepo(obj, Repo.Add, feedback : "created");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Edit(TModel obj)
        {
            if(!Find(obj.Id, out TModel output))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                return UpdateRepo(obj, Repo.Update, feedback: "updated");
            }

            return View();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            if (!Find(id, out var obj))
            {
                return NotFound();
            }

            return UpdateRepo(obj, Repo.Remove, feedback: "deleted");
        }

        public virtual IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new TModel());
            }

            return View(Repo.GetById(id));
        }

        [HttpPost]
        public IActionResult Upsert(TModel model)
        {
            if (CheckForDuplicates(model))
            {
                return View();
            }

            if (ModelState.IsValid)
            {
                UpdateRepo(model, Repo.AddOrUpdate, feedback: "created");
            }

            return View();
        }

        #region APICalls
        [HttpGet]
        public virtual IActionResult GetAll()
        {
            var list = Repo.GetAll(includeProperties: IncludedApiProperties);
            return Json(new { data = list });
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
