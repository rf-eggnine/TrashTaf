CREATE TABLE TestRuns (
    TestRunID INT PRIMARY KEY,
    TestName VARCHAR(255) NOT NULL,
    Status VARCHAR(50),
    StartTime DATETIME,
    EndTime DATETIME
);
