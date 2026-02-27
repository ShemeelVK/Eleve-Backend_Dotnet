using AutoMapper;
using Eleve_Backend.Application.DTOs;
using Eleve_Backend.Application.DTOs.Orders;
using Eleve_Backend.Application.DTOs.Payment;
using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Domain.Enums;
using Eleve_Backend.Domain.ValueObjects;
using Eleve_Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using Razorpay.Api.Errors;
using AutoMapper.QueryableExtensions;
using System.Linq;
using System.Threading.Tasks;
using Order = Eleve_Backend.Domain.Entities.Order;

namespace Eleve_Backend.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly EleveDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(EleveDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<string> PlaceOrderAsync(int userId,CreateOrderDto dto)
        {
            var Random=new Random();
            string friendlyId = $"Eleve - {DateTime.UtcNow:yyMMdd} - {Random.Next(1000, 9999)}";
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderReference = friendlyId,  //for generating to the DB
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                PaymentMethod= dto.PaymentMethod,
                TransactionId=dto.TransactionId,

                //Map the address Dto to domain Value object
                ShippingAddress = new Address
                (
                    dto.ShippingAddress.Name,
                    dto.ShippingAddress.Street,
                    dto.ShippingAddress.City,
                    dto.ShippingAddress.State,
                    dto.ShippingAddress.ZipCode,
                    dto.ShippingAddress.PhoneNumber
                )
            };

            decimal calculatedTotal = 0;

            foreach(var itemDto in dto.Items)
            {
                //fetch real product from db to get the price
                var product = await _context.Products.FindAsync(itemDto.ProductId);

                if (product == null)
                {
                    throw new Exception($"Product with ID {itemDto.ProductId} not found");
                }

                if (product.Stock < itemDto.Quantity)
                {
                    throw new Exception($"Not enough stock for {product.Name}. Only {product.Stock} left");
                }

                product.Stock-=itemDto.Quantity;


                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = itemDto.Quantity
                };

                //Adding to order
                order.Items.Add(orderItem);

                //Add to Total
                calculatedTotal += (product.Price * itemDto.Quantity);

            }

            order.TotalAmount = calculatedTotal;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order.OrderReference;
        }


        public async Task<bool> UpdateOrderStatusAsync(Guid orderId,OrderStatus newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.Items) //if we need to restore the stock
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;
            
            if(order.Status==OrderStatus.Cancelled || order.Status==OrderStatus.Delivered)
                return false;

            bool isValidTransition = false;

            switch (order.Status)
            {
                case OrderStatus.Pending:
                    if (newStatus == OrderStatus.Shipped || newStatus == OrderStatus.Cancelled)
                        isValidTransition = true;
                    break;

                case OrderStatus.Shipped:
                    if(newStatus==OrderStatus.Delivered)
                        isValidTransition = true;
                    break;
            }

            if (!isValidTransition)
            {
                throw new InvalidOperationException($"Cannot change status from {order.Status} to {newStatus}.");
            }
                

            if(newStatus == OrderStatus.Cancelled)
            {
                foreach (var item in order.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if(product != null)
                    {
                        product.Stock += item.Quantity;
                    }
                }
            }

            order.Status = newStatus;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<OrderResponseDto>> GetOrdersByUserIdAsync(int userId)
        {
                 return await _context.Orders
                    .AsNoTracking()
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    // .AsSplitQuery() tells EF to fetch items in a separate, simple SQL call
                    // This prevents the complex nested JOIN that is taking 25 seconds.
                    .AsSplitQuery() 
                    .Select(o => new OrderResponseDto
                    {
                        Id = o.Id,
                        OrderReference = o.OrderReference,
                        OrderDate = o.OrderDate,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status.ToString(),
                        PaymentMethod = o.PaymentMethod,
                        Username = o.ShippingAddress.Name,
                        ShippingAddress = new AddressDto
                        {
                            Name = o.ShippingAddress.Name,
                            Street = o.ShippingAddress.Street,
                            City = o.ShippingAddress.City,
                            State = o.ShippingAddress.State,
                            ZipCode = o.ShippingAddress.ZipCode,
                            PhoneNumber = o.ShippingAddress.PhoneNumber
                        },
                        Items = o.Items.Select(i => new OrderItemResponseDto
                        {
                            ProductId = i.ProductId,
                            ProductName = i.ProductName,
                            UnitPrice = i.UnitPrice,
                            Quantity = i.Quantity,
                            ProductImage = i.Product.ImageUrl
                        }).ToList()
                    })
                        .ToListAsync();
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders=await _context.Orders
                .Include(s=>s.Items)
                .ThenInclude(i=>i.Product)
                .OrderByDescending(o=>o.OrderDate)
                .ToListAsync();

            //converting dto
            return _mapper.Map<List<OrderResponseDto>>(orders);

        }

        public async Task<bool> CancelOrderAsync(Guid orderId,int userId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if(order==null || order.UserId!=userId || order.Status != OrderStatus.Pending)
            {
                return false;
            }

            order.Status = OrderStatus.Cancelled;

            await _context.SaveChangesAsync();
            return true;
        }

        public bool VerifyPayment(PaymentVerificationDto dto)
        {
            string SecretKey = "Test-key";
            Dictionary<string, string> attributes = new Dictionary<string, string>
            {
                {"razorpay_order_id",dto.RazorpayOrderId },
                {"razorpay_payment_id",dto.RazorpayPaymentId },
                {"razorpay_signature",dto.RazorpaySignature }
            };

            try
            {
                Utils.verifyPaymentSignature(attributes);

                return true; //Payment successfull
            }
            catch (SignatureVerificationError)
            {
                return false;
            }
        }

        public async Task<bool> ReturnOrderAsync(Guid orderId, int userId)
        {
            var order= await _context.Orders
                .Include(o=> o.Items)
                .FirstOrDefaultAsync(o=> o.Id==orderId);

            if(order==null || order.UserId!=userId || order.Status != OrderStatus.Delivered)
            {
                return false;
            }

            foreach(var item in order.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                }
            }

            order.Status = OrderStatus.Returned;

            await _context.SaveChangesAsync();
            return true;
        }

        ////Helper method to map the code
        //private List<OrderResponseDto> MapToDto(List<Order> orders)
        //{
        //    return orders.Select(order => new OrderResponseDto
        //    {
        //        Id = order.Id,
        //        OrderDate = order.OrderDate,
        //        TotalAmount = order.TotalAmount,
        //        Status = order.Status.ToString(), //convert enum to string

        //        ShippingAddress = new AddressDto
        //        {
        //            Street = order.ShippingAddress.Street,
        //            City = order.ShippingAddress.City,
        //            State = order.ShippingAddress.State,
        //            ZipCode = order.ShippingAddress.ZipCode
        //        },

        //        Items = order.Items.Select(item => new OrderItemResponseDto
        //        {
        //            ProductId = item.ProductId,
        //            ProductName = item.ProductName,
        //            UnitPrice = item.UnitPrice,
        //            Quantity = item.Quantity,
        //        }).ToList()


        //    }).ToList();
        //}


    }
}
