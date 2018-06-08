using DbInfrastructure.Entities;
using DbInfrastructure.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DotNetCoreCodeGenerator.Controllers
{
    public class ProductsController : Controller
    {
        public IProductService ProductService { get; set; }
        public ProductsController(IProductService ProductService)
        {
            this.ProductService = ProductService;
        }
        public async Task<IActionResult> Index()
        {
            var items = await ProductService.GetAllAsync();
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id=0)
        {
            Product model = new Product();
            if (id > 0)
            {
                model = await ProductService.GetSingleAsync(id);

                if (model == null)
                {
                    return RedirectToAction("Index");
                }

            }
           
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            var t = await ProductService.SaveOrUpdateAsync(product, product.Id);
          
            return RedirectToAction("Index");
        }
         
    }
}