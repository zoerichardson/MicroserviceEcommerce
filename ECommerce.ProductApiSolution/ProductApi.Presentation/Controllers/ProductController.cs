using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [AllowAnonymous]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            //get all products from ProductRepository, although when did that happen???????????
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
                return NotFound("No products detected in the database");

            //convert data from entity to DTO and return it
            var (_, list) = ProductConversions.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No product found");
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            // get single product from the ProductRepository 
            var product = await productInterface.FindByIdAsync(id);
            if (product is null)
                return NotFound("Product requested not found");

            //convert from entity to DTO and return
            var (_product, _) = ProductConversions.FromEntity(product, null!);
            return _product is not null ? Ok(_product) : NotFound("Product not found");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            //check model state is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //convert from DTO to entity
            var getEntity = ProductConversions.ToEntity(product);
            var response = await productInterface.CreateAsync(getEntity);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            //check model state is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //convert from DTO to entity
            var getEntity = ProductConversions.ToEntity(product);
            var response = await productInterface.UpdateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            //convert from DTO to entity
            var getEntity = ProductConversions.ToEntity(product);
            var response = await productInterface.DeleteAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);

        }

    }
}
