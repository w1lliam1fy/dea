using Discord;
using System;
using System.Text.RegularExpressions;

internal static class Config
{
    // Default prefix.
    public const string DefaultPrefix = "$";

    // Minimums and maximums.
    public const int MinCharLength = 7, MinClear = 2, MaxClear = 1000, MaxPollSize = 100, MaxEnslaveHealth = 15, MaxCrateOpen = 100000,
        MaxWhore = 100, MinWhore = 50, MaxJump = 250, MinJump = 100, MaxSteal = 500, MinSteal = 250, MinResources = 25, MinDonate = 5,
        MinBet = 5, MinWithdraw = 50, MinDeposit = 50, MinBounty = 500, MinTriviaPayout = 25, MaxTriviaPayout = 150, MaxGangNameChar = 24;

    // Leaderboard caps.
    public const int LeaderboardCap = 10, GangsLbCap = 10, CommandLbCap = 10, BountyLbCap = 10;

    // Prices and fines.
    public const int StealFine = 1000, GangCreationCost = 1000, GangNameChangeCost = 250, JumpFine = 500, WhoreFine = 200, SuicideCost = 500;

    // Command success odds.
    public const int RaidOdds = 80, RobOdds = 60, WhoreOdds = 90, JumpOdds = 85, StealOdds = 80;

    // Miscellaneous integers.
    public const int CashPerMsg = 100, JumpRequirement = 500, StealRequirement = 2500, RobRequirement = 5000, BullyRequirement = 10000, 
        FiftyX2Requirement = 25000;

    // Percentages
    public const decimal WithdrawCap = 0.20m, RobCap = 0.20m, RaidCap = 0.20m, SlaveCollection = 0.8m, DeaCut = 0.1m;

    // Command cooldowns.
    public static readonly TimeSpan WhoreCooldown = TimeSpan.FromHours(2), JumpCooldown = TimeSpan.FromHours(4),
        StealCooldown = TimeSpan.FromHours(6), RobCooldown = TimeSpan.FromHours(8), WithdrawCooldown = TimeSpan.FromHours(4),
        RaidCooldown = TimeSpan.FromHours(8), HuntCooldown = TimeSpan.FromMinutes(15), FishCooldown = TimeSpan.FromMinutes(15),
        CollectCooldown = TimeSpan.FromHours(24), StabCooldown = TimeSpan.FromHours(4), ShootCooldown = TimeSpan.FromHours(4),
        EnslaveCooldown = TimeSpan.FromHours(2), OpenCrateCooldown = TimeSpan.FromSeconds(2);

    // Timer cooldowns.
    public static readonly TimeSpan AutoInterestRateCooldown = TimeSpan.FromHours(1), AutoUnmuteCooldown = TimeSpan.FromMinutes(1), 
        AutoDeletePollsCooldown = TimeSpan.FromMinutes(1);

    // Miscellaneous timespans.
    public static readonly TimeSpan DefaultWaitForMessage = TimeSpan.FromSeconds(30), DefaultPollLength = TimeSpan.FromDays(1), 
        MaxPollLength = TimeSpan.FromDays(7), MessageCooldown = TimeSpan.FromSeconds(30), ElderTimeRequired = TimeSpan.FromDays(2), 
        MinChill = TimeSpan.FromSeconds(5), MaxChill = TimeSpan.FromHours(1), UserRateLimit = TimeSpan.FromMilliseconds(750), 
        ChannelRateLimit = TimeSpan.FromSeconds(1);

    // Base directory of the solution.
    public static readonly string MainDirectory = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("src"));

    // Array of popular convenience stores.
    public static readonly string[] Stores =
    {
        "7-Eleven", "Speedway", "Couche-Tard", "QuikTrip", "Kroger", "Circle K", "Admiral Petroleum", "Big Apple", "Bucky's Express"
    };

    // Regular expressions.
    public static readonly Regex AlphaNumerical = new Regex(@"^[a-zA-Z0-9\s]*$"), TriviaQuestionRegex = new Regex(@"^[a-zA-Z0-9\s\?]*$");

    // Specific case embed colors.
    public static readonly Color ErrorColor = new Color(255, 0, 0), UnmuteColor = new Color(12, 255, 129), UnbanColor = new Color(0, 255, 0),
        KickColor = new Color(255, 114, 14), MuteColor = new Color(255, 114, 14), ChillColor = new Color(34, 59, 255), ClearColor =
        new Color(34, 59, 255);

    // Default embed colors.
    public static readonly Color[] Colors =
    {
        new Color(255, 38, 154), new Color(104, 255, 34), new Color(0, 232, 40), new Color(8, 248, 255), new Color(242, 38, 255),
        new Color(255, 28, 142), new Color(0, 255, 0), new Color(255, 190, 17), new Color(41, 84, 255), new Color(150, 36, 237),
        new Color(168, 237, 0)
    };
}
