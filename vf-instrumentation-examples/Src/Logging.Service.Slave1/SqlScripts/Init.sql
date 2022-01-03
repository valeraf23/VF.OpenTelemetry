USE MASTER
GO
    IF NOT EXISTS (
        SELECT
            *
        FROM
            sys.databases
        WHERE
            name = 'Slave1'
    ) BEGIN CREATE DATABASE Slave1;

END;

GO
    USE Slave1;

GO
    IF OBJECT_ID('Data', 'U') IS NULL BEGIN CREATE TABLE dbo.Data (
        Id INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
        Value VARCHAR(100)
    );

END;

IF OBJECT_ID('SubData', 'U') IS NULL BEGIN CREATE TABLE dbo.SubData (
    Id INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    Value VARCHAR(100),
    Data INT FOREIGN KEY REFERENCES Data(Id)
);

END;

IF TYPE_ID(N'[DataType]') IS NULL BEGIN CREATE TYPE [dbo].[DataType] AS TABLE(
    [Data] [varchar](100) NULL,
    [SubData] [varchar](100) NULL
)
END;

IF OBJECT_ID('InsertData', 'P') IS NULL EXEC(
    'CREATE PROCEDURE dbo.InsertData AS BEGIN

SELECT  NULL; END;'
);

GO
    ALTER PROCEDURE dbo.InsertData @Data AS varchar(100),
    @SubData AS varchar(100) AS BEGIN declare @Data_ID int
SELECT
    @Data_ID = Id
FROM
    dbo.Data
WHERE
    dbo.Data.Value = @Data IF @Data_ID IS NULL BEGIN
INSERT INTO
    dbo.Data
VALUES
    (@Data)
SET
    @Data_ID = @@identity
END
INSERT INTO
    dbo.SubData
VALUES
    (@SubData, @Data_ID)
END;

GO
    IF OBJECT_ID('SeedData', 'P') IS NULL EXEC(
        'CREATE PROCEDURE dbo.SeedData AS BEGIN

SELECT  NULL; END;'
    );

GO
    ALTER PROCEDURE dbo.SeedData @tblData DataType READONLY AS BEGIN IF NOT EXISTS (
        SELECT
            *
        FROM
            dbo.SubData
    ) BEGIN DECLARE @SQL varchar(max) = ''
SELECT
    @SQL = @SQL + 'exec InsertData ' + '"' + Data + '"' + ',' + '"' + SubData + '"' + ';'
FROM
    @tblData EXECUTE (@SQL);

END
END;