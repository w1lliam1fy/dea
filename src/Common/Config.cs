using Discord;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

public static class Config
{
    public static readonly string DEFAULT_PREFIX = "$";

    public static readonly Regex ALPHANUMERICAL = new Regex(@"^[a-zA-Z0-9\s]*$");

    public static readonly CultureInfo CI = new CultureInfo("en-CA");

    public static readonly Color[] COLORS = { new Color(255, 38, 154), new Color(0, 255, 0), new Color(0, 232, 40), new Color(8, 248, 255),
        new Color(242, 38, 255), new Color(255, 28, 142), new Color(104, 255, 34), new Color(255, 190, 17), new Color(41, 84, 255),
        new Color(150, 36, 237), new Color(168, 237, 0)};

    public static readonly TimeSpan WHORE_COOLDOWN = TimeSpan.FromHours(2), JUMP_COOLDOWN = TimeSpan.FromHours(4), 
        STEAL_COOLDOWN = TimeSpan.FromHours(6), ROB_COOLDOWN = TimeSpan.FromHours(8), WITHDRAW_COOLDOWN = TimeSpan.FromHours(4), 
        RAID_COOLDOWN = TimeSpan.FromHours(4), LINE_COOLDOWN = TimeSpan.FromSeconds(25);

    public static readonly int MIN_CHAR_LENGTH = 7, LEADERBOARD_CAP = 10, RATELB_CAP = 10, WHORE_ODDS = 90, JUMP_ODDS = 85, STEAL_ODDS = 80,
        MIN_CHILL = 5, MAX_CHILL = (int)TimeSpan.FromHours(1).TotalSeconds, MIN_CLEAR = 2, MAX_CLEAR = 1000, GANG_NAME_CHAR_LIMIT = 24,
        GANGSLB_CAP = 10, MIN_ROB_ODDS = 50, MAX_ROB_ODDS = 75, DEA_CUT = 10, TEMP_MULTIPLIER_RESET_COOLDOWN =
        (int)TimeSpan.FromHours(1).TotalMilliseconds, INTEREST_RATE_COOLDOWN = (int)TimeSpan.FromHours(1).TotalMilliseconds,
        AUTO_UNMUTE_COOLDOWN = (int)TimeSpan.FromMinutes(5).TotalMilliseconds, RAID_SUCCESS_ODDS = 65;

    public static readonly double DEFAULT_MESSAGE_COOLDOWN = TimeSpan.FromSeconds(30).TotalMilliseconds;

    public static readonly decimal LINE_COST = 250, POUND_COST = 1000, KILO_COST = 2500, POUND_MULTIPLIER = 2, KILO_MULTIPLIER = 4,
        RESET_REWARD = 10000, MAX_WHORE = 100, MIN_WHORE = 50, WHORE_FINE = 200, MAX_JUMP = 250, JUMP_FINE = 500, MIN_JUMP = 100,
        MAX_STEAL = 500, MIN_STEAL = 250, STEAL_FINE = 1000, MAX_RESOURCES = 1000, MIN_RESOURCES = 25, DONATE_MIN = 5, BET_MIN = 5,
        GANG_CREATION_COST = 2500, GANG_NAME_CHANGE_COST = 500, WITHDRAW_CAP = 0.20m, MIN_WITHDRAW = 50, MIN_DEPOSIT = 50,
        JUMP_REQUIREMENT = 500, STEAL_REQUIREMENT = 2500, ROB_REQUIREMENT = 5000, BULLY_REQUIREMENT = 10000, FIFTYX2_REQUIREMENT = 25000;

    public static readonly string[] BANKS = { "Bank of America", "Wells Fargo Bank", "JPMorgan Chase Bank", "Capital One Bank",
        "RBC Bank", "USAA Bank", "Union Bank", "Morgan Stanley Bank" }, STORES = { "7-Eleven", "Speedway", "Couche-Tard", "QuikTrip",
        "Kroger", "Circle K" };
}
