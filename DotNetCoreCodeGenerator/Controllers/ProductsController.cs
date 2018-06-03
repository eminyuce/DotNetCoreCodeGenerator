using DbInfrastructure.Entities;
using DbInfrastructure.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreCodeGenerator.Controllers
{
    public class ProductsController : Controller
    {
        public IProductService ProductService { get; set; }
        public ProductsController(IProductService ProductService)
        {
            this.ProductService = ProductService;
        }
        public IActionResult Index()
        {
            var items = ProductService.GetAll();
            return View(items);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = ProductService.GetSingle(id);

            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(Product product)
        {
           ProductService.SaveOrEditEntity(product);
           return RedirectToAction("Index");
        }
    }
}