namespace CHKS.Entity;

public record Order(
    Guid Id,
    int Amount,
    DateOnly OrderDate,
    DateOnly? DeliveryDate,
    DateOnly? OrderReceivedDate,
    string Description,
    bool IsCancelled,
    bool IsOrderReceived,
    decimal TotalPrice,
    Guid ProductId
);