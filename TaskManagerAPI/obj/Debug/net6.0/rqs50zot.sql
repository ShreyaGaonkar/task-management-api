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
    [ProjectID] int NOT NULL,
    [ProjectName] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [RoleID] int NOT NULL,
    [RoleName] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Priority] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Teams] (
    [Id] int NOT NULL IDENTITY,
    [TeamID] int NOT NULL,
    [TeamName] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Teams] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [Username] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Tasks] (
    [Id] int NOT NULL IDENTITY,
    [TaskID] int NOT NULL,
    [ProjectID] int NOT NULL,
    [TaskName] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [AssignedToUserId] int NOT NULL,
    [AssignedByUserId] int NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Priority] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
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
    [UserRolesID] int NOT NULL,
    [UserID] int NOT NULL,
    [RoleID] int NOT NULL,
    [AssignedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserRole_Roles_RoleID] FOREIGN KEY ([RoleID]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRole_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserTeams] (
    [Id] int NOT NULL IDENTITY,
    [UserTeamID] int NOT NULL,
    [UserID] int NOT NULL,
    [TeamID] int NOT NULL,
    [ProjectID] int NOT NULL,
    [AssignedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_UserTeams] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserTeams_Projects_ProjectID] FOREIGN KEY ([ProjectID]) REFERENCES [Projects] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserTeams_Teams_TeamID] FOREIGN KEY ([TeamID]) REFERENCES [Teams] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserTeams_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
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

CREATE INDEX [IX_UserRole_RoleID] ON [UserRole] ([RoleID]);
GO

CREATE INDEX [IX_UserRole_UserID] ON [UserRole] ([UserID]);
GO

CREATE INDEX [IX_UserTeams_ProjectID] ON [UserTeams] ([ProjectID]);
GO

CREATE INDEX [IX_UserTeams_TeamID] ON [UserTeams] ([TeamID]);
GO

CREATE INDEX [IX_UserTeams_UserID] ON [UserTeams] ([UserID]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240712150923_Initial_Migration', N'6.0.0');
GO

COMMIT;
GO

