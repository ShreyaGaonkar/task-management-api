using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost("tasks")]
        public async Task<IActionResult> GetTasksReport([FromBody] TaskReportDTO taskReportDTO)
        {
            byte[] excelFileContents = await _reportService.GenerateTasksReport(taskReportDTO);

            return File(excelFileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TeamTasksReport.xlsx");
        }

    }
}
