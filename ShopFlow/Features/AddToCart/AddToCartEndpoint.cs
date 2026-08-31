using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopFlow.Data;
using ShopFlow.Models;

namespace ShopFlow.Features.AddToCart;

public static class AddToCartEndpoint
{
    private const string DemoUserId = "demo-user-1";

    public static void MapAddToCartEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/cart/items", async (
            [FromBody] AddToCartRequest request,
            ShopFlowDbContext db,
            CancellationToken cancellationToken) =>
        {
            var (isValid, validationErrors) = AddToCartValidator.Validate(request);
            if (!isValid)
            {
                return Results.BadRequest(new { message = "Validation failed.", errors = validationErrors });
            }

            var product = await db.Products
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product is null)
            {
                return Results.NotFound(new { message = $"Product with ID {request.ProductId} was not found." });
            }

            var cart = await db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == DemoUserId, cancellationToken);

            if (cart is null)
            {
                cart = new Cart { UserId = DemoUserId };
                db.Carts.Add(cart);
                await db.SaveChangesAsync(cancellationToken);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            int currentCartQuantity = existingItem?.Quantity ?? 0;
            int totalRequestedQuantity = currentCartQuantity + request.Quantity;

            if (totalRequestedQuantity > product.StockQuantity)
            {
                return Results.Conflict(new
                {
                    message = $"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}, Requested in cart: {totalRequestedQuantity}."
                });
            }

            CartItem affectedItem;
            if (existingItem is not null)
            {
                existingItem.Quantity = totalRequestedQuantity;
                affectedItem = existingItem;
            }
            else
            {
                affectedItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = request.Quantity
                };
                cart.Items.Add(affectedItem);
            }

            await db.SaveChangesAsync(cancellationToken);

            var response = new AddToCartResponse
            {
                CartItemId = affectedItem.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = affectedItem.Quantity,
                UnitPrice = product.Price,
                TotalPrice = product.Price * affectedItem.Quantity
            };

            return Results.Created($"/api/cart/items/{affectedItem.Id}", response);
        })
        .WithName("AddToCart")
        .WithTags("Cart")
        .Produces<AddToCartResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict)
        .WithSummary("Add a product to the user's shopping cart")
        .WithDescription("Demonstrates a single vertical slice: validates input, ensures stock availability, and updates the cart.");
    }
}
