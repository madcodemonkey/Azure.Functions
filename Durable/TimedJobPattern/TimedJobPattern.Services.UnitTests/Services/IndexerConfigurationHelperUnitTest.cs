using TimedJobPattern.Modals;

namespace TimedJobPattern.Services.UnitTests;

[TestClass]
public class IndexerConfigurationHelperUnitTest
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ComputeNextIndexerRun_SettingRunTimeIntervalToZero_CausesAnException()
    {
        // Arrange
        var classUnderTest = new WorkerConfigurationService();
        var config = new WorkerConfiguration()
        {
            Id = Guid.NewGuid(),
            AddIntervalToNextRunTime = true,
            LastRunTime = DateTime.UtcNow.AddMinutes(-12),
            NextRunTime = DateTime.UtcNow.AddMinutes(-1),
            LastHeartbeatTime = null,
            RunTimeIntervalInMinutes = 0 // <------------------------- Do NOT do this...generates an exception.
         };
        
        // Act
        classUnderTest.ComputeNextWorkerRun(config);

        // Assert
        Assert.Fail("Expected an exception!");
    }

    /// <summary>
    /// When AddIntervalToNextRunTime is false, we should add the RunTimeIntervalInMinutes to
    /// DateTime.UtcNow. The next run time will most likely move because the time is computed
    /// AFTER it finishes working on changes.
    ///
    /// Keep in mind that we JUST ran the indexer and now we are looking at the time that the
    /// Next run should happen. If the time interval is still in the future, someone manually
    /// triggered it. If the time interval is way in the past, the service was down and is
    /// catching back up!
    /// </summary>
    /// <param name="nextRunAdjustment">
    /// The number of minutes to add to the NextRunTime. For example, if it is zero, it is set
    /// to DateTime.UtcNow. If it where 15, we would add 15 minutes to DateTime.UtcNow.
    /// </param>
    /// <param name="runTimeIntervalInMinutes">
    /// The run time interval to add after completing the work.
    /// </param>
    [DataTestMethod]
    [DataRow(0, 60)]   // Next run is supposed to happen NOW! Probably the system doing work.
    [DataRow(-15, 60)]   // Next run time is 15 minutes in the past!  System was down.
    [DataRow(-30, 60)] // Next run time is 30 minutes in the past!  System was down.
    [DataRow(-60, 60)] // Next run time is 60 minutes in the past!  System was down.
    [DataRow(-75, 60)] // Next run time is 75 minutes in the past!  System was down.
    [DataRow(-1440, 60)] // Next run time is a day in the past!  System was down.
    [DataRow(29, 60)]  // Next run is not supposed to happen for another 29 minutes.  Manually triggered!
    [DataRow(30, 60)]  // Next run is not supposed to happen for another 30 minutes.  Manually triggered!
    [DataRow(31, 60)]  // Next run is not supposed to happen for another 31 minutes.  Manually triggered!
    [DataRow(60, 60)]  // Next run is not supposed to happen for another 60 minutes.  Manually triggered!
    [DataRow(75, 60)]  // Next run is not supposed to happen for another 75 minutes.  Manually triggered!
    public void ComputeNextIndexerRun_WhenAddIntervalToNextRunTimeIsFalse_WeAlwaysJustAddTheIntervalToTheCurrentTime(
        int nextRunAdjustment, int runTimeIntervalInMinutes)
    {
        // Arrange
        var classUnderTest = new WorkerConfigurationService();
        var config = new WorkerConfiguration()
        {
            Id = Guid.NewGuid(),
            AddIntervalToNextRunTime = false, // <------------------------- when FALSE!
            LastRunTime = DateTime.UtcNow.AddMinutes(-12),
            NextRunTime = DateTime.UtcNow.AddMinutes(-1),
            LastHeartbeatTime = null,
            RunTimeIntervalInMinutes = runTimeIntervalInMinutes
        };
        

        var startTime = DateTime.UtcNow.AddMinutes(nextRunAdjustment);
        config.NextRunTime = startTime;

        var expectedRunDateTime = DateTime.UtcNow.AddMinutes(runTimeIntervalInMinutes);

        // Act
        var actualDate = classUnderTest.ComputeNextWorkerRun(config);

        // Assert
        var timeSpanDifference = expectedRunDateTime - actualDate;
        var totalMinutesDifference = Math.Abs(timeSpanDifference.TotalMinutes);
        Assert.IsTrue(totalMinutesDifference < 2,
            $"Expected difference to be less than 2 minutes, but there were {totalMinutesDifference} total minutes difference!  " +
            $"Given that this method is called AFTER processing data and the current time is {DateTime.UtcNow}, " +
            $"we always expect the NextRunTime to increment by the interval of {runTimeIntervalInMinutes}; thus, " +
            $"we expect the NextRunTime to be {expectedRunDateTime}!");
    }

    /// <summary>
    /// When AddIntervalToNextRunTime is true, we should be adding the RunTimeIntervalInMinutes
    /// to the NextRunTime. So, if you have set it to 1440 (24 hours), the timer would run at
    /// the same time every day.
    ///
    /// Note: Keep in mind that we JUST ran the indexer and now we are looking at the time that
    ///       the Next run should happen. If the time interval is still in the future, someone
    /// manually triggered it. If the time interval is way in the past, the service was down and
    /// is catching back up!
    /// </summary>
    /// <param name="nextRunAdjustment">
    /// The number of minutes to add to the NextRunTime. For example, if it is zero, it is set
    /// to DateTime.UtcNow. If it where 15, we would add 15 minutes to DateTime.UtcNow.
    /// </param>
    /// <param name="expectedNextRunAdjustment">
    /// The expected DateTime. We will add this many minutes to NextRunTime. For example, if we
    /// think that the a 60 interval will have to be incremented twice, we would add 120 minutes.
    /// </param>
    /// <param name="runTimeIntervalInMinutes">
    /// The run time interval to add after completing the work.
    /// </param>
    [DataTestMethod]
    [DataRow(0, 60, 60)]     // Next run is supposed to happen NOW! Probably the system doing work.
    [DataRow(-15, 60, 60)]   // Next run time is 15 minutes in the past!  System was down.
    [DataRow(-29, 60, 60)]   // Next run time is 29 minutes in the past!  System was down.
    [DataRow(-30, 120, 60)]  // Next run time is 30 minutes in the past!  System was down.
    [DataRow(-31, 120, 60)]  // Next run time is 31 minutes in the past!  System was down.
    [DataRow(-60, 120, 60)]  // Next run time is 60 minutes in the past!  System was down.
    [DataRow(-75, 120, 60)]  // Next run time is 75 minutes in the past!  System was down.
    [DataRow(-1440, 1500, 60)] // Next run time is a day in the past!  System was down.
    [DataRow(15, 60, 60)]    // Next run is not supposed to happen for another 15 minutes.  Manually triggered! Yes, move the interval forward!
    [DataRow(29, 60, 60)]    // Next run is not supposed to happen for another 29 minutes.  Manually triggered! Yes, move the interval forward!
    [DataRow(30, 60, 60)]    // Next run is not supposed to happen for another 30 minutes.  Manually triggered! Yes, move the interval forward!
    [DataRow(31, 0, 60)]     // Next run is not supposed to happen for another 31 minutes.  Manually triggered! No, do NOT move the interval forward. We are less than 1/2 away, so fire it again in 29 minutes.
    [DataRow(60, 0, 60)]     // Next run is not supposed to happen for another 60 minutes.  Manually triggered! No, do NOT move the interval forward. We are 100% away, so fire it again in 60 minutes.
    [DataRow(75, 0, 60)]     // Next run is not supposed to happen for another 75 minutes.  Manually triggered! No, do NOT move the interval forward. We are more than 100% away, so fire it again in 75 minutes.
    public void ComputeNextIndexerRun_WhenAddIntervalToNextRunTimeIsTrue_TimeIsAddedToTheNextRunTimeRatherThanUsingTheCurrentTime(
        int nextRunAdjustment, int expectedNextRunAdjustment, int runTimeIntervalInMinutes)
    {
        // Arrange
        var classUnderTest = new WorkerConfigurationService();
        var config = new WorkerConfiguration()
        {
            Id = Guid.NewGuid(),
            AddIntervalToNextRunTime = true,  // <------------------------- when TRUE!
            LastRunTime = DateTime.UtcNow.AddMinutes(-12),
            NextRunTime = DateTime.UtcNow.AddMinutes(-1),
            LastHeartbeatTime = null,
            RunTimeIntervalInMinutes = runTimeIntervalInMinutes
        };

        var startTime = DateTime.UtcNow.AddMinutes(nextRunAdjustment);
        config.NextRunTime = startTime; // new DateTime(startTime.Ticks);

        var expectedRunDateTime = config.NextRunTime.AddMinutes(expectedNextRunAdjustment);

        // Act
        var actualDate = classUnderTest.ComputeNextWorkerRun(config);

        // Assert
        var timeSpanDifference = expectedRunDateTime - actualDate;
        var totalMinutesDifference = Math.Abs(timeSpanDifference.TotalMinutes);
        Assert.IsTrue(totalMinutesDifference < 2,
            $"Expected difference to be less than 2 minutes, but there were {totalMinutesDifference} total minutes difference!  " +
            $"Given that this method is called AFTER processing data, the current time is {DateTime.UtcNow} " +
            $"and the NextRunTime starts at {startTime} with an interval of {runTimeIntervalInMinutes}, we expect " +
            $"the NextRunTime to be {expectedRunDateTime}!");
    }
     
}