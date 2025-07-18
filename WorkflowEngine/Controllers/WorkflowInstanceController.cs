using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Models;
using WorkflowEngine.Services;

namespace WorkflowEngine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowInstanceController : ControllerBase
    {
        private readonly IWorkflowInstanceService _instanceService;

        public WorkflowInstanceController(IWorkflowInstanceService instanceService)
        {
            _instanceService = instanceService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkflowInstance>> StartInstance([FromQuery] string definitionId)
        {
            try
            {
                var instance = await _instanceService.StartInstanceAsync(definitionId);
                return CreatedAtAction(nameof(GetInstance), new { id = instance.Id }, instance);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Workflow definition with ID {definitionId} not found");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkflowInstance>> GetInstance(string id)
        {
            try
            {
                var instance = await _instanceService.GetInstanceAsync(id);
                return Ok(instance);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WorkflowInstance>>> GetAllInstances()
        {
            var instances = await _instanceService.GetAllInstancesAsync();
            return Ok(instances);
        }

        [HttpPost("{id}/execute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkflowInstance>> ExecuteAction(string id, [FromQuery] string actionId)
        {
            try
            {
                var instance = await _instanceService.ExecuteActionAsync(id, actionId);
                return Ok(instance);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
