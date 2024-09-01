CREATE TABLE TestRuns (
    TestRunID INT PRIMARY KEY,
    TestName VARCHAR(255),
    ClassName VARCHAR(255),
    RunDateTime TIMESTAMP,
    DurationMs INTEGER,
    Result VARCHAR(50),
    ExceptionType VARCHAR(255),
    ExceptionMessage VARCHAR(255),
    OperatingSystemName VARCHAR(25),
    OperatingSystemVersion VARCHAR(25),
    BrowserName VARCHAR(25),
    BrowserVersion VARCHAR(25),
    LogMessages TEXT
);
