using Microsoft.AspNetCore.Mvc;
using Quantum_Count.DTOs;
using Quantum_Count.Services;
using System;
using System.Threading.Tasks;

namespace Quantum_Count.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        // ✅ SWAP: Change this from GeminiAiService to your Orchestrator
        private readonly InventoryAiOrchestrator _orchestrator;

        public AIController(InventoryAiOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AIRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest(new
                {
                    message = "Question cannot be empty."
                });
            }

            try
            {
                // ✅ SWAP: Call the dynamic database routing method instead of raw AI service
                var answer = await _orchestrator.GetPersonalizedInventoryAnalysisAsync(request.Question);

                return Ok(new AIResponse
                {
                    Answer = answer
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while processing the personalized inventory query.",
                    error = ex.Message
                });
            }
        }
    }
}
