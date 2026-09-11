using Microsoft.AspNetCore.Mvc;
using Quantum_Count.Models;
using Quantum_Count.Services;
namespace Quantum_Count.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly InventoryService _inventoryService;
    public ProjectsController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _inventoryService.GetProjectsAsync();
        return Ok(projects);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProject(int id)
    {
        var project = await _inventoryService.GetProjectAsync(id);
        if (project == null)
            return NotFound(new { message = "Project not found." });
        return Ok(project);
    }
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] Project project)
    {
        var result = await _inventoryService.CreateProjectAsync(project);
        if (!result.success)
            return BadRequest(new { message = result.message });
        return CreatedAtAction(nameof(GetProject),new { id = result.project!.Id },result.project);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProject(int id,[FromBody] Project project)
    {
        if (id != project.Id)
            return BadRequest(new { message = "Project ID mismatch." });
        var result = await _inventoryService.UpdateProjectAsync(project);
        if (!result.success)
            return BadRequest(new { message = result.message });
        return Ok(result.project);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var result = await _inventoryService.DeleteProjectAsync(id);
        if (!result.success)
            return NotFound(new { message = result.message });
        return Ok(new { message = result.message });
    }
    [HttpGet("{projectId:int}/materials")]
    public async Task<IActionResult> GetProjectMaterials(int projectId)
    {
        var materials = await _inventoryService.GetProjectMaterialsAsync(projectId);
        return Ok(materials);
    }
    [HttpPost("{projectId:int}/materials")]
    public async Task<IActionResult> AddProjectMaterial(int projectId, [FromBody] ProjectMaterial material)
    {
        material.ProjectId = projectId;
        var result = await _inventoryService.AddProjectMaterialAsync(material);
        if (!result.success)
            return BadRequest(new { message = result.message });
        return Ok(result.projectMaterial);
    }
    [HttpPost("{projectId:int}/materials/{projectMaterialId:int}/return")]
    public async Task<IActionResult> ReturnProjectMaterial( int projectId, int projectMaterialId, [FromBody] decimal quantity)
    {
        var result = await _inventoryService.ReturnProjectMaterialAsync( projectMaterialId, quantity);
        if (!result.success)
            return BadRequest(new { message = result.message });
        return Ok(new { message = result.message });
    }
    [HttpDelete("{projectId:int}/materials/{projectMaterialId:int}")]
    public async Task<IActionResult> RemoveProjectMaterial(int projectId,int projectMaterialId)
    {
        var result = await _inventoryService.RemoveProjectMaterialAsync( projectMaterialId);
        if (!result.success)
            return BadRequest(new { message = result.message });
        return Ok(new { message = result.message });
    }
    [HttpGet("{projectId:int}/equipment")]
    public async Task<IActionResult> GetProjectEquipment(int projectId)
    {
        var equipment = await _inventoryService.GetProjectEquipmentAsync(projectId);
        return Ok(equipment);
    }
    [HttpPost("{projectId:int}/equipment")]
    public async Task<IActionResult> AssignEquipment( int projectId, [FromBody] ProjectEquipment equipment)
    {
        equipment.ProjectId = projectId;
        var result = await _inventoryService.AssignEquipmentToProjectAsync(equipment);
        if (!result.success)
            return BadRequest(new { message = result.message });
        return Ok(result.projectEquipment);
    }
    [HttpDelete("{projectId:int}/equipment/{projectEquipmentId:int}")]
    public async Task<IActionResult> ReturnEquipment(int projectId,int projectEquipmentId)
    {
        var result = await _inventoryService.ReturnEquipmentFromProjectAsync( projectEquipmentId);
        if (!result.success)
           return BadRequest(new { message = result.message });
        return Ok(new { message = result.message });
    }
    [HttpGet("{projectId:int}/tasks")]
    public async Task<IActionResult> GetProjectTasks(int projectId)
    {
        var tasks = await _inventoryService.GetProjectTasksAsync(projectId);
        return Ok(tasks);
    }
    [HttpPost("{projectId:int}/tasks")]
    public async Task<IActionResult> CreateProjectTask(int projectId,[FromBody] ProjectTask task)
    {
        task.ProjectId = projectId;
        var result = await _inventoryService.CreateProjectTaskAsync(task);
        if (!result.success)
            return BadRequest(new { message = result.message });
        return Ok(result.task);
    }
    [HttpPut("{projectId:int}/tasks/{taskId:int}")]
    public async Task<IActionResult> UpdateProjectTask(int projectId, int taskId, [FromBody] ProjectTask task)
    {
        if (taskId != task.Id)
            return BadRequest(new { message = "Task ID mismatch." });
        task.ProjectId = projectId;
        var result = await _inventoryService.UpdateProjectTaskAsync(task);
        if (!result.success)
            return BadRequest(new { message = result.message });
        return Ok(result.task);
    }
    [HttpGet("{projectId:int}/progress")]
    public async Task<IActionResult> GetProjectProgress(int projectId)
    {
        var progress = await _inventoryService.GetTaskProgressAsync(projectId);
        return Ok(new
        {
            projectId,
            progress
        });
    }
    [HttpGet("{projectId:int}/dashboard")]
    public async Task<IActionResult> GetProjectDashboard(int projectId)
    {
        var dashboard = await _inventoryService.GetProjectDashboardAsync(projectId);
        if (dashboard == null)
            return NotFound(new { message = "Project not found." });
        return Ok(dashboard);
    }
}