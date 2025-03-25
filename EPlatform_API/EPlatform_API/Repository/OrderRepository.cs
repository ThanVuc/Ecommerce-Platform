using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.DTOs.ProductDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Models.ShopOwners;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using StackExchange.Redis;

namespace EPlatform_API.Repository
{
    public class OrderRepository : RepositoryBase<Models.ShopOwners.Order>
    {
        private readonly AppDbContext _context;
        private readonly IDatabase _redis;
        public OrderRepository(AppDbContext context, IConfiguration configuration, ILoggingService loggingService) : base(context, configuration, loggingService)
        {
            _context = context;
            _redis = RedisManager.Connection.GetDatabase();
        }

        public async Task InitStatus(){
            _context.OrderStatuses.RemoveRange(_context.OrderStatuses);
            await _context.OrderStatuses.AddRangeAsync(new OrderStatus[]{
                new OrderStatus{StatusName = "Preparing", Description = "The order is being prepared by the shop owner", IsFinal = false},
                new OrderStatus{StatusName = "Delivering", Description = "The order is being delivered to the customer", IsFinal = false},
                new OrderStatus{StatusName = "Completed", Description = "The order has been completed", IsFinal = true},
                new OrderStatus{StatusName = "Cancelled", Description = "The order has been cancelled", IsFinal = true}
            });
            await _context.SaveChangesAsync();
        }

        public async Task CreateOrder(CreateOrdersRequest customerInfo, KeyValuePair<string, List<CartItemOfOrder>> shopProductsPair, string customerId)
        {
            try {
                if (_context.OrderStatuses == null){
                    throw new Exception("OrderStatuses not found");
                }

                if (_context.OrderStatuses.Count() == 0){
                    await InitStatus();
                }

                var preparingStatus = _context.OrderStatuses.FirstOrDefault(s => s.StatusName == "Preparing");
                
                if (preparingStatus == null){
                    throw new Exception("Preparing status not found");
                }

                Console.WriteLine(shopProductsPair.ToJson());

                _context.Orders.Add(new Models.ShopOwners.Order
                {
                    ShopId = shopProductsPair.Key,
                    CustomerId = customerId,
                    OrderStatusId = preparingStatus.OrderStatusId,
                    ExpectedDeliveryDate = DateTime.Now.AddDays(4),
                    isDeleted = false,
                    PaymentMethod = "Cash",
                    ShippingAddress = customerInfo.ShippingAddress,
                    Email = customerInfo.Email,
                    Phone = customerInfo.Phone,
                    TotalAmount = shopProductsPair.Value.Sum(p => p.Price * p.Quantity),
                    ShipmentCost = 0,
                    CreatedAt = DateTime.Now,
                    OrderProducts = shopProductsPair.Value.Select(p => new OrderProduct
                    {
                        ProductId = p.ProductId,
                        Quantity = p.Quantity,
                        ProductsPrice = p.Price,
                        SpecInfo = p.SpecInfo
                    }).ToList()
                });
            } catch(Exception ex) {
                throw new Exception($"Create order failed in order repo: {ex.Message}");
            }
        }
    
        public async Task<IQueryable<Models.ShopOwners.Order>> GetOrderByShop(string shopId, int? orderStatusId, string? searchString){
            try {
                IQueryable<Models.ShopOwners.Order>? orders = null;
                
                if (orderStatusId != null && string.IsNullOrEmpty(searchString)){
                    orders = _context.Orders
                    .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                    .Include(o => o.OrderStatus)
                    .Include(o => o.Customer)
                    .Where(o => o.ShopId == shopId && o.OrderStatusId == orderStatusId && o.isDeleted == false)
                    .AsNoTracking()
                    .AsQueryable();

                } else {
                    orders = _context.Orders
                    .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                    .Include(o => o.OrderStatus)
                    .Include(o => o.Customer)
                    .Where(o => o.ShopId == shopId && o.isDeleted == false && o.OrderStatus.IsFinal == false)
                    .AsNoTracking()
                    .AsQueryable();
                    
                    if (!string.IsNullOrEmpty(searchString)){
                        // find by order id before find by product name
                        var ordersTemp = orders.Where(o => o.OrderId.ToString().Contains(searchString.Trim('#')));
                        if (ordersTemp.Count() > 0){
                            orders = ordersTemp;
                        } else {
                            orders = orders.Where(o => o.OrderProducts.Any(op => op.Product.Name.Contains(searchString)));
                        }
                    }

                }

                // Take care of with this code it can crack the app if one of the orders is null
                // i think it null because of the shopId is null
                if (orders == null){
                    throw new Exception("Orders not found");
                }

                return orders;
            } catch(Exception ex) {
                throw new Exception($"Get order by shop failed in order repo: {ex.Message}");
            }   
        }
    
        public List<object> GetAllOrderStatus(){
            try {
                return _context.OrderStatuses.Select(s => new {
                    s.OrderStatusId,
                    s.StatusName
                }).ToList<object>();
            } catch(Exception ex) {
                throw new Exception($"Get all order status failed in order repo: {ex.Message}");
            }
        }
    }
}