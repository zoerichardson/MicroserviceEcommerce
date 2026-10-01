using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly;
using Polly.Registry;
using System.Net.Http.Json;


namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface, HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {

        //GET PRODUCT
        public async Task<ProductDTO> GetProduct(int productId)
        {
            //Call ProductAPI using HttpClient
            //Redirect this call to the API Gateway since ProductsAPI is not responding to outsiders
            var getProduct = await httpClient.GetAsync($"api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null!;

            //returns product and de serialize from json - then return product as productDTO
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }

        //GET USER
        public async Task<AppUserDTO> GetUser(int userId)
        {
            //call AuthenticationAPI using HttpClient
            //Redirect this call to the API Gateway since ProductsAPI is not responding to outsiders
            var getUser = await httpClient.GetAsync($"api/authentication/id?id={userId}");
            if (!getUser.IsSuccessStatusCode)
                return null!;

            //returns user and de serialize from json - then return user as userDTO
            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user!;
        }

        //GET ORDER DETAILS BY ID
	    //using Polly is for error handling 
        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            //prepare order
            var order = await orderInterface.FindByIdAsync(orderId);
            if (order is null || order!.Id <= 0)
                return null!;
            
            //Get Retry pipeline
	    //this is where the pipleline is defined 
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            //Prepare products
	    //wrap http call in ExecuteAsync method
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            //Prepare client
           //wrap http call in ExecuteAsync method
           var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId)); 

            //populate order details
            return new OrderDetailsDTO(
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.PhoneNumber,
                productDTO.Name,
               order.PurchaseQuantity,
                productDTO.Price,
                productDTO.Quantity * order.PurchaseQuantity,
                order.OrderedDate
                );
        }

        //GET orders by client id
        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            //get all clients orders
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
            if (!orders.Any()) return null!;

            //convert from entity  to DTO
            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;

        }
    }
}
