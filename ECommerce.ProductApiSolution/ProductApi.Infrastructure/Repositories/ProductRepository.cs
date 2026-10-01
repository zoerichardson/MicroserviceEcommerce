using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entites;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
    internal class ProductRepository(ProductDbContext context) : IProduct
    {

        //IProduct is the abstract declaration of these functions. but the actual functionality
        //is implemented here

        //repository is the comminication with the db
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                //check if product already exists
                var getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
                if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                    return new Response(false, $"{entity.Name} already exists");

                //
                var currentEntity = context.MSProducts.Add(entity).Entity;
                await context.SaveChangesAsync();
                if (currentEntity is not null && currentEntity.Id > 0)
                    return new Response(true, $"{entity.Name} added to the database successfully");
                else
                    return new Response(false, $"Error occurred while adding {entity.Name}");

            }
            catch (Exception ex)
            {
                //log the original exception
                LogException.LogExceptions(ex);

                //display scary-free messge to the client
                return new Response(false, "Error occurred adding new product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} does not exist");


                context.MSProducts.Remove(product);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} deleted from the database successfully");

            }
            catch (Exception ex)
            {
                //log the original exception
                LogException.LogExceptions(ex);

                //display scary-free messge to the client
                return new Response(false, "Error occurred deleting the product");
            }
        }
        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} does not exist");

                context.Entry(product).State = EntityState.Detached;
                context.MSProducts.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} updated from the database successfully");

            }
            catch (Exception ex)
            {
                //log the original exception
                LogException.LogExceptions(ex);

                //display scary-free messge to the client
                return new Response(false, "Error occurred updating the product");
            }
        }

        //find one product by Id
        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.MSProducts.FindAsync(id);
                return product is not null ? product : null!;

            }
            catch (Exception ex)
            {
                //log the original exception
                LogException.LogExceptions(ex);

                //display scary-free messge to the client
                throw new Exception("Error occurred while finding the product");
            }
        }

        //get list of all products 
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.MSProducts.AsNoTracking().ToListAsync();
                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Error occurred while finding the products");
            }
        }

        //
        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.MSProducts.Where(predicate).FirstOrDefaultAsync()!;
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Error occurred while finding the product");
            }
        }

        
    }
}
