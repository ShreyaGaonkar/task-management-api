BEGIN TRANSACTION;
GO

ALTER TABLE [UserTeams] DROP CONSTRAINT [FK_UserTeams_Projects_ProjectId];
GO

DROP INDEX [IX_UserTeams_ProjectId] ON [UserTeams];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserTeams]') AND [c].[name] = N'ProjectId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [UserTeams] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [UserTeams] DROP COLUMN [ProjectId];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240713110141_Migration_1.0', N'6.0.0');
GO

COMMIT;
GO

