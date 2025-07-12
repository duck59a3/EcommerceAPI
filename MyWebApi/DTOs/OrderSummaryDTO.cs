using MyWebApi.Enums;

namespace MyWebApi.DTOs
{
    public record OrderSummaryDTO(
        int Id,
        DateTime orderDate,
        OrderStatus orderStatus,
        int GrandTotal);

}
