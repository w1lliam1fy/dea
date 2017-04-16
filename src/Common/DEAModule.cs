using DEA.Database.Repository;
using DEA.Services;
using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Common
{
    public abstract class DEAModule : ModuleBase<DEAContext>
    {

        public async Task<IUserMessage> WaitForMessage(IMessageChannel channel, string contentFilter, TimeSpan? timeout = null, IUser user = null)
        {
            if (timeout == null) timeout = Config.DEFAULT_WAITFORMESSAGE;

            var blockToken = new CancellationTokenSource();
            IUserMessage response = null;

            Func<IMessage, Task> isValid = (messageParameter) =>
            {
                var message = messageParameter as IUserMessage;
                if (message == null) return Task.CompletedTask;
                if (LevenshteinDistance.Compute(message.Content, contentFilter) > 3) return Task.CompletedTask;
                if (user != null && message.Author.Id != user.Id) return Task.CompletedTask;
                if (message.Channel.Id != channel.Id) return Task.CompletedTask;
                var context = new ResponseContext(DEABot.Client, message);

                response = message;
                blockToken.Cancel(true);
                return Task.CompletedTask;
            };

            DEABot.Client.MessageReceived += isValid;
            try
            {
                if (timeout == TimeSpan.Zero)
                    await Task.Delay(-1, blockToken.Token);
                else
                    await Task.Delay(timeout.Value, blockToken.Token);
            }
            catch (TaskCanceledException)
            {
                return response;
            }
            catch
            {
                throw;
            }
            finally
            {
                DEABot.Client.MessageReceived -= isValid;
            }
            return null;
        }

        public Task<IUserMessage> Reply( string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = $"{ResponseMethods.Name(Context.User as IGuildUser, Context.DbUser)}, {description}",
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            return ReplyAsync(string.Empty, embed: builder);
        }

        public Task<IUserMessage> Send(string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = description,
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            return ReplyAsync(string.Empty, embed: builder);
        }

        public Task<string> NameAsync()
        {
            var user = Context.User as IGuildUser;
            if (string.IsNullOrWhiteSpace(Context.DbUser.Name))
                return Task.FromResult((string.IsNullOrWhiteSpace(user.Nickname)) ? $"**{user.Username}**" : $"**{user.Nickname}**");
            else
                return Task.FromResult($"**{Context.DbUser.Name}**");
        }

        public Task<string> TitleNameAsync()
        {
            var user = Context.User as IGuildUser;
            if (string.IsNullOrWhiteSpace(Context.DbUser.Name))
                return Task.FromResult((string.IsNullOrWhiteSpace(user.Nickname)) ? user.Username : user.Nickname);
            else
                return Task.FromResult(Context.DbUser.Name);
        }

        public static Task<bool> IsModAsync(DEAContext context, IGuildUser user)
        {
            if (user.GuildPermissions.Administrator) return Task.FromResult(true);
            if (context.DbGuild.ModRoles.ElementCount != 0)
                foreach (var role in context.DbGuild.ModRoles)
                    if (user.Guild.GetRole(Convert.ToUInt64(role.Name)) != null)
                        if (user.RoleIds.Any(x => x.ToString() == role.Name)) return Task.FromResult(true);
            return Task.FromResult(false);
        }

        public static Task<bool> IsHigherModAsync(DEAContext context, IGuildUser mod, IGuildUser user)
        {
            int highest = mod.GuildPermissions.Administrator ? 2 : 0;
            int highestForUser = user.GuildPermissions.Administrator ? 2 : 0;
            if (context.DbGuild.ModRoles.ElementCount == 0) return Task.FromResult(highest > highestForUser);

            foreach (var role in context.DbGuild.ModRoles.OrderBy(x => x.Value))
                if (mod.Guild.GetRole(Convert.ToUInt64(role.Name)) != null)
                    if (mod.RoleIds.Any(x => x.ToString() == role.Name)) highest = role.Value.AsInt32;

            foreach (var role in context.DbGuild.ModRoles.OrderBy(x => x.Value))
                if (user.Guild.GetRole(Convert.ToUInt64(role.Name)) != null)
                    if (user.RoleIds.Any(x => x.ToString() == role.Name)) highestForUser = role.Value.AsInt32;

            return Task.FromResult(highest > highestForUser);
        }

        public async Task InformSubjectAsync(IUser moderator, string action, IUser subject, string reason)
        {
            try
            {
                var channel = await subject.CreateDMChannelAsync();
                if (reason == "No reason.")
                    await ResponseMethods.DM(channel, $"{moderator} has attempted to {action.ToLower()} you.");
                else
                    await ResponseMethods.DM(channel, $"{moderator} has attempted to {action.ToLower()} you for the following reason: \"{reason}\"");
            }
            catch { }
        }

        public async Task GambleAsync(decimal bet, decimal odds, decimal payoutMultiplier)
        {
            if (Context.Guild.GetTextChannel(Context.DbGuild.GambleId) != null && Context.Channel.Id != Context.DbGuild.GambleId)
                await ErrorAsync($"You may only gamble in {Context.Guild.GetTextChannel(Context.DbGuild.GambleId).Mention}!");
            if (bet < Config.BET_MIN) await ErrorAsync($"Lowest bet is {Config.BET_MIN}$.");
            if (bet > Context.DbUser.Cash) await ErrorAsync($"You do not have enough money. Balance: {Context.DbUser.Cash.ToString("C", Config.CI)}.");
            decimal roll = new Random().Next(1, 10001) / 100m;
            if (roll >= odds)
            {
                await UserRepository.EditCashAsync(Context, (bet * payoutMultiplier));
                await ResponseMethods.Reply(Context, $"You rolled: {roll.ToString("N2")}. Congrats, you won " +
                                                     $"{(bet * payoutMultiplier).ToString("C", Config.CI)}! Balance: {(Context.DbUser.Cash + (bet * payoutMultiplier)).ToString("C", Config.CI)}.");
            }
            else
            {
                await UserRepository.EditCashAsync(Context, -bet);
                await ResponseMethods.Reply(Context, $"You rolled: {roll.ToString("N2")}. Unfortunately, you lost " +
                                                     $"{bet.ToString("C", Config.CI)}. Balance: {(Context.DbUser.Cash - bet).ToString("C", Config.CI)}.");
            }
        }

        public Task ErrorAsync(string message)
        {
            throw new DEAException(message);
        }
    }
}
