namespace MyWebApi.Enums
{
    public enum OrderStatus
    {
        Pending = 1, //Đang chờ 
        Confirmed = 2, //Đã xác nhận
        Processing = 3, //Đang xử lý
        Shipping = 4, //Đang giao hàng
        Delivered = 5, //Đã giao hàng
        Cancelled = 6, //Đã hủy
        Returned = 7 //Đã hoàn 
    }
}
