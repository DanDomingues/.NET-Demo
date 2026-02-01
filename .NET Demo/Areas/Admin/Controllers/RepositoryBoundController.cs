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
        protected virtual string? DefaultIncludeProperties => null;

        protected void AddOperationFeedback(
            string name,
            string? objName = null)
        {
            objName ??= DefaultFeedbackName;
            TempData["success"] = $"{objName} {name} successfully";
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

        protected bool CheckForDuplicates<T>(
            T model,
            IRepository<T> repo,
            string feedbackName = "entry",
            bool addModelError = true,
            bool ignoreNonZeroIds = false)
            where T : class, INamedModel
        {
            if(model.Id != 0 && ignoreNonZeroIds)
            {
                return false;
            }

            var duplicate = repo.GetFirstOrDefault(c => string.Equals(c.Name.ToLower(), model.Name.ToLower()));

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
                return true;
            }

            return false;
        }

        public virtual IActionResult Index()
        {
            //Might be best to have the .ToList() conversion inside the GetAll method, depending on future use cases
            return View(Repo.GetAll(includeProperties: DefaultIncludeProperties).ToList());
        }

        public IActionResult Create()
        {
            return View(new TModel());
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

        public IActionResult Edit(int? id)
        {
            if (!Find(id, out TModel model))
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(TModel model)
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

        public IActionResult Delete(int? id)
        {
            if (!Find(id, out TModel model))
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(TModel model)
        {
            if (!Find(model.Id, out var foundModel))
            {
                return NotFound();
            }

            return UpdateRepo(foundModel, Repo.Remove, feedback: "deleted");
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
            if(ModelState.IsValid && ValidateForUpsert(model))
            {
                UpdateRepo(model, Repo.AddOrUpdate, feedback: model.Id == 0 ? "created" : "updated");
            }
            return RedirectToAction(nameof(Index));
        }

        protected virtual bool ValidateForUpsert(TModel model)
        {
            if(model is INamedModel namedModel && CheckForDuplicates(namedModel, Repo as IRepository<INamedModel>, ignoreNonZeroIds: true))
            {
                return false;
            }
            return true;
        }

        #region APICalls
        [HttpGet]
        public virtual IActionResult GetAll()
        {
            var list = Repo.GetAll(includeProperties: DefaultIncludeProperties);
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
