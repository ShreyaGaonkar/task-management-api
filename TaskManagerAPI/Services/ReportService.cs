using OfficeOpenXml;
using TaskManagerAPI.Data.Repositories;
using TaskManagerAPI.DTO.Request;

namespace TaskManagerAPI.Services
{
    public interface IReportService
    {
        Task<byte[]> GenerateTasksReport(TaskReportDTO taskReportDTO);
    }

    public class ReportService : IReportService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITeamRepository _teamRepository;

        public ReportService(ITaskRepository taskRepository, ITeamRepository teamRepository)
        {
            _taskRepository = taskRepository;
            _teamRepository = teamRepository;
        }

        public async Task<byte[]> GenerateTasksReport(TaskReportDTO taskReportDTO)
        {
            byte[] excelFileContents;
            using (var package = new ExcelPackage())
            {
                var tasks = await _taskRepository.GetTasksAsync(taskReportDTO);
                var teams = tasks.Select(x => x.Project).SelectMany(x => x.TeamProjects).Select(y => y.Team).Distinct().ToList();

                foreach (var team in teams)
                {
                    var teamTasks = tasks.Where(t => t.Project.TeamProjects.Any(tp => tp.Team.TeamName == team.TeamName)).ToList();

                    var worksheet = package.Workbook.Worksheets.Add(team.TeamName + " Tasks");

                    worksheet.Cells["A1"].Value = "Task ID";
                    worksheet.Cells["B1"].Value = "Project Name";
                    worksheet.Cells["C1"].Value = "Task Name";
                    worksheet.Cells["D1"].Value = "Assigned To";
                    worksheet.Cells["E1"].Value = "Status";
                    worksheet.Cells["F1"].Value = "Priority";
                    worksheet.Cells["G1"].Value = "Completed Date";
                    worksheet.Cells["H1"].Value = "Created Date";

                    using (var range = worksheet.Cells["A1:H1"])
                    {
                        range.Style.Font.Bold = true;
                    }

                    int row = 2;
                    foreach (var task in teamTasks)
                    {
                        worksheet.Cells[row, 1].Value = task.Id;
                        worksheet.Cells[row, 2].Value = task.Project.ProjectName;
                        worksheet.Cells[row, 3].Value = task.TaskName;
                        worksheet.Cells[row, 4].Value = task.AssignedToUser.FirstName + " " + task.AssignedToUser.LastName;
                        worksheet.Cells[row, 5].Value = task.Status;
                        worksheet.Cells[row, 6].Value = task.Priority;
                        worksheet.Cells[row, 7].Value = task.CompletedDate?.ToString("yyyy-MM-dd");
                        worksheet.Cells[row, 8].Value = task.CreatedDate.ToString("yyyy-MM-dd");
                        row++;
                    }

                    worksheet.Cells.AutoFitColumns();
                }

                var summarySheet = package.Workbook.Worksheets.Add("Summary");
                summarySheet.Cells["A1"].Value = "Team Name";
                summarySheet.Cells["B1"].Value = "Total Tasks";
                summarySheet.Cells["C1"].Value = "Tasks Completed";
                summarySheet.Cells["D1"].Value = "Tasks In Progress";
                summarySheet.Cells["E1"].Value = "Tasks ToDo";

                using (var range = summarySheet.Cells["A1:E1"])
                {
                    range.Style.Font.Bold = true;
                }

                int summaryRow = 2;
                foreach (var team in teams)
                {
                    var teamTasks = tasks.Where(t => t.Project.TeamProjects.Any(tp => tp.Team.TeamName == team.TeamName)).ToList();

                    int totalTasks = teamTasks.Count();
                    int tasksCompleted = teamTasks.Count(t => t.Status == "Done");
                    int tasksInProgress = teamTasks.Count(t => t.Status == "InProgress");
                    int tasksToDo = teamTasks.Count(t => t.Status == "ToDo");

                    summarySheet.Cells[summaryRow, 1].Value = team.TeamName;
                    summarySheet.Cells[summaryRow, 2].Value = totalTasks;
                    summarySheet.Cells[summaryRow, 3].Value = tasksCompleted;
                    summarySheet.Cells[summaryRow, 4].Value = tasksInProgress;
                    summarySheet.Cells[summaryRow, 5].Value = tasksToDo;
                    summaryRow++;
                }

                summarySheet.Cells.AutoFitColumns();

                excelFileContents = package.GetAsByteArray();
            }

            return excelFileContents;
        }
    }
}
