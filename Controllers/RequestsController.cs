using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrsNetWeb.Models;

namespace PrsNetWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly PRSContext _context;

        public RequestsController(PRSContext context)
        {
            _context = context;
        }

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {

            var request = _context.Requests.Include(r => r.User);
                                           
            return await request.ToListAsync();
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id)
        {

            var request = await _context.Requests.Include(r => r.User)
                                                
                                                 .FirstOrDefaultAsync(r => r.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return request;
        }

        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(id))
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

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request)
        {
            
            request.RequestNumber = GetRequestNumber();
            request.Status = "NEW";
            request.Total = 0.0m;
            request.SubmittedDate = DateTime.Now;
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private string GetRequestNumber() {
            string requestNbr = "R";
            // add YYMMDD string
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            requestNbr += today.ToString("yyMMdd");
            // get maximum request number from db
            string maxReqNbr = _context.Requests.Max(r => r.RequestNumber);
            String reqNbr = "";
            if (maxReqNbr != null) {
                // get last 4 characters, convert to number
                String tempNbr = maxReqNbr.Substring(7);
                int nbr = Int32.Parse(tempNbr);
                nbr++;
                // pad w/ leading zeros
                reqNbr += nbr;
                reqNbr = reqNbr.PadLeft(4, '0');
            }
            else {
                reqNbr = "0001";
            }
            requestNbr += reqNbr;
            return requestNbr;

        }
    
      
     
        // GET: api/Requests/submit-review/id
        [HttpPut("submit-review/{id}")]
        public async Task<IActionResult> PutRequestForReview(int id, Request request)
        {
            request.Status = "REVIEW";
            if (id != request.Id) {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!RequestExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }
        // GET: api/Requests/approve/id
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> PutRequestForApproved(int id, Request request)
        {
            request.Status = "APPOVED";
            if (id != request.Id) {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!RequestExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // GET: api/Requests/approve/id
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> PutRequestForRejected(int id, Request request)
        {
            if (id != request.Id) {
                return BadRequest();
            }
           
                request.Status = "REJECTED";
            
            _context.Entry(request).State = EntityState.Modified;
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!RequestExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // GET: api/Requests/list-review/id
        [HttpGet("list-review/{id}")]
        public async Task<ActionResult<List<Request>>> GetRequestsForReview(int id)
        {
            
            
            var request = await _context.Requests.Where(r => r.Status == "REVIEW" && r.UserId != id)
                                                 .ToListAsync();
            if (request == null) {
                return NotFound();
            }

            return request;
     
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
