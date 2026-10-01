using OrderApi.Domain.Entites;

namespace OrderApi.Application.DTOs.Conversions
{
    public static class OrderConversion
    {
        //coming in as a DTO (coming from client)
        //to output as an Order (for the database)
        public static Order ToEntity(OrderDTO order) => new()
        {
            Id = order.Id,
            ProductId = order.ProductId,
            ClientId = order.ClientId,
            PurchaseQuantity = order.PurchaseQuantity,
            OrderedDate = order.OrderedDate,
            
        };


        //coming in as an Order(s) from the database
        //outputting as an Order(s)DTO for the client
        public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<Order>? orders)
        {
            //return single 
            if (order is not null || orders is null)
            {
                var singleOrder = new OrderDTO(
                    order!.Id,
                    order.ProductId,
                    order.ClientId,
                    order.PurchaseQuantity,
                    order.OrderedDate);

                return (singleOrder, null);
            }

            //if returning a list of orders
            if (orders is not null || order is null)
            {
                var _orders = orders!.Select(o =>
                new OrderDTO(o.Id,
                o.ProductId,
                o.ClientId,
                o.PurchaseQuantity,
                o.OrderedDate));

                return (null, _orders);
            }

            return (null, null);
        }
    }
}

