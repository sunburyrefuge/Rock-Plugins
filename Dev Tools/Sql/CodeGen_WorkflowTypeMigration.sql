set nocount on
declare
@crlf varchar(2) = char(13) + char(10)

begin

IF OBJECT_ID('tempdb..#codeTable') IS NOT NULL
    DROP TABLE #codeTable

IF OBJECT_ID('tempdb..#knownGuidsToIgnore') IS NOT NULL
    DROP TABLE #knownGuidsToIgnore

create table #knownGuidsToIgnore(
    [Guid] UniqueIdentifier, 
    CONSTRAINT [pk_knownGuidsToIgnore] PRIMARY KEY CLUSTERED  ( [Guid]) 
);

-- Categories
insert into #knownGuidsToIgnore values 
('8F8B272D-D351-485E-86D6-3EE5B7C84D99')  --Checkin

-- Workflow Types
--insert into #knownGuidsToIgnore values 
--('011E9F5A-60D4-4FF5-912A-290881E37EAF'),  --Checkin
--('C93EEC26-4BE3-4EB5-92D4-5C30EEF069D9'),  --Parse Labels
--('221BF486-A82C-40A7-85B7-BB44DA45582F'),  --Person Data Error
--('236AB611-EDE8-42B5-B559-6B6A88ADDDCB'),  --External Inquiry
--('417D8016-92DC-4F25-ACFF-A071B591FA4F'),  --Facilities Request
--('51FE9641-FB8F-41BF-B09E-235900C3E53E'),  --IT Support
--('036F2F0B-C2DC-49D0-A17B-CCDAC7FC71E2'),  --Photo Request
--('655BE2A4-2735-4CF9-AEC8-7EF5BE92724C'),  --Position Approval
--('16D12EF7-C546-4039-9036-B73D118EDC90'),  --Background Check
--('885CBA61-44EA-4B4A-B6E1-289041B6A195'),  --DISC Request
--('C9D8F6A2-CE98-4DD5-8963-9403D02F9CC8'),  --Apollos UserLogin Delete Workflow Type
--('7CB0BE68-98B9-44FC-90E8-D78DD59DE3DC'),  --Apollos UserLogin Save Workflow Type
--('8F066EB1-95E9-43C7-81F7-D6A7CB27B90B'),  --Apollos Person Delete Workflow Type
--('C1EC88CB-7F52-4938-ADEF-C28959F38F96'),  --Apollos Person Save Workflow Type
--('6E8CD562-A1DA-4E13-A45C-853DB56E0014'),  --Attended Check-in
--('F3218F81-423F-46EA-B748-ED7AB365CD07'),  --Send A Text Message
--('C53337C8-7352-4159-8B8C-D0E4076FF96C')   --Request For Criminal Records

insert into #knownGuidsToIgnore values 
('011E9F5A-60D4-4FF5-912A-290881E37EAF'),
('C93EEC26-4BE3-4EB5-92D4-5C30EEF069D9'),
('221BF486-A82C-40A7-85B7-BB44DA45582F'),
('236AB611-EDE8-42B5-B559-6B6A88ADDDCB'),
('417D8016-92DC-4F25-ACFF-A071B591FA4F'),
('51FE9641-FB8F-41BF-B09E-235900C3E53E'),
('036F2F0B-C2DC-49D0-A17B-CCDAC7FC71E2'),
('655BE2A4-2735-4CF9-AEC8-7EF5BE92724C'),
('16D12EF7-C546-4039-9036-B73D118EDC90'),
('885CBA61-44EA-4B4A-B6E1-289041B6A195'),
('C9D8F6A2-CE98-4DD5-8963-9403D02F9CC8'),
('7CB0BE68-98B9-44FC-90E8-D78DD59DE3DC'),
('8F066EB1-95E9-43C7-81F7-D6A7CB27B90B'),
('C1EC88CB-7F52-4938-ADEF-C28959F38F96'),
('6E8CD562-A1DA-4E13-A45C-853DB56E0014'),
('14947674-A075-4AC7-BF99-79326980ED9D'),
('08765EA3-2973-4AA8-BD1F-F8C0E86A4E44'),
('259D88F2-D1CD-48FC-802F-C519CDFEBACA'),
('638970FA-0261-4A98-BD86-7C110BB1B158'),
('E178756A-3968-4C22-AF62-41E9F42C693C'),
('FBA8DB8E-06B1-4400-A486-C8EB0DB0467D'),
('AFACD892-A663-4DEE-B9CF-01DE6001308F'),
('92CA5A5A-3AF4-47FB-8BB3-5741B1F517BA'),
('64552484-C96C-487E-AAC2-BABC05054E98'),
('8A57F52D-A706-405B-911D-E3A5ABF4AF22'),
('3DED7E24-0B8C-4CBC-A79F-1CDC8A8F4D7D'),
('F868FE91-9D54-43EA-873A-A1A3A5316EAE'),
('E94306BF-E623-48A4-9729-AAB5C2291C03'),
('9CA0354B-02D1-41CA-805D-1ADDF6B83A6B'),
('6F5BA05C-24EE-494A-83AB-6F9FE1312461'),
('511D0180-A0D7-433C-80E2-2BCA74BCA271'),
('E2DBDD4F-1704-474A-ACC6-07E2405951A7'),
('6F38D246-2227-4DA0-ACE0-5386A23DC638'),
('74A62192-832B-4B80-A47E-6CF327068FE2'),
('952825D7-15CE-4E46-B520-B1B0DC107CC5'),
('54C51F65-7467-48DF-9617-8406C50CBE39'),
('2C051FCB-EF64-4AB1-A1FA-880F194CB893'),
('3D034947-9D3F-45F6-9986-1978CA13374E'),
('9BB5CB25-74C4-4CD4-846A-657026BE2590'),
('A9998262-EAAF-4A78-B9CE-43EE7DB6663E'),
('1EE51060-5A1D-4B4B-AA59-3AEFBF59A944'),
('22614B61-C2FF-48B4-BA23-1DAD1DD7DF77'),
('0EBEB85C-6AEB-4454-AD7C-3793770BAE51'),
('96D3131B-1F92-4954-93EF-12EA367A7CF4'),
('43F177AC-E2EB-40C9-B041-5FA810F6B9ED'),
('1B4F8EA0-ED4C-4376-AC48-E0C3D7D9DD7F'),
('5FF478A1-0ABB-40E8-B40D-25BB381C0F8E'),
('89171E14-EA0C-49A1-8C85-49C90ECF680B'),
('F895D4AA-AE31-42DC-8FAE-28D1E86BC2C9'),
('0C7E0616-014D-4447-BE06-1CE76957E75E'),
('731853B1-93F1-44E8-9EE1-062CC94C63DB'),
('62E1411D-7DA1-4B77-917F-67B5CA7FE80F'),
('F3218F81-423F-46EA-B748-ED7AB365CD07'),
('CAC8E272-30A0-460C-9CB2-FF4AF669DDB5'),
('3969F564-1E30-4851-AF11-E70283BA9DD7'),
('D96FF580-362E-461F-9952-7EC885563C09'),
('509C62AF-05FC-4E3A-822E-0FFDFC82E965'),
('0BE77C0A-DA8C-4E78-A558-52B799C1E149'),
('AAEA0DDE-91DB-4E27-B005-4D61BBF0354B'),
('127DA1BD-311D-4114-ADFA-AA45B360156F'),
('8B1562F2-3859-4AC0-A4B3-BC84858244DB'),
('6ED7962C-8408-45D6-9954-7C92EE6C04D9'),
('5503055F-E5C3-42B5-B290-CD2D1298D155'),
('D2996FBB-E80C-404B-AFC0-CA9FEE1B5420'),
('7D9EA409-7245-4E6F-905E-11A31079EBAC'),
('3BCD6A54-2204-4404-B320-BB456F9ECA98'),
('42D5F513-225B-40B2-A094-AB830EB70C5D')
--('F3B48EF4-4E42-4CA6-BD7A-DD92FD50354F'), 
create table #codeTable (
    Id int identity(1,1) not null,
    CodeText nvarchar(max),
    CONSTRAINT [pk_codeTable] PRIMARY KEY CLUSTERED  ( [Id]) );
    
	-- field Types
	insert into #codeTable
	SELECT
        '            RockMigrationHelper.UpdateFieldType("'+    
        ft.Name+ '","'+ 
        ISNULL(ft.Description,'')+ '","'+ 
        ft.Assembly+ '","'+ 
        ft.Class+ '","'+ 
        CONVERT(nvarchar(50), ft.Guid)+ '");'+
        @crlf
    from [FieldType] [ft]
    where (ft.IsSystem = 0)
	
	-- entitiy types
    insert into #codeTable
    values 
		('            RockMigrationHelper.UpdateEntityType("Rock.Model.Workflow", "3540E9A7-FE30-43A9-8B0A-A372B63DFC93", true, true);' + @crlf ),
		('            RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActivity", "2CB52ED0-CB06-4D62-9E2C-73B60AFA4C9F", true, true);' + @crlf ),
		('            RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActionType", "23E3273A-B137-48A3-9AFF-C8DC832DDCA6", true, true);' + @crlf )

	-- Action entity types
    insert into #codeTable
    SELECT DISTINCT
        '            RockMigrationHelper.UpdateEntityType("'+
		[et].[name]+ '","'+   
        CONVERT(nvarchar(50), [et].[Guid])+ '",'+     
		(CASE [et].[IsEntity] WHEN 1 THEN 'true' ELSE 'false' END) + ','+
		(CASE [et].[IsSecured] WHEN 1 THEN 'true' ELSE 'false' END) + ');' +
        @crlf
    from [WorkflowActionType] [a]
	inner join [WorkflowActivityType] [at] on [a].[ActivityTypeId] = [at].[id]
	inner join [WorkflowType] [wt] on [at].[WorkflowTypeId] = [wt].[id]
	inner join [EntityType] [et] on [et].[Id] = [a].[EntityTypeId]
    where [wt].[Guid] not in (select [Guid] from #knownGuidsToIgnore)

    insert into #codeTable
    SELECT @crlf

	-- Action entity type attributes
    insert into #codeTable
    SELECT DISTINCT
        '            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("'+ 
        CONVERT(nvarchar(50), [aet].[Guid])+ '","'+   
        CONVERT(nvarchar(50), [ft].[Guid])+ '","'+     
        [a].[Name]+ '","'+  
        [a].[Key]+ '","'+ 
        ISNULL(REPLACE([a].[Description],'"','\"'),'')+ '",'+ 
        CONVERT(varchar, [a].[Order])+ ',@"'+ 
        ISNULL([a].[DefaultValue],'')+ '","'+
        CONVERT(nvarchar(50), [a].[Guid])+ '");' +
        ' // ' + aet.Name + ':'+ a.Name+
        @crlf
	from [Attribute] [a] 
	inner join [EntityType] [et] on [et].[Id] = [a].[EntityTypeId] and [et].Name = 'Rock.Model.WorkflowActionType'
    inner join [FieldType] [ft] on [ft].[Id] = [a].[FieldTypeId]
	inner join [EntityType] [aet] on CONVERT(varchar, [aet].[id]) = [a].[EntityTypeQualifierValue]
    where [a].[EntityTypeQualifierColumn] = 'EntityTypeId'
	and [aet].[id] in (
		select distinct [at].[EntityTypeId]
		from [WorkflowType] [wt]
		inner join [WorkflowActivityType] [act] on [act].[WorkflowTypeId] = [wt].[id]
		inner join [WorkflowActionType] [at] on [at].[ActivityTypeId] = [act].[id]
		and [wt].[Guid] not in (select [Guid] from #knownGuidsToIgnore)
	)

    insert into #codeTable
    SELECT @crlf

	-- categories
    insert into #codeTable
    SELECT 
		'            RockMigrationHelper.UpdateCategory("' +
        CONVERT( nvarchar(50), [e].[Guid]) + '","'+ 
        [c].[Name] +  '","'+
        [c].[IconCssClass] +  '","'+
        ISNULL(REPLACE([c].[Description],'"','\"'),'')+ '","'+ 
        CONVERT( nvarchar(50), [c].[Guid])+ '",'+
		CONVERT( nvarchar, [c].[Order] )+ ');' +
		' // ' + c.Name +
        @crlf
    FROM [Category] [c]
    join [EntityType] [e] on [e].[Id] = [c].[EntityTypeId]
    where [c].[IsSystem] = 0
    and [c].[Guid] not in (select [Guid] from #knownGuidsToIgnore)
	and [e].[Name] = 'Rock.Model.WorkflowType'
    order by [c].[Order]

    insert into #codeTable
    SELECT @crlf

	-- Workflow Type
    insert into #codeTable
    SELECT 
        '            RockMigrationHelper.UpdateWorkflowType('+ 
		(CASE [wt].[IsSystem] WHEN 1 THEN 'true' ELSE 'false' END) + ','+
		(CASE [wt].[IsActive] WHEN 1 THEN 'true' ELSE 'false' END) + ',"'+
        [wt].[Name]+ '","'+  
        ISNULL(REPLACE([wt].[Description],'"','\"'),'')+ '","'+ 
        CONVERT(nvarchar(50), [c].[Guid])+ '","'+     
        [wt].[WorkTerm]+ '","'+
        ISNULL([wt].[IconCssClass],'')+ '",'+ 
        CONVERT(varchar, ISNULL([wt].[ProcessingIntervalSeconds],0))+ ','+
		(CASE [wt].[IsPersisted] WHEN 1 THEN 'true' ELSE 'false' END) + ','+
        CONVERT(varchar, [wt].[LoggingLevel])+ ',"'+
		CONVERT(nvarchar(50), [wt].[Guid])+ '");'+
        ' // ' + wt.Name + 
        @crlf
    from [WorkflowType] [wt]
	inner join [Category] [c] on [c].[Id] = [wt].[CategoryId] 
    and [wt].[Guid] not in (select [Guid] from #knownGuidsToIgnore)

    insert into #codeTable
    SELECT @crlf

	-- Workflow Type Attributes
    insert into #codeTable
    SELECT 
        '            RockMigrationHelper.UpdateWorkflowTypeAttribute("'+ 
        CONVERT(nvarchar(50), wt.Guid)+ '","'+   
        CONVERT(nvarchar(50), ft.Guid)+ '","'+     
        a.Name+ '","'+  
        a.[Key]+ '","'+ 
        ISNULL(a.Description,'')+ '",'+ 
        CONVERT(varchar, a.[Order])+ ',@"'+ 
        ISNULL(a.DefaultValue,'')+ '","'+
        CONVERT(nvarchar(50), a.Guid)+ '");' +
        ' // ' + wt.Name + ':'+ a.Name+
        @crlf
    from [WorkflowType] [wt]
	inner join [Attribute] [a] on cast([a].[EntityTypeQualifierValue] as int) = [wt].[Id] 
	inner join [EntityType] [et] on [et].[Id] = [a].[EntityTypeId] and [et].Name = 'Rock.Model.Workflow'
    inner join [FieldType] [ft] on [ft].[Id] = [a].[FieldTypeId]
    where EntityTypeQualifierColumn = 'WorkflowTypeId'
    and [wt].[Guid] not in (select [Guid] from #knownGuidsToIgnore)

    insert into #codeTable
    SELECT @crlf

	-- Workflow Type Attribute Qualifiers
    insert into #codeTable
    SELECT 
        '            RockMigrationHelper.AddAttributeQualifier("'+ 
        CONVERT(nvarchar(50), a.Guid)+ '","'+   
        [aq].[Key]+ '",@"'+ 
        ISNULL([aq].[Value],'')+ '","'+
        CONVERT(nvarchar(50), [aq].[Guid])+ '");' +
        ' // ' + [wt].[Name] + ':'+ [a].[Name]+ ':'+ [aq].[Key]+
        @crlf
    from [WorkflowType] [wt]
	inner join [Attribute] [a] on cast([a].[EntityTypeQualifierValue] as int) = [wt].[Id] 
	inner join [AttributeQualifier] [aq] on [aq].[AttributeId] = [a].[Id]
	inner join [EntityType] [et] on [et].[Id] = [a].[EntityTypeId] and [et].Name = 'Rock.Model.Workflow'
    where [a].[EntityTypeQualifierColumn] = 'WorkflowTypeId'
    and [wt].[Guid] not in (select [Guid] from #knownGuidsToIgnore)

    insert into #codeTable
    SELECT @crlf

	-- Workflow Activity Type
    insert into #codeTable
    SELECT 
        '            RockMigrationHelper.UpdateWorkflowActivityType("'+ 
        CONVERT(nvarchar(50), [wt].[Guid])+ '",'+     
		(CASE [at].[IsActive] WHEN 1 THEN 'true' ELSE 'false' END) + ',"'+
        [at].[Name]+ '","'+  
        ISNULL(REPLACE([at].[Description],'"','\"'),'')+ '",'+ 
		(CASE [at].IsActivatedWithWorkflow WHEN 1 THEN 'true' ELSE 'false' END) + ','+
        CONVERT(varchar, [at].[Order])+ ',"'+
        CONVERT(nvarchar(50), [at].[Guid])+ '");' +
        ' // ' + wt.Name + ':'+ at.Name+
        @crlf
    from [WorkflowActivityType] [at]
	inner join [WorkflowType] [wt] on [wt].[id] = [at].[WorkflowTypeId]
    where [wt].[Guid] not in (select [Guid] from #knownGuidsToIgnore)

    insert into #codeTable
    SELECT @crlf

	-- Workflow Activity Type Attributes
    insert into #codeTable
    SELECT 
        '            RockMigrationHelper.UpdateWorkflowActivityTypeAttribute("'+ 
        CONVERT(nvarchar(50), at.Guid)+ '","'+   
        CONVERT(nvarchar(50), ft.Guid)+ '","'+     
        a.Name+ '","'+  
        a.[Key]+ '","'+ 
        ISNULL(a.Description,'')+ '",'+ 
        CONVERT(varchar, a.[Order])+ ',@"'+ 
        ISNULL(a.DefaultValue,'')+ '","'+
        CONVERT(nvarchar(50), a.Guid)+ '");' +
        ' // ' + wt.Name + ':'+ at.Name + ':'+ a.Name+
        @crlf
    from [WorkflowType] [wt]
	inner join [WorkflowActivityType] [at] on [at].[WorkflowTypeId] = [wt].[id]
	inner join [Attribute] [a] on cast([a].[EntityTypeQualifierValue] as int) = [at].[Id] 
	inner join [EntityType] [et] on [et].[Id] = [a].[EntityTypeId] and [et].Name = 'Rock.Model.WorkflowActivity'
    inner join [FieldType] [ft] on [ft].[Id] = [a].[FieldTypeId]
    where [a].[EntityTypeQualifierColumn] = 'ActivityTypeId'
    and [wt].[Guid] not in (select [Guid] from #knownGuidsToIgnore)

    insert into #codeTable
    SELECT @crlf

	-- Workflow Activity Type Attribute Qualifiers
    insert into #codeTable
    SELECT 
        '            RockMigrationHelper.AddAttributeQualifier("'+ 
        CONVERT(nvarchar(50), a.Guid)+ '","'+   
        [aq].[Key]+ '",@"'+ 
        ISNULL([aq].[Value],'')+ '","'+
        CONVERT(nvarchar(50), [aq].[Guid])+ '");' +
        ' // ' + [wt].[Name] + ':'+ [a].[Name]+ ':'+ [aq].[Key]+
        @crlf
    from [WorkflowType] [wt]
	inner join [WorkflowActivityType] [at] on [at].[WorkflowTypeId] = [wt].[id]
	inner join [Attribute] [a] on cast([a].[EntityTypeQualifierValue] as int) = [at].[Id] 
	inner join [AttributeQualifier] [aq] on [aq].[AttributeId] = [a].[Id]
	inner join [EntityType] [et] on [et].[Id] = [a].[EntityTypeId] and [et].Name = 'Rock.Model.WorkflowActivity'
    where [a].[EntityTypeQualifierColumn] = 'ActivityTypeId'
    and [wt].[Guid] not in (select [Guid] from #knownGuidsToIgnore)

    insert into #codeTable
    SELECT @crlf

	-- Action Forms
    insert into #codeTable
    SELECT 
        '            RockMigrationHelper.UpdateWorkflowActionForm(@"'+ 
        ISNULL([f].[Header],'')+ '",@"'+ 
        ISNULL([f].[Footer],'')+ '","'+ 
        ISNULL([f].[Actions],'')+ '","'+ 
		(CASE WHEN [se].[Guid] IS NULL THEN '' ELSE CONVERT(nvarchar(50), [se].[Guid]) END) + '",'+
		(CASE [f].[IncludeActionsInNotification] WHEN 1 THEN 'true' ELSE 'false' END) + ',"'+
        ISNULL(CONVERT(nvarchar(50), [f].[ActionAttributeGuid]),'')+ '","'+ 
		CONVERT(nvarchar(50), [f].[Guid])+ '");' +
        ' // ' + wt.Name + ':'+ at.Name + ':'+ a.Name+
        @crlf
    from [WorkflowActionForm] [f]
	inner join [WorkflowActionType] [a] on [a].[WorkflowFormId] = [f].[id]
	inner join [WorkflowActivityType] [at] on [at].[id] = [a].[ActivityTypeId]
	inner join [WorkflowType] [wt] on [wt].[id] = [at].[WorkflowTypeId]
	left outer join [SystemEmail] [se] on [se].[id] = [f].[NotificationSystemEmailId]
    where [wt].[Guid] not in (select [Guid] from #knownGuidsToIgnore)

    insert into #codeTable
    SELECT @crlf

	-- Action Form Attributes
    insert into #codeTable
    SELECT 
        '            RockMigrationHelper.UpdateWorkflowActionFormAttribute("'+ 
		CONVERT(nvarchar(50), [f].[Guid])+ '","' +
		CONVERT(nvarchar(50), [a].[Guid])+ '",' +
		CONVERT(varchar, [fa].[Order])+ ',' +
		(CASE [fa].[IsVisible] WHEN 1 THEN 'true' ELSE 'false' END) + ','+
		(CASE [fa].[IsReadOnly] WHEN 1 THEN 'true' ELSE 'false' END) + ','+
		(CASE [fa].[IsRequired] WHEN 1 THEN 'true' ELSE 'false' END) + ',"'+
		CONVERT(nvarchar(50), [fa].[Guid])+ '");' +
        ' // '+ wt.Name+ ':'+ act.Name+ ':'+ at.Name+ ':'+ a.Name+
        @crlf
    from [WorkflowActionFormAttribute] [fa]
	inner join [WorkflowActionForm] [f] on [f].[id] = [fa].[WorkflowActionFormId]
	inner join [Attribute] [a] on [a].[id] = [fa].[AttributeId]
	inner join [WorkflowActionType] [at] on [at].[WorkflowFormId] = [f].[id]
	inner join [WorkflowActivityType] [act] on [act].[id] = [at].[ActivityTypeId]
	inner join [WorkflowType] [wt] on [wt].[id] = [act].[WorkflowTypeId]
    where [wt].[Guid] not in (select [Guid] from #knownGuidsToIgnore)

    insert into #codeTable
    SELECT @crlf

	-- Workflow Action Type
    insert into #codeTable
    SELECT 
        '            RockMigrationHelper.UpdateWorkflowActionType("'+ 
        CONVERT(nvarchar(50), [at].[Guid])+ '","'+     
        [a].[Name]+ '",'+  
        CONVERT(varchar, [a].[Order])+ ',"'+
        CONVERT(nvarchar(50), [et].[Guid])+ '",'+     
		(CASE [a].[IsActionCompletedOnSuccess] WHEN 1 THEN 'true' ELSE 'false' END) + ','+
		(CASE [a].[IsActivityCompletedOnSuccess] WHEN 1 THEN 'true' ELSE 'false' END) + ',"'+
		(CASE WHEN [f].[Guid] IS NULL THEN '' ELSE CONVERT(nvarchar(50), [f].[Guid]) END) + '","'+
        ISNULL(CONVERT(nvarchar(50), [a].[CriteriaAttributeGuid]),'')+ '",'+ 
        CONVERT(varchar, [a].[CriteriaComparisonType])+ ',"'+ 
        ISNULL([a].[CriteriaValue],'')+ '","'+ 
        CONVERT(nvarchar(50), [a].[Guid])+ '");' +
        ' // '+ wt.Name+ ':'+ at.Name+ ':'+ a.Name+
        @crlf
    from [WorkflowActionType] [a]
	inner join [WorkflowActivityType] [at] on [at].[id] = [a].[ActivityTypeId]
	inner join [WorkflowType] [wt] on [wt].[id] = [at].[WorkflowTypeId]
	inner join [EntityType] [et] on [et].[id] = [a].[EntityTypeId]
	left outer join [WorkflowActionForm] [f] on [f].[id] = [a].[WorkflowFormId]
    where [wt].[Guid] not in (select [Guid] from #knownGuidsToIgnore)

    insert into #codeTable
    SELECT @crlf


    -- Workflow Action Type attributes values 
    insert into #codeTable
    SELECT 
		CASE WHEN [FT].[Guid] = 'E4EAB7B2-0B76-429B-AFE4-AD86D7428C70' THEN
        '            RockMigrationHelper.AddActionTypePersonAttributeValue("' ELSE
        '            RockMigrationHelper.AddActionTypeAttributeValue("' END+
        CONVERT(nvarchar(50), at.Guid)+ '","'+ 
        CONVERT(nvarchar(50), a.Guid)+ '",@"'+ 
		REPLACE(ISNULL(av.Value,''), '"', '""') + '");'+
        ' // '+ wt.Name+ ':'+ act.Name+ ':'+ at.Name+ ':'+ a.Name +
        @crlf
    from [AttributeValue] [av]
    inner join [WorkflowActionType] [at] on [at].[Id] = [av].[EntityId]
    inner join [Attribute] [a] on [a].[id] = [av].[AttributeId] AND [a].EntityTypeQualifierValue = CONVERT(nvarchar, [at].EntityTypeId)
	inner join [FieldType] [ft] on [ft].[id] = [a].[FieldTypeId] 
	inner join [EntityType] [et] on [et].[id] = [a].[EntityTypeId] and [et].[Name] = 'Rock.Model.WorkflowActionType'
    inner join [WorkflowActivityType] [act] on [act].[Id] = [at].[ActivityTypeId]
	inner join [WorkflowType] [wt] on [wt].[Id] = [act].[WorkflowTypeId]
    and [wt].[Guid] not in (select [Guid] from #knownGuidsToIgnore) 
	order by [wt].[Order], [act].[Order], [at].[Order], [a].[Order]

    select CodeText [MigrationUp] from #codeTable
    where REPLACE(CodeText, @crlf, '') != ''
    order by Id

IF OBJECT_ID('tempdb..#codeTable') IS NOT NULL
    DROP TABLE #codeTable

IF OBJECT_ID('tempdb..#knownGuidsToIgnore') IS NOT NULL
    DROP TABLE #knownGuidsToIgnore

end