const patron = require('patron.js');
const db = require('../database');
const ModerationService = require('../services/ModerationService.js');

class Moderator extends patron.Precondition {
  async run(command, context) {
    const dbGuild = await db.guildRepo.getGuild(context.guild.id);

    if (ModerationService.getPermLevel(dbGuild, context.guild.member(context.author)) >= 1) {
      return patron.PreconditionResult.fromSuccess();
    }

    return patron.PreconditionResult.fromError(command, 'You must be a moderator in order to use this command.');
  }
}

module.exports = new Moderator();
