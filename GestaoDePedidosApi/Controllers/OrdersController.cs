using GestaoDesPedidosApi.Data;
using GestaoDesPedidosApi.Models;
using GestaoDesPedidosApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoDesPedidosApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly ServiceBusService _serviceBusService;

        public OrdersController(OrderDbContext context, ServiceBusService serviceBusService)
        {
            _context = context;
            _serviceBusService = serviceBusService;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Initialize order properties
            order.Id = Guid.NewGuid();
            order.Status = "Pendente";
            order.DataCriacao = DateTime.Now;

            // Save order to database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Send message to Azure Service Bus
            await _serviceBusService.SendOrderMessageAsync(order);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
    }
}
