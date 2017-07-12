const util = require('../utility');
const db = require('../database');
const config = require('../config.json');

module.exports = (client) => {
  client.on('guildCreate', async (guild) => {
    return util.Messenger.trySend(guild.defaultChannel, 'Hey! I am DEA, ya know, the slickest bot in town.\n\nJust wanted to let you know that you can use the `$help` command to get all the command info a man\'s heart could desire.\n\nAlso, you can setup automatic welcome messages with the `' + config.prefix + 'welcome <message>` command. Unsourceable studies have shown that new users are more likely to stay if they are instantly welcomed!\n\nIf you don\'t like it, no problem! You can disable it at any time with the `' + config.prefix + 'disablewelcome` command. If you have any questions or concerns, you may always join the **Official DEA Support Server:** ' + config.serverInviteLink);
  });
};
