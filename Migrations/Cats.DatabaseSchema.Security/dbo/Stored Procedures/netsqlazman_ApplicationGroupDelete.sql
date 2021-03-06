﻿CREATE PROCEDURE [dbo].[netsqlazman_ApplicationGroupDelete]
(
	@ApplicationGroupId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ApplicationGroupId FROM dbo.[netsqlazman_ApplicationGroups]() WHERE ApplicationGroupId = @ApplicationGroupId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_ApplicationGroupsTable] WHERE [ApplicationGroupId] = @ApplicationGroupId AND [ApplicationId] = @ApplicationId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)

GO
GRANT EXECUTE
    ON OBJECT::[dbo].[netsqlazman_ApplicationGroupDelete] TO [NetSqlAzMan_Managers]
    AS [dbo];

