using DEA.Database.Models;
using DEA.Database.Repository;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Services.Timers
{
    class AutoTrivia
    {
        private Timer _timer;
        IDependencyMap _map;
        IMongoCollection<Guild> _guilds;
        DiscordSocketClient _client;
        UserRepository _userRepo;
        ResponseService _responseService;
        InteractiveService _interactiveService;

        public AutoTrivia(IDependencyMap map)
        {
            _map = map;
            _guilds = _map.Get<IMongoCollection<Guild>>();
            _userRepo = map.Get<UserRepository>();
            _client = map.Get<DiscordSocketClient>();
            _interactiveService = map.Get<InteractiveService>();
            _responseService = map.Get<ResponseService>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(Trivia);

            _timer = new Timer(TimerDelegate, StateObj, 0, Config.AUTO_TRIVIA_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private void Trivia(object stateObj)
        {
            Task.Run(async () =>
            {
                var builder = Builders<Guild>.Filter;
                foreach (var dbGuild in await (await _guilds.FindAsync(builder.Empty)).ToListAsync())
                {
                    if (dbGuild.AutoTrivia)
                    {
                        var guild = _client.GetGuild(dbGuild.Id);
                        var defaultChannel = guild.DefaultChannel;
                        if (guild != null)
                        {
                            int roll = new Random().Next(0, dbGuild.Trivia.ElementCount);
                            var element = dbGuild.Trivia.GetElement(roll);
                            await _responseService.Send(defaultChannel, "__**TRIVIA:**__ " + element.Name);
                            var answer = await _interactiveService.WaitForMessage(defaultChannel, y => y.Content.ToLower() == element.Value.AsString.ToLower());
                            if (answer != null)
                            {
                                var user = answer.Author as IGuildUser;
                                await _userRepo.EditCashAsync(user, dbGuild, await _userRepo.FetchUserAsync(user), Config.TRIVIA_PAYOUT);
                                await _responseService.Send(defaultChannel, $"{await _responseService.NameAsync(user, await _userRepo.FetchUserAsync(user))}, Congrats! You just " +
                                           $"won {Config.TRIVIA_PAYOUT.ToString("C", Config.CI)} for correctly answering \"{element.Value.AsString}\"");
                            }
                            else
                            {
                                await _responseService.Send(defaultChannel, $"NOBODY got the right answer for the trivia question! Alright, I'll sauce it to you guys, but next time " +
                                           $"you are on your own. The right answer is: \"{element.Value.AsString}\"");
                            }
                        }
                    }
                }
            });
        }
    }
}