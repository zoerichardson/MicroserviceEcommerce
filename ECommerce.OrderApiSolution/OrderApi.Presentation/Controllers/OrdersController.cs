using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController(IOrder orderInterface, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            //retrive data from Order Repository > Dbcontext
	    await Task.Delay(4000);
            var orders = await orderInterface.GetAllAsync();
            if (!orders.Any())
                return NotFound("No orders found in the database");

            //convert from Order to OrderDTO
            var (_, list) = OrderConversion.FromEntity(null, orders);
            return !list!.Any() ? NotFound() : Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {

            //same functionality route as GetOrders()
            var order = await orderInterface.FindByIdAsync(id);
            if (order is null)
                return NotFound(null);

            var (_order, _) = OrderConversion.FromEntity(order, null);
            return Ok(_order);
        }

        [HttpGet("client/{clientid}")]
        public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientid)
        {
            if (clientid <= 0) return BadRequest("invalid data provided");

            var orders = await orderService.GetOrdersByClientId(clientid);
            return !orders.Any() ? NotFound(null) : Ok(orders);
        }

       
        [HttpGet("details/{orderId}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0) return BadRequest("invalid data provided");

            var orderDetails = await orderService.GetOrderDetails(orderId);
            return orderDetails.OrderId > 0 ? Ok(orderDetails) : NotFound("No order found");

        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDTO)
        {
            //check model state if all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest("Incomplete data submitted");

            //convert to OrderDTO to Order 
            var getEntity = OrderConversion.ToEntity(orderDTO);

            //send data to Order Repository
            var response = await orderInterface.CreateAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO orderDTO)
        {
            //convert to OrderDTO to Order 
            var order = OrderConversion.ToEntity(orderDTO);

            //update the data in the repository 
            var response = await orderInterface.UpdateAsync(order);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDTO orderDTO)
        {
            //convert from OrderDTO to Order
            var order = OrderConversion.ToEntity(orderDTO);

            //delete from order repository 
            var response = await orderInterface.DeleteAsync(order);
            return response.Flag ? Ok(response) : BadRequest(response);
        }


    }
}
