﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrsNetWeb.Models;

namespace PrsNetWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineItemsController : ControllerBase
    {
        private readonly PRSContext _context;

        public LineItemsController(PRSContext context)
        {
            _context = context;
        }

        // GET: api/LineItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LineItem>>> GetLineItems()
        {
            var lineItems = _context.LineItems.Include(l => l.Product)
                                               .Include(l => l.Request);
            return await lineItems.ToListAsync();
        }

        // GET: api/LineItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LineItem>> GetLineItem(int id)
        {
            var lineItem = await _context.LineItems.Include(l => l.Product)
                                                   .Include(l => l.Request)
                                                   .FirstOrDefaultAsync(l => l.Id == id);
            if (lineItem == null)
            {
                return NotFound();
            }

            return lineItem;
        }

        // PUT: api/LineItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLineItem(int id, LineItem lineItem)
        {
            if (id != lineItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(lineItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                RecalculateCollectionValue(lineItem.RequestId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LineItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LineItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LineItem>> PostLineItem(LineItem lineItem)
        {
            _context.LineItems.Add(lineItem);
            await _context.SaveChangesAsync();
            RecalculateCollectionValue(lineItem.RequestId);
            return CreatedAtAction("GetLineItem", new { id = lineItem.Id }, lineItem);
        }

        // DELETE: api/LineItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLineItem(int id)
        {
            var lineItem = await _context.LineItems.FindAsync(id);
            if (lineItem == null)
            {
                return NotFound();
            }

            _context.LineItems.Remove(lineItem);
            await _context.SaveChangesAsync();
            RecalculateCollectionValue(lineItem.RequestId);
            return NoContent();
        }
        // GET: api/LineItems/Lines-for-req5
        [HttpGet("lines-for-req/{id}")]
        public async Task<ActionResult<List<LineItem>>> GetLineItemByRequestID(int id)
        {
            var lineItem = await _context.LineItems.Where(l => l.RequestId == id)
                                                   .ToListAsync();
            if (lineItem == null) {
                return NotFound();
            }

            return lineItem;
            //lineitems for request
        }
        private void RecalculateCollectionValue(int reqId)
        {
        
            var request = _context.Requests.Find(reqId);
          
            var l = _context.LineItems.Include(l => l.Product)
                                      .Include(l => l.Request)
                                      .Where(li => li.RequestId == reqId);
            
          
            
            decimal sum = 0;
            foreach (LineItem li in l) {
                sum += li.Quantity * li.Product.Price;
            }
            request.Total = sum;
           
            _context.SaveChanges();
        }

            private bool LineItemExists(int id)
        {
            return _context.LineItems.Any(e => e.Id == id);
        }
    }
}
