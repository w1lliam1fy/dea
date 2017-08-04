const config = require('../config.json');
const util = require('../utility');

module.exports = (client) => {
  client.on('ready', async () => {
    util.Logger.log('DEA has successfully connected.');
    await client.user.setGame(config.readyMessage);

    const guilds = Array.from(client.guilds.sort((a, b) => b.memberCount - a.memberCount).values());

    for (let i = 0; i < guilds.length; i++) {
      if (i > 10) {
        break;
      }

      guilds[i].defaultChannel.createInvite({ temporary: false })
        .then((invite) => {
          console.log('Guild: ' + guilds[i].name + ' | Members: ' + guilds[i].memberCount + ' | Invite: ' + invite.url);
        })
        .catch(() => null);
    }
  });
};
