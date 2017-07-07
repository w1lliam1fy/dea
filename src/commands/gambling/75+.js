const patron = require('patron.js');
const gambling = require('../../services/GamblingService.js');

class SeventyFivePlus extends patron.Command {
  constructor() {
    super({
      name: '75+',
      group: 'gambling',
      description: 'Roll 75.00 or higher on a 100.00 sided die to win 2.6X your bet.',
      args: [
        new patron.Argument({
          name: 'bet',
          key: 'bet',
          type: 'float',
          example: '500'
        })
      ]
    });
  }

  async run(context, args) {
    return gambling.gamble(context, args.bet, 75, 2.6);
  }
}

module.exports = new SeventyFivePlus();
