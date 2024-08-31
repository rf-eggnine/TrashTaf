CREATE TABLE TestRuns (
    TestRunID INT PRIMARY KEY,
    testName VARCHAR(255),
    className VARCHAR(255),
    runDateTime TIMESTAMP,
    durationMs INTEGER,
    result VARCHAR(50),
    exceptionType VARCHAR(255),
    exceptionMessage VARCHAR(255),
    operatingSystemName VARCHAR(25),
    operatingSystemVersion VARCHAR(25),
    browserName VARCHAR(25),
    browserVersion VARCHAR(25),
    logMessages TEXT
);
