using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data
{
    public class TaskManagementDbContext : DbContext
    {
        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<UserTeam> UserTeams { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<TaskDocument> TaskDocuments { get; set; }
        public DbSet<TaskNote> TaskNotes { get; set; }
        public DbSet<TeamProject> TeamProjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.AssignedToUser)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.AssignedByUser)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserTeam>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTeams)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<UserTeam>()
                .HasOne(ut => ut.Team)
                .WithMany(t => t.UserTeams)
                .HasForeignKey(ut => ut.TeamId);

            modelBuilder.Entity<TeamProject>()
                .HasOne(tp => tp.Team)
                .WithMany(t => t.TeamProjects)
                .HasForeignKey(tp => tp.TeamId);

            modelBuilder.Entity<TeamProject>()
                .HasOne(tp => tp.Project)
                .WithMany(p => p.TeamProjects)
                .HasForeignKey(tp => tp.ProjectId);

            modelBuilder.Entity<TaskDocument>()
                .HasOne(td => td.Task)
                .WithMany(t => t.TaskDocuments)
                .HasForeignKey(td => td.TaskId);

            modelBuilder.Entity<TaskDocument>()
                .HasOne(td => td.UploadedByUser)
                .WithMany()
                .HasForeignKey(td => td.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskNote>()
                .HasOne(tn => tn.Task)
                .WithMany(t => t.TaskNotes)
                .HasForeignKey(tn => tn.TaskId);

            modelBuilder.Entity<TaskNote>()
                .HasOne(tn => tn.AddedByUser)
                .WithMany()
                .HasForeignKey(tn => tn.AddedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            base.OnModelCreating(modelBuilder);
        }
    }

}
