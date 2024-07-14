IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Projects] (
    [Id] int NOT NULL IDENTITY,
    [ProjectName] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [StartDate] datetime2 NULL,
    [EndDate] datetime2 NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] int NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [RoleName] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Priority] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] int NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] int NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Teams] (
    [Id] int NOT NULL IDENTITY,
    [TeamName] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] int NULL,
    CONSTRAINT [PK_Teams] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TeamProjects] (
    [Id] int NOT NULL IDENTITY,
    [TeamId] int NOT NULL,
    [ProjectId] int NOT NULL,
    [AssignedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_TeamProjects] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeamProjects_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeamProjects_Teams_TeamId] FOREIGN KEY ([TeamId]) REFERENCES [Teams] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Tasks] (
    [Id] int NOT NULL IDENTITY,
    [ProjectId] int NOT NULL,
    [TaskName] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [AssignedToUserId] int NOT NULL,
    [AssignedByUserId] int NOT NULL,
    [DueDate] datetime2 NULL,
    [Status] nvarchar(max) NOT NULL,
    [Priority] nvarchar(max) NOT NULL,
    [Estimate] int NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] int NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] int NULL,
    CONSTRAINT [PK_Tasks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Tasks_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Tasks_Users_AssignedByUserId] FOREIGN KEY ([AssignedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Tasks_Users_AssignedToUserId] FOREIGN KEY ([AssignedToUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [UserRoles] (
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    [AssignedDate] datetime2 NOT NULL,
    [Id] int NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserTeams] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [TeamId] int NOT NULL,
    [AssignedDate] datetime2 NOT NULL,
    [ProjectId] int NULL,
    CONSTRAINT [PK_UserTeams] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserTeams_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id]),
    CONSTRAINT [FK_UserTeams_Teams_TeamId] FOREIGN KEY ([TeamId]) REFERENCES [Teams] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserTeams_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [TaskDocuments] (
    [Id] int NOT NULL IDENTITY,
    [TaskId] int NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Path] nvarchar(max) NOT NULL,
    [UploadedByUserId] int NOT NULL,
    [UploadedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_TaskDocuments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TaskDocuments_Tasks_TaskId] FOREIGN KEY ([TaskId]) REFERENCES [Tasks] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TaskDocuments_Users_UploadedByUserId] FOREIGN KEY ([UploadedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TaskNotes] (
    [Id] int NOT NULL IDENTITY,
    [TaskId] int NOT NULL,
    [Note] nvarchar(max) NOT NULL,
    [AddedByUserId] int NOT NULL,
    [AddedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_TaskNotes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TaskNotes_Tasks_TaskId] FOREIGN KEY ([TaskId]) REFERENCES [Tasks] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TaskNotes_Users_AddedByUserId] FOREIGN KEY ([AddedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_TaskDocuments_TaskId] ON [TaskDocuments] ([TaskId]);
GO

CREATE INDEX [IX_TaskDocuments_UploadedByUserId] ON [TaskDocuments] ([UploadedByUserId]);
GO

CREATE INDEX [IX_TaskNotes_AddedByUserId] ON [TaskNotes] ([AddedByUserId]);
GO

CREATE INDEX [IX_TaskNotes_TaskId] ON [TaskNotes] ([TaskId]);
GO

CREATE INDEX [IX_Tasks_AssignedByUserId] ON [Tasks] ([AssignedByUserId]);
GO

CREATE INDEX [IX_Tasks_AssignedToUserId] ON [Tasks] ([AssignedToUserId]);
GO

CREATE INDEX [IX_Tasks_ProjectId] ON [Tasks] ([ProjectId]);
GO

CREATE INDEX [IX_TeamProjects_ProjectId] ON [TeamProjects] ([ProjectId]);
GO

CREATE INDEX [IX_TeamProjects_TeamId] ON [TeamProjects] ([TeamId]);
GO

CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
GO

CREATE UNIQUE INDEX [IX_UserRoles_UserId] ON [UserRoles] ([UserId]);
GO

CREATE INDEX [IX_UserTeams_ProjectId] ON [UserTeams] ([ProjectId]);
GO

CREATE INDEX [IX_UserTeams_TeamId] ON [UserTeams] ([TeamId]);
GO

CREATE INDEX [IX_UserTeams_UserId] ON [UserTeams] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240713094944_Initial_Migration', N'6.0.0');
GO

COMMIT;
GO

