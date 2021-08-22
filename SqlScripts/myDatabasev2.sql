
-- QUERIES
-- Give the first and last names of all [Owner]s of a [Projects]
SELECT [BTUsers].[FirstName], [BTUsers].[LastName]
FROM [BTUsers]
WHERE [BTUsers].[Id] IN (
	SELECT [Projects].[Owner]
	FROM [Projects]
);


-- Give all the users working on [Projects] 1
SELECT [BTUsers].[FirstName], [BTUsers].[LastName]
FROM [BTUsers]
WHERE  [BTUsers].[Id] IN (
	SELECT [Assignments].[UserAssigned]
	FROM [Assignments]
	WHERE [Assignments].[Project]_[Id] = 1
);


-- Give all users who have submitted a [Tickets]
SELECT [BTUsers].[FirstName], [BTUsers].[LastName]
FROM [BTUsers]
WHERE  [BTUsers].[Id] IN (
	SELECT [Tickets].[OpenedBy]
	FROM [Tickets]
);

-- Give all users who have commented
SELECT [BTUsers].[FirstName], [BTUsers].[LastName]
FROM [BTUsers]
WHERE  [BTUsers].[Id] IN (
	SELECT [Comments].[Owner]
	FROM [Comments]
);

-- Give all users and their assigned projects
SELECT [BTUsers].[UserName], [Assignment].[ProjectId]
FROM [BTUsers]
JOIN [Assignments]
ON [BTUsers].[Id] = [Assignment].[UserAssigned] ;

-- Give all users and their assigned project names
SELECT [BTUsers].[UserName], [Assignment].[ProjectId], [Project].[Title]
FROM [BTUsers]
JOIN [Assignments]
ON [BTUsers].[Id] = [Assignment].[UserAssigned] 
JOIN [Projects]
ON [Assignment].[ProjectId] = [Project].[Id];

-- Give all users Full Names
SELECT[BTUsers].[FirstName]+' '+[BTUsers].[LastName] as FullName
FROM [BTUsers];

-- Get project owners' full names
SELECT CONCAT([BTUsers].[FirstName], ' ',  [BTUsers].LastName) AS FullName
FROM [BTUsers]
WHERE [BTUsers].[Id] IN (
	SELECT [Projects].[Owner]
	FROM [Projects]
);

-- Get all users working on project 1
SELECT * 
FROM BTUsers 
WHERE BTUsers.Id IN(
	SELECT Assignments.UserAssigned
	FROM Assignments 
	WHERE Assignments.ProjectId = 1
);

-- Get all user not assigned to a project.
SELECT * 
FROM BTUsers 
WHERE BTUsers.Id NOT IN (
	SELECT Assignments.UserAssigned
	FROM Assignments
);

-- Get a ticket's history from oldest to newest
SELECT *
FROM Tickets
WHERE Tickets.HistoryId = 1
ORDER BY Tickets.Id ASC;

-- Get most current ticket.Id from historyId (first)
SELECT Tickets.Id
FROM Tickets
WHERE Tickets.HistoryId IN (
	-- Get the HistoryId of a particular ticket.
	SELECT Tickets.HistoryId
	FROM Tickets
	WHERE Tickets.Id = 2
)
ORDER BY Tickets.Id DESC;

-- Get name of user who closed ticket.
SELECT CONCAT([BTUsers].[FirstName], ' ',  [BTUsers].LastName) AS FullName
FROM [BTUsers]
WHERE [BTUsers].[Id] IN (
	SELECT ClosedTickets.UserWhoClosed
	FROM ClosedTickets
	WHERE ClosedTickets.ProjectParent = 1 AND ClosedTickets.TicketClosed = 1002
);
-- old not used anymore
-- Add owners to the projects.
UPDATE [Projects]
SET [Projects].[Owner] = 
	(-- get hk001's Id
		SELECT [BTUsers].[Id]
		FROM BTUsers
		WHERE[BTUsers].[UserName] = 'hk001'
	)
WHERE [Projects].[Title] = 'Metal Gear Solid 5: Ground Zero';

UPDATE [Projects]
SET [Projects].[Owner] = 
	(-- get tt006's Id
		SELECT [BTUsers].[Id]
		FROM BTUsers
		WHERE[BTUsers].[UserName] = 'tt006'
	)
WHERE [Projects].[Title] = 'New Super Mario Bros.';

-- Get fullname of the person who commented id=1
SELECT CONCAT(BTUsers.FirstName, ' ',  BTUsers.LastName) AS FullName
FROM BTUsers
WHERE BTUsers.Id IN (
	SELECT Comments.Owner
	FROM Comments
	WHERE Comments.Id = 1
);










-- Delete everything
drop table [Comments] ;
drop table [ClosedTickets] ;
drop table [Tickets] ;
drop table [Assignments] ;
drop table [Projects] ;
drop table [BTUsers] ;




-- CREATING TABLES
-- IDENTITY not doing exactly what I want.
-- Taken care of in [Id]entity but gonna separate the important stuff for a model.
CREATE TABLE [BTUsers] (
	[Id] INT PRIMARY KEY,
	[StringId] NVARCHAR (450) DEFAULT NULL,
	[UserName] VARCHAR(40) DEFAULT NULL,
	[FirstName] VARCHAR(20),
	[LastName] VARCHAR(20),
	[Role] INT DEFAULT 7,
	[Administrator] INT DEFAULT NULL,
	FOREIGN KEY([Administrator]) REFERENCES [BTUsers]([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Projects] (
	[Id] INT PRIMARY KEY,
	[Title] VARCHAR(255),
	[Owner] INT,
	FOREIGN KEY([Owner]) REFERENCES [BTUsers]([Id]) ON DELETE SET NULL
);

CREATE TABLE [Assignments](
	[UserAssigned] INT,
	[ProjectId] INT,
	PRIMARY KEY([UserAssigned], [ProjectId]),
	FOREIGN KEY([UserAssigned]) REFERENCES [BTUsers]([Id]) ON DELETE CASCADE,
	FOREIGN KEY([ProjectId])    REFERENCES [Projects]([Id]) ON DELETE CASCADE
);

CREATE TABLE [Tickets](
	[ProjectParent] INT,
	[Id] INT UNIQUE,
	[HistoryId] INT,
	[IsCurr] BIT NOT NULL DEFAULT 0,
	[Title] VARCHAR(255),
	[Severity] INT,
	[Status] INT,
	[UnwantedBehavior] VARCHAR(1023),
	[RepeatableSteps] VARCHAR(1023),
	[OpenedBy] INT,
	[DateCreated] DATETIME,
	PRIMARY KEY([ProjectParent], [Id]),
	FOREIGN KEY([ProjectParent]) REFERENCES [Projects]([Id]) ON DELETE CASCADE,
	FOREIGN KEY([OpenedBy]) REFERENCES [BTUsers]([Id]) ON DELETE SET NULL
);

CREATE TABLE [ClosedTickets](
	[ProjectParent] INT,
	[TicketClosed] INT,
	[UnwantedBehaviorCause] VARCHAR(1023),
	[UnwantedBehaviorSolution] VARCHAR(1023),
	[IsTemp]  BIT NOT NULL DEFAULT 0,
	[DateClosed] DATETIME,
	[UserWhoClosed] INT,
	PRIMARY KEY([ProjectParent], [TicketClosed]),
	FOREIGN KEY([ProjectParent]) REFERENCES [Projects]([Id]) ON DELETE NO ACTION,
	FOREIGN KEY([TicketClosed]) REFERENCES [Tickets]([Id]) ON DELETE CASCADE,
	FOREIGN KEY([UserWhoClosed]) REFERENCES [BTUsers]([Id]) ON DELETE SET NULL
);


CREATE TABLE [Comments](
	[Id] INT,
	[Owner] INT,
	[TicketId] INT,
	[Msg] VARCHAR(255),
	[DateCreated] DATETIME,
	PRIMARY KEY ([Id]),
	FOREIGN KEY ([Owner]) REFERENCES [BTUsers]([Id]) ON DELETE CASCADE,
	FOREIGN KEY ([TicketId]) REFERENCES [Tickets]([Id]) ON DELETE CASCADE
);




-- POPULATING
-- Users: no [stringId] so they are dummy accounts.
INSERT INTO [BTUsers]([Id], [UserName], [FirstName], [LastName]) VALUES(1, 'hk001',     'Hideo',      'Kojima' );
INSERT INTO [BTUsers]([Id], [UserName], [FirstName], [LastName]) VALUES(2, 'gz002',   'Glenden',   'Zacharias' );
INSERT INTO [BTUsers]([Id], [UserName], [FirstName], [LastName]) VALUES(3, 'ml003',     'Matai',    'Lacusky ' );
INSERT INTO [BTUsers]([Id], [UserName], [FirstName], [LastName]) VALUES(4, 'nd004',      'Nyla',    'Driedric' );
INSERT INTO [BTUsers]([Id], [UserName], [FirstName], [LastName]) VALUES(5, 'em005', 'Ernestyna',   'Macmullan' );

-- Give Hideo Kojima the Administrator role.
UPDATE [BTUsers] 
	SET [BTUsers].[Role] = 1
	WHERE[BTUsers].[UserName] = 'hk001';

-- Make Hideo Kojima the Administrator of all the people so far.
UPDATE [BTUsers]
	SET [BTUsers].[Administrator] = (-- get hk001's Id
		SELECT [BTUsers].[Id]
		FROM BTUsers
		WHERE[BTUsers].[UserName] = 'hk001'
	)
	-- Where you don't have an Administrator AND you are not an Administrator yourself.
	WHERE [BTUsers].[Administrator] IS NULL AND [BTUsers].[Role] != 1;

-- More Users (still dummy accounts).
INSERT INTO [BTUsers]([Id], [UserName], [FirstName], [LastName]) VALUES(6, 'tt006',   'Takashi',    'Tezuka' );
INSERT INTO [BTUsers]([Id], [UserName], [FirstName], [LastName]) VALUES(7, 'po007',  'Polyxena',     'Oline' );
INSERT INTO [BTUsers]([Id], [UserName], [FirstName], [LastName]) VALUES(8, 'tf008','Tarquinius',    'Frans ' );
INSERT INTO [BTUsers]([Id], [UserName], [FirstName], [LastName]) VALUES(9, 'br009',  'Benedykt',     'Rakel' );
INSERT INTO [BTUsers]([Id], [UserName], [FirstName], [LastName]) VALUES(10, 'bm010',    'Bilhah', 'Macmullan' );

-- Give Takashi Tezuka the Administrator role.
UPDATE [BTUsers] 
	SET [BTUsers].[Role] = 1
	WHERE[BTUsers].[UserName] = 'tt006'
-- Make Takashi Tezuka the Administrator of all the people who do not have an Administrator yet.
UPDATE [BTUsers]
	SET [BTUsers].[Administrator] = (-- get tt006's Id
		SELECT [BTUsers].[Id]
		FROM BTUsers
		WHERE[BTUsers].[UserName] = 'tt006'
	)
	-- Where you don't have an Administrator AND you are not an Administrator yourself.
	WHERE [BTUsers].[Administrator] IS NULL AND [BTUsers].[Role] != 1;


-- Projects: [Id], [Title], [Owner].
-- Hideo Kojima and Takashi Tezuka are owners
INSERT INTO [Projects] VALUES(1, 'Metal Gear Solid 5: Ground Zero', 1);
INSERT INTO [Projects] VALUES(2, 'New Super Mario Bros.', 6);


-- Assign users.
INSERT INTO [Assignments] VALUES(1, 1);
INSERT INTO [Assignments] VALUES(2, 1);
INSERT INTO [Assignments] VALUES(3, 1);
INSERT INTO [Assignments] VALUES(4, 1);
INSERT INTO [Assignments] VALUES(5, 1);
INSERT INTO [Assignments] VALUES(6, 2);
INSERT INTO [Assignments] VALUES(7, 2);
INSERT INTO [Assignments] VALUES(8, 2);
INSERT INTO [Assignments] VALUES(9, 2);
INSERT INTO [Assignments] VALUES(10,2);


-- Add Tickets.
-- [ProjectParent] INT,
-- [Id] INT IDENTITY UNIQUE,
-- [HistoryId] INT,
-- [IsCurr] BIT NOT NULL DEFAULT 0,
-- [Title] VARCHAR(255),
-- [Severity] INT,
-- [Status] INT,
-- [UnwantedBehavior] VARCHAR(1023),
-- [RepeatableSteps] VARCHAR(1023),
-- [OpenedBy] INT,
-- [DateCreated] DATETIME,
--
INSERT INTO [Tickets] VALUES(
	1, 1, 1, 1, 'Aiming is not accurate', 1, 0,
	'Bullets shot from over-the-shoulder view have majorly reduced accuracy compared to first-person view. This makes it less fun.',
	'1. Hold the aim button2. Press the fire button3. Bullet gravitates towards bottom left from reticle rather then the center',
	2,
	'2021-07-16 16:06:00'
);

INSERT INTO [Tickets] VALUES(
	1, 2, 2, 1, 'Guards bounce on the floor', 2, 1,
	'Guards and other npcs move up and down in harmonic motion as if the floor is bouncy.',
	'1. Enter the Settings menu2. set fps to unlocked3. Enter the game4. get near an npc',
	3,
	'2021-06-13 16:06:00'
);

INSERT INTO [Tickets] VALUES(
	1, 3, 3, 1, 'Diving can launch the player in certain places', 3, 1,
	'Placeholder Text about diving',
	'1.',
	4,
	'2021-06-14 16:06:00'
);

INSERT INTO [Tickets] VALUES(
	2, 4, 4, 1, 'Coyote time is noticable', 3, 1,
	'Placeholder Text about Coyote time',
	'1. Step 12. Step 23.Step coyote time',
	2,
	'2021-07-10 16:06:00'
);

INSERT INTO [Tickets] VALUES(
	2, 5, 5, 1, 'The copter pickup acts like the mushroom', 3, 1,
	'Placeholder Text about copter pickup',
	'1. Step 12. Step 23.Step copter pickup',
	2,
	'2021-07-03 16:06:00'
);


-- Close a Ticket
-- [ProjectParent] INT,
-- [TicketClosed] INT,
-- [UnwantedBehaviorCause] VARCHAR(1023),
-- [UnwantedBehaviorSolution] VARCHAR(1023),
-- [IsTemp]  BIT NOT NULL DEFAULT 0,
-- [DateClosed] DATETIME,
-- [UserWhoClosed] INT,
--
INSERT INTO [ClosedTickets] VALUES(
	1, 3, 
	'The vectors were not calculated properly', 
	'I redid an example by hand and implemented that', 
	0, 
	'2021-07-16 16:06:00', 
	4
);
UPDATE [Tickets]
	SET [Tickets].[Status] = 3, [Tickets].[IsCurr] = 0
	WHERE [Tickets].[Id] = 3;


-- Comments
-- [Id] INT,
-- [Owner] INT,
-- [TicketId] INT,
-- [Msg] VARCHAR(255),
-- [DateCreated] DATETIME,
--
INSERT INTO [Comments] VALUES(
	1, 1, 1, 
	'This is a comment',
	'2021-07-16 16:06:00'
);


