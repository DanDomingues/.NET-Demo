using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Debut.Areas.Admin.Controllers
{
    public class CompanyController : Controller
    {
        IUnitOfWork unitOfWork;
        ICompanyRepository repo => unitOfWork.CompanyRepository;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;          
        }

        private void AddOperationFeedback(
            string name, 
            string objName = "Category")
        {
            TempData["success"] = $"{objName} {name} successfully";
        }

        private bool Find(int? id, out Company company)
        {
            if(id == null || id == 0)
            {
                company = new Company();
                return false;
            }

            company = repo.GetFirstOrDefault(u => u.Id == id) ?? new();

            if(company == null)
            {
                return false;
            }

            return true;
        }

        public IActionResult Index()
        {
            return View(repo.GetAll());
        }

        [HttpPost]
        public IActionResult Create(Company company)
        {
            if(ModelState.IsValid)
            {
                repo.Add(company);
                unitOfWork.Save();
                AddOperationFeedback("created");
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Edit(Company company)
        {
            if (ModelState.IsValid)
            {
                repo.Update(company);
                unitOfWork.Save();
                AddOperationFeedback("edited");
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            if(!Find(id, out var company))
            {
                return NotFound();
            }
            repo.Remove(company);
            unitOfWork.Save();
            AddOperationFeedback("deleted");
            return RedirectToAction("Index");
        }
    }
}
