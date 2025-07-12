using MyWebApi.Models;

namespace MyWebApi.DTOs.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDTO orderDto)
        {
            var order = new Order
            {
                Id = orderDto.Id,
                UserId = orderDto.UserId,
                OrderDate = orderDto.OrderDate,
                TotalAmount = orderDto.TotalPrice,
                ShippingCost = orderDto.ShippingCost,
                PaymentMethod = orderDto.PaymentMethod,
                PaymentStatus = orderDto.PaymentStatus,
                Notes = orderDto.Notes,
                CreatedAt = orderDto.CreatedAt,
                FullName = orderDto.FullName,
                Phone = orderDto.Phone,
                Address = orderDto.Address,
                City = orderDto.City,
                Items = orderDto.OrderDetails.Select(od => new OrderDetail
                {
                    Id = od.Id,
                    ProductId = od.productId,
                    ProductName = od.productName,
                    OrderId = od.orderId,
                    Count = od.count,
                    Price = od.price,

                }).ToList()
            };
            return order;

        }

        public static (OrderDTO? , IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<Order>? orders)
        {
            //tra ve 1 orderDTO
            if (order is not null || orders is null)
            {
                var singleOrder = new OrderDTO(order!.Id,
                    order.UserId,
                    order.OrderDate,
                    order.Status!,
                    order.TotalAmount,
                    order.ShippingCost,
                    order.PaymentMethod,
                    order.PaymentStatus,
                    order.Notes,
                    order.CreatedAt,
                    order.FullName,
                    order.Phone,
                    order.Address,
                    order.City,
                    order.Items.Select(od => new OrderDetailDTO(
                        od.Id,
                        od.ProductId,
                        od.ProductName,
                        od.OrderId, 
                        od.Count,
                        od.Price)).ToList()
                    );
                return (singleOrder, null);
            }
            if (order is null || orders is not null)
            {
                //tra ve nhieu orderDTO
                var orderDtos = orders!.Select(o => new OrderDTO(
                    o.Id,
                    o.UserId,
                    o.OrderDate,
                    o.Status!,
                    o.TotalAmount,
                    o.ShippingCost,
                    o.PaymentMethod,
                    o.PaymentStatus,
                    o.Notes,
                    o.CreatedAt,
                    o.FullName,
                    o.Phone,
                    o.Address,
                    o.City,
                    o.Items.Select(od => new OrderDetailDTO(
                        od.Id,
                        od.ProductId,
                        od.ProductName,
                        od.OrderId, 
                        od.Count,
                        od.Price)).ToList()
                )).ToList();
                return (null, orderDtos);
            }
            return (null, null);
        }
    }
}
