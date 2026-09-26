using EcommerceAPI.DTO;
using EcommerceAPI.DTOs.Checkout;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;
using System.Security.Claims;

namespace EcommerceAPI.Services.Implementations
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICheckoutRepository _checkoutRepository;
        private readonly IProductPriceService _productPriceService;
        private readonly IProductAvailabilityService _productAvailabilityService;
        private readonly IOrderService _orderService;
        private readonly IAddressService _addressService;

        public CheckoutService(
            ICheckoutRepository checkoutRepository,
            IProductPriceService productPriceService,
            IProductAvailabilityService productAvailabilityService,
            IOrderService orderService,
            IAddressService addressService)
        {
            _checkoutRepository = checkoutRepository;
            _productPriceService = productPriceService;
            _productAvailabilityService = productAvailabilityService;
            _orderService = orderService;
            _addressService = addressService;
        }

        public async Task<CheckoutResponseDto> CalculateCheckoutAsync(
            int customerId,
            string customerName,
            CheckoutRequestDto request,
            CancellationToken cancellationToken)
        {

            // 1. Get Cart

            var cartItems =
                await _checkoutRepository.GetCartItemsAsync(
                    customerId,
                    cancellationToken);

            if (!cartItems.Any())
            {
                throw new InvalidOperationException(
                    "Cart is empty.");
            }


  
            // 2. Get Selected Address
            var addresses =
                await _addressService.GetAllAsync(
                    customerId,
                    cancellationToken);

            var address = addresses.FirstOrDefault(
                a => a.AddressId == request.AddressId);

            if (address == null)
            {
                throw new InvalidOperationException(
                    "Selected address was not found.");
            }

            // 3. Check Stock

            var priceChanges = new List<PriceChangeDto>();
            var stockIssues = new List<StockIssueDto>();

            var inventoryItems = cartItems
                .Select(item => new InventoryCheckItemDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                })
                .ToList();

            var inventoryResults =
                await _productAvailabilityService.CheckInventoryAsync(
                    inventoryItems,
                    cancellationToken);

            foreach (var result in inventoryResults)
            {
                if (!result.IsAvailable)
                {
                    var cartItem = cartItems.First(
                        item => item.ProductId == result.ProductId);

                    stockIssues.Add(new StockIssueDto
                    {
                        ProductId = result.ProductId,
                        ProductName = cartItem.ProductName,
                        RequestedQuantity = result.RequestedQuantity,
                        AvailableQuantity = result.AvailableQuantity
                    });
                }
            }

            // 4. Check Price Changes
      
            foreach (var item in cartItems)
            {
                var currentPrice =
                    await _productPriceService
                        .GetCurrentPriceByProductIdAsync(
                            item.ProductId);

                if (currentPrice != null &&
                    currentPrice.Price != item.Price)
                {
                    priceChanges.Add(new PriceChangeDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        OldPrice = item.Price,
                        CurrentPrice = currentPrice.Price
                    });
                }
            }


            // 5. Stop if Stock / Price Problems
      
            if (priceChanges.Any() || stockIssues.Any())
            {
                return new CheckoutResponseDto
                {
                    PriceChanges = priceChanges,
                    StockIssues = stockIssues
                };
            }

            // 6. Calculate Totals
         
            var subtotal = cartItems.Sum(
                item => item.ItemSubtotal);

            decimal shippingCharge =
                request.ShippingMethod.ToLower() switch
                {
                    "standard" => 100,
                    "express" => 250,
                    "same day" => 400,
                    _ => 0
                };

            var tax = subtotal * 0.18m;

            var totalAmount =
                subtotal +
                shippingCharge +
                tax;


       
            // 7. Create Order
       
            var order =
                new EcommerceAPI.Models.Order.Order
                {
                    CustomerId = customerId,
                    Subtotal = subtotal,
                    ShippingCharge = shippingCharge,
                    Tax = tax,
                    TotalAmount = totalAmount,
                    OrderStatus = "Pending",
                    CreatedBy = customerId
                };


            // 8. Create Order Items
  
            var orderItems = cartItems
                .Select(item =>
                    new EcommerceAPI.Models.Order.OrderItem
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        ItemSubtotal = item.ItemSubtotal,
                        CreatedBy = customerId
                    })
                .ToList();


        
            // 9. Create Shipping Snapshot

            var shipping =
                new EcommerceAPI.Models.Order.OrderShipping
                {
                    ShippingName = customerName,

                    ShippingAddress =
                        string.IsNullOrWhiteSpace(
                            address.AddressLine2)
                        ? address.AddressLine1
                        : $"{address.AddressLine1}, {address.AddressLine2}",

                    ShippingCity = address.City,

                    ShippingState = address.State,

                    ShippingPostalCode = address.PostalCode,

                    ShippingMethod = request.ShippingMethod,

                    ShippingCharge = shippingCharge,

                    CreatedBy = customerId
                };



            // 10. Create Complete Order

            var orderId =
                await _orderService.CreateCompleteOrderAsync(
                    order,
                    orderItems,
                    shipping,
                    cancellationToken);


        
            // 11. Return Checkout Response
       
            return new CheckoutResponseDto
            {
                OrderId = orderId,
                Subtotal = subtotal,
                ShippingCharge = shippingCharge,
                Tax = tax,
                TotalAmount = totalAmount,
                PriceChanges = priceChanges,
                StockIssues = stockIssues
            };
        }
    }
}