﻿namespace MyWebApi.DTOs.Requests
{
    public record PaymentCallBack(
        string TransactionId,
        string GatewayResponse);
}
