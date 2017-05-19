using DEA.Common.Data;
using DEA.Common.Extensions;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Common.Preconditions;
using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DEA.Modules.Gangs
{
    public partial class Gangs
    {
        [Command("JoinGang")]
        [Require(Attributes.NoGang)]
        [Summary("Sends a request to join a gang.")]
        public async Task AddToGang([Summary("SLAM EM BOYS")] [Remainder] string gangName)
        {
            var gang = await _gangRepo.GetGangAsync(gangName, Context.Guild.Id);
            if (gang.Members.Length == 4)
            {
                ReplyError("This gang is already full!");
            }

            var leader = await (Context.Guild as IGuild).GetUserAsync(gang.LeaderId);

            if (leader != null)
            {
                var leaderDM = await leader.CreateDMChannelAsync();

                var key = Config.RAND.Next();
                await leaderDM.SendAsync($"{Context.User.Boldify()} has requested to join your gang. Please respond with \"{key}\" within 5 minutes to add this user to your gang.");

                await ReplyAsync($"The leader of {gang.Name} has been informed of your request to join their gang.");

                var answer = await _interactiveService.WaitForMessage(leaderDM, x => x.Content == key.ToString(), TimeSpan.FromMinutes(5));
                if (answer != null)
                {
                    if (await _gangRepo.InGangAsync(Context.GUser))
                    {
                        await leaderDM.SendAsync("This user has already joined a different gang.");
                    }
                    else if ((await _gangRepo.GetGangAsync(leader)).Members.Length == 4)
                    {
                        await leaderDM.SendAsync("Your gang is already full.");
                    }
                    else
                    {
                        await _gangRepo.AddMemberAsync(gang, Context.User.Id);

                        await leaderDM.SendAsync($"You have successfully added {Context.User.Boldify()} as a member of your gang.");

                        await Context.User.Id.DMAsync(Context.Client, $"Congrats! {leader} has accepted your request to join {gang.Name}!");
                    }
                }
            }
            else
            {
                await ReplyAsync("The leader of that gang is no longer in this server. ***RIP GANG ROFL***");
            }
        }
    }
}