const patron = require('patron.js');
const ModerationService = require('../services/ModerationService.js');

class Moderator extends patron.Precondition {
  async run(command, msg) {
    if (ModerationService.getPermLevel(msg.dbGuild, msg.guild.member(msg.author)) >= 1) {
      return patron.PreconditionResult.fromSuccess();
    }

    return patron.PreconditionResult.fromError(command, 'You must be a moderator in order to use this command.');
  }
}

module.exports = new Moderator();
