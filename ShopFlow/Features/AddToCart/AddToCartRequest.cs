using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Features.AddToCart;

public class AddToCartRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0.")]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }
}
