const patron = require('patron');
const util = require('../../utility');

class Help extends patron.Command {
  constructor() {
    super({
      name: 'help',
      aliases: ['info', 'information'],
      group: 'system',
      description: 'Information about the recent lack of commands.',
      guildOnly: false
    });
  }

  async run(context) {
    return util.Messenger.reply(context.channel, context.author, 
      'DEA is currently receiving a MASSIVE revamp of its functionality with a completely *NEW* framework. Since everything is being rewritten from ' + 
      'ground up, nearly 99% of all commands will not exist, and they will all gradually be reimplemented as time goes by. Thank you for your patience!');
  }
}

module.exports = new Help();
