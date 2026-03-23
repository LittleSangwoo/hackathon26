CREATE DATABASE YouthParliamentDB;
GO
USE YouthParliamentDB;
GO

-- =============================================
-- 1. ÌÎÄÓËÜ ÏÎËÜÇÎÂÀÒÅËÅÉ È ÄÎÑÒÓÏÀ (Identity)
-- =============================================

CREATE TABLE [Roles] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE [Users] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Email] NVARCHAR(256) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [FullName] NVARCHAR(256) NOT NULL,
    [AvatarUrl] NVARCHAR(MAX) NULL,
    [City] NVARCHAR(100) NULL,
    [Age] INT NULL,
    [RoleId] INT NOT NULL,
    [TotalRating] DECIMAL(18, 2) DEFAULT 0.00,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_Users_Roles FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id])
);

CREATE TABLE [OrganizerProfiles] (
    [UserId] UNIQUEIDENTIFIER PRIMARY KEY,
    [TrustRating] DECIMAL(3, 2) DEFAULT 5.00, -- Ðåéòèíã îò 1.00 äî 5.00
    [Bio] NVARCHAR(MAX) NULL,
    [IsVerified] BIT DEFAULT 0,
    
    CONSTRAINT FK_OrganizerProfiles_Users FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);

-- =============================================
-- 2. ÌÎÄÓËÜ ÌÅÐÎÏÐÈßÒÈÉ È ÊÀÒÅÃÎÐÈÉ
-- =============================================

CREATE TABLE [EventCategories] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [IconUrl] NVARCHAR(MAX) NULL
);

CREATE TABLE [Events] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [OrganizerId] UNIQUEIDENTIFIER NOT NULL,
    [CategoryId] INT NOT NULL,
    [Title] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Location] NVARCHAR(255) NULL,
    [EventDate] DATETIME2 NOT NULL,
    [ComplexityWeight] DECIMAL(3, 2) DEFAULT 1.00, -- Êîýôôèöèåíò ñëîæíîñòè
    [Status] INT DEFAULT 0, -- 0: Draft, 1: Active, 2: Completed, 3: Cancelled
    [VerificationCode] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Events_Users FOREIGN KEY ([OrganizerId]) REFERENCES [Users]([Id]),
    CONSTRAINT FK_Events_Categories FOREIGN KEY ([CategoryId]) REFERENCES [EventCategories]([Id])
);

CREATE TABLE [Prizes] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [EventId] UNIQUEIDENTIFIER NOT NULL,
    [Title] NVARCHAR(255) NOT NULL,
    [PointsValue] INT DEFAULT 0,
    [IsBonus] BIT DEFAULT 0,

    CONSTRAINT FK_Prizes_Events FOREIGN KEY ([EventId]) REFERENCES [Events]([Id]) ON DELETE CASCADE
);

-- =============================================
-- 3. ÌÎÄÓËÜ Ó×ÀÑÒÈß È ÐÅÉÒÈÍÃÀ
-- =============================================

CREATE TABLE [Participations] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [EventId] UNIQUEIDENTIFIER NOT NULL,
    [RegistrationDate] DATETIME2 DEFAULT GETUTCDATE(),
    [CheckInDate] DATETIME2 NULL,
    [Status] INT DEFAULT 0, -- 0: Registered, 1: Attended, 2: Rejected
    [FinalPoints] DECIMAL(18, 2) DEFAULT 0.00,

    CONSTRAINT FK_Participations_Users FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
    CONSTRAINT FK_Participations_Events FOREIGN KEY ([EventId]) REFERENCES [Events]([Id])
);

CREATE TABLE [OrganizerReviews] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [OrganizerId] UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId] UNIQUEIDENTIFIER NOT NULL,
    [Score] INT CHECK ([Score] >= 1 AND [Score] <= 5),
    [Comment] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Reviews_Organizer FOREIGN KEY ([OrganizerId]) REFERENCES [Users]([Id]),
    CONSTRAINT FK_Reviews_Participant FOREIGN KEY ([ParticipantId]) REFERENCES [Users]([Id])
);

-- =============================================
-- 4. ÍÀÑÒÐÎÉÊÈ È ÈÍÄÅÊÑÛ
-- =============================================

CREATE TABLE [GlobalSettings] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [SettingKey] NVARCHAR(100) NOT NULL UNIQUE,
    [SettingValue] NVARCHAR(MAX) NOT NULL
);

-- Èíäåêñû äëÿ óñêîðåíèÿ ðàáîòû ðåéòèíãà è ôèëüòðîâ
CREATE INDEX IX_Users_TotalRating ON [Users] ([TotalRating] DESC);
CREATE INDEX IX_Events_Date ON [Events] ([EventDate]);
CREATE INDEX IX_Participations_UserStatus ON [Participations] ([UserId], [Status]);

GO