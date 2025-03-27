using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.DTOs.OrderDTOs;
using EPlatform_API.DTOs.ProductDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Mappers;
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
                    .Where(o => o.ShopId == shopId && o.isDeleted == false)
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

                orders = orders.OrderByDescending(o => o.CreatedAt);
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

        public async Task<string[]> ChangeOrderStatus(List<UpdateOrderStatusRequest> changeOrderStatuses){
            try {
                // note the order cannot be change
                string[] errorOrders = new string[]{};
                foreach (var orderStatusItem in changeOrderStatuses)
                {
                    var newStatus = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.OrderStatusId == orderStatusItem.StatusId);
                    if (newStatus == null){
                        errorOrders.Append($"Order {orderStatusItem.OrderId} status not found");
                        continue;
                    }

                    var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderStatusItem.OrderId);
                    if (order == null){
                        errorOrders.Append($"Order {orderStatusItem.OrderId} not found");
                        continue;
                    }

                    if (order.OrderStatusId == newStatus.OrderStatusId){
                        continue;
                    }

                    order.OrderStatusId = newStatus.OrderStatusId;
                }
                return errorOrders;
            } catch(Exception ex) {
                throw new Exception($"Change order status failed in order repo: {ex.Message}");
            }
        }
    
        public async Task<OrderDetailResponse> GetOrderById(int orderId){
            try {
                var order = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Include(o => o.OrderStatus)
                .Include(o => o.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null){
                    throw new Exception("Orders not found");
                }

                return order.ToOrderDetailResponse();
            } catch(Exception ex) {
                throw new Exception($"Get order by id failed in order repo: {ex.Message}");
            }
        }
    
        // get purchase orders by customer
        public async Task<List<PurchaseOrdersResponse>> GetPurchaseOrdersByCustomer(string customerId){
            try {
                // get user's orders
                var orders = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Include(o => o.OrderStatus)
                .Include(o => o.Shop)
                .Where(o => o.CustomerId == customerId && o.isDeleted == false)
                .Where(o => !(o.OrderStatus.IsFinal == true && o.CreatedAt < DateTime.Now.AddMonths(-2)))
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => o.ToPurchaseOrdersResponse())
                .ToListAsync();
                
                if (orders == null){
                    throw new Exception("Orders not found");
                }

                return orders;
            } catch(Exception ex) {
                throw new Exception($"Get purchase orders by customer failed in order repo: {ex.Message}");
            }   
        }
    
        public async Task CancelOrder(int orderId){
            try {
                var statusId = (await _context.OrderStatuses.FirstOrDefaultAsync(s => s.StatusName == "Cancelled"))?.OrderStatusId;
                
                if (statusId == null){
                    throw new Exception("Cancelled status not found");
                }

                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null){
                    throw new Exception("Order not found");
                }

                order.OrderStatusId = (int)statusId;
            } catch(Exception ex) {
                throw new Exception($"Cancel order failed in order repo: {ex.Message}");
            }
        }
    }
}