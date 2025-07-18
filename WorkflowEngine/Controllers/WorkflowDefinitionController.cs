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
    public class WorkflowDefinitionController : ControllerBase
    {
        private readonly IWorkflowDefinitionService _definitionService;

        public WorkflowDefinitionController(IWorkflowDefinitionService definitionService)
        {
            _definitionService = definitionService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WorkflowDefinition>> CreateDefinition([FromBody] WorkflowDefinition definition)
        {
            try
            {
                var createdDefinition = await _definitionService.CreateDefinitionAsync(definition);
                return CreatedAtAction(nameof(GetDefinition), new { id = createdDefinition.Id }, createdDefinition);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkflowDefinition>> GetDefinition(string id)
        {
            try
            {
                var definition = await _definitionService.GetDefinitionAsync(id);
                return Ok(definition);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WorkflowDefinition>>> GetAllDefinitions()
        {
            var definitions = await _definitionService.GetAllDefinitionsAsync();
            return Ok(definitions);
        }

        [HttpPost("validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> ValidateDefinition([FromBody] WorkflowDefinition definition)
        {
            var isValid = await _definitionService.ValidateDefinitionAsync(definition);
            if (isValid)
            {
                return Ok(true);
            }
            return BadRequest("Invalid workflow definition");
        }
    }
}
