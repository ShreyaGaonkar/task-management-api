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
    [Username] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] int NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Tasks] (
    [Id] int NOT NULL IDENTITY,
    [ProjectID] int NOT NULL,
    [TaskName] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [AssignedToUserId] int NOT NULL,
    [AssignedByUserId] int NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Priority] nvarchar(max) NOT NULL,
    [Estimate] int NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] int NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] int NULL,
    [UserId] int NULL,
    [UserId1] int NULL,
    CONSTRAINT [PK_Tasks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Tasks_Projects_ProjectID] FOREIGN KEY ([ProjectID]) REFERENCES [Projects] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Tasks_Users_AssignedByUserId] FOREIGN KEY ([AssignedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Tasks_Users_AssignedToUserId] FOREIGN KEY ([AssignedToUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Tasks_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Tasks_Users_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [Users] ([Id])
);
GO

CREATE TABLE [UserRole] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    [AssignedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserRole_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRole_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserTeams] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [TeamId] int NOT NULL,
    [ProjectId] int NOT NULL,
    [AssignedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_UserTeams] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserTeams_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserTeams_Teams_TeamId] FOREIGN KEY ([TeamId]) REFERENCES [Teams] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserTeams_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Tasks_AssignedByUserId] ON [Tasks] ([AssignedByUserId]);
GO

CREATE INDEX [IX_Tasks_AssignedToUserId] ON [Tasks] ([AssignedToUserId]);
GO

CREATE INDEX [IX_Tasks_ProjectID] ON [Tasks] ([ProjectID]);
GO

CREATE INDEX [IX_Tasks_UserId] ON [Tasks] ([UserId]);
GO

CREATE INDEX [IX_Tasks_UserId1] ON [Tasks] ([UserId1]);
GO

CREATE INDEX [IX_UserRole_RoleId] ON [UserRole] ([RoleId]);
GO

CREATE INDEX [IX_UserRole_UserId] ON [UserRole] ([UserId]);
GO

CREATE INDEX [IX_UserTeams_ProjectId] ON [UserTeams] ([ProjectId]);
GO

CREATE INDEX [IX_UserTeams_TeamId] ON [UserTeams] ([TeamId]);
GO

CREATE INDEX [IX_UserTeams_UserId] ON [UserTeams] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240712153006_Initial_Migration', N'6.0.0');
GO

COMMIT;
GO

