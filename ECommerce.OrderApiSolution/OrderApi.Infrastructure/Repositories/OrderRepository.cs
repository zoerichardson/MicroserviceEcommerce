using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entites;
using OrderApi.Infrastructure.Data;
using Serilog;
using System.Linq.Expressions;


namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRespository(OrderDbContext context) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                var order = context.MSOrders.Add(entity).Entity;
                await context.SaveChangesAsync();
                return order.Id > 0 ? new Response(true, "order placed successfully") :
                    new Response(false, "error occured while placing order");

            }
            catch (Exception e)
            {
                // log original exception
                LogException.LogExceptions(e);

                //display scary-free message to the client;
                return new Response(false, "Error occured while placing order");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order is null)
                    return new Response(false, "Order not found");

                context.MSOrders.Remove(entity);
                await context.SaveChangesAsync();
                return new Response(true, "Order sucessfully deleted");
            }
            catch (Exception e)
            {
                // log original exception
                LogException.LogExceptions(e);

                //display scary-free message to the client;
                return new Response(false, "Error occured while deleting order");
            }
        }

        public async Task<Order> FindByIdAsync(int id)
        {
            try
            {
                var order = await context.MSOrders!.FindAsync(id);
                return order is not null ? order : null!;
            }
            catch (Exception e)
            {
                // log original exception
                LogException.LogExceptions(e);

                //display scary-free message to the client;
                throw new Exception("Error occured while getting order");
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                var orders = await context.MSOrders.AsNoTracking().ToListAsync();
                return orders is not null ? orders : null!;
            }
            catch (Exception e)
            {
                // log original exception
                LogException.LogExceptions(e);

                //display scary-free message to the client;
                throw new Exception("Error occured while getting all orders");
            }
        }

        //where is this used?

        public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.MSOrders.Where(predicate).FirstOrDefaultAsync()!;
                return order is not null ? order : null!;
            }
            catch (Exception e)
            {
                // log original exception
                LogException.LogExceptions(e);

                //display scary-free message to the client;
                throw new Exception("Error occured while getting order");
            }
        }

        //used in GetClientOrders() - order controller
        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var orders = await context.MSOrders.Where(predicate).ToListAsync();
                return orders is not null ? orders : null!;
            }
            catch (Exception e)
            {
                // log original exception
                LogException.LogExceptions(e);

                //display scary-free message to the client;
                throw new Exception("Error occured while getting orders");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order is null)
                {
                    return new Response(false, "Order not found");
                }

                context.Entry(order).State = EntityState.Detached;
                context.MSOrders.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, "Order updated");
            }
            catch (Exception e)
            {
                // log original exception
                LogException.LogExceptions(e);

                //display scary-free message to the client;
                return new Response(false, "Error occured while placing order");
            }
        }
    }

}
