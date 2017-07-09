const patron = require('patron.js');
const util = require('../../utility');
const config = require('../../config.json');

class Help extends patron.Command {
  constructor() {
    super({
      name: 'help',
      aliases: ['commands', 'command', 'cmd', 'cmds', 'support', 'docs'],
      group: 'system',
      description: 'All command information.',
      guildOnly: false,
      args: [
        new patron.Argument({
          name: 'command',
          key: 'command',
          type: 'string',
          default: '',
          example: 'money'
        })
      ]
    });
  }

  async run(msg, args) {
    if (util.StringUtil.isNullOrWhiteSpace(args.command)) {
      await util.Messenger.DM(msg.author,
        'DEA is **THE** cleanest bot around, and you can have it in **YOUR** server simply by clicking here: ' + config.inviteLink + '.\n\n' +
        'For all information about command usage and setup on your Discord Sever, view the official documentation: ' + config.documentationLink + '.\n\n' +
        'The `' + config.prefix +  'help <command>` command may be used for view the usage and an example of any command.\n\n' + 
        'If you have **ANY** questions, you may join the **Official DEA Discord Server:** ' + config.serverInviteLink + ' for instant support along with a great community.');

      if (msg.channel.type !== 'dm') {
        return util.Messenger.reply(msg.channel, msg.author, 'You have been DMed with all the command information!');
      }
    } else {
      args.command = args.command.startsWith(config.prefix) ? args.command.slice(config.prefix.length) : args.command;

      const lowerInput = args.command.toLowerCase();

      let command = msg.client.registry.commands.get(lowerInput);
		
      if (command === undefined) {
        const matches = msg.client.registry.commands.filterArray((value) => value.aliases.some((v) => v === lowerInput));
        
        if (matches.length > 0) {
          command = matches[0];
        } else {
          return util.Messenger.replyError(msg.channel, msg.author, 'This command does not exist.');
        }
      }

      return util.Messenger.send(msg.channel, '**Description:** ' + command.description + '\n**Usage:** `' + config.prefix + command.getUsage() + '`\n**Example:** `' + 
        config.prefix + command.getExample() + '`', util.StringUtil.upperFirstChar(command.name));
    }  
  }
}

module.exports = new Help();
