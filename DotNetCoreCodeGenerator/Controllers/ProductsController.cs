using DbInfrastructure.Entities;
using DbInfrastructure.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using HelpersProject;
using DotNetCoreCodeGenerator.Domain;

namespace DotNetCoreCodeGenerator.Controllers
{

    //[Route("[controller]/[action]")]
    public class ProductsController : BaseController
    {
        public IProductService ProductService { get; set; }
        private ILogger<ProductsController> Logger { get; set; }

        public ProductsController(
            IProductService ProductService,     
            ILoggerFactory loggerFactory,
            MyAppSetttings myAppSetttings) :base(loggerFactory, myAppSetttings)
        {
            this.ProductService = ProductService;
            this.Logger = loggerFactory.CreateLogger<ProductsController>();
     
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
                    return RedirectToAction(nameof(Index));
                }

            }
           
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            var t = await ProductService.SaveOrUpdateAsync(product, product.Id);

            return RedirectToAction(nameof(Index));
        }
         
    }
}