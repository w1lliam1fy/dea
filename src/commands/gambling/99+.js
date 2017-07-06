const patron = require('patron');
const gambling = require('../../services/GamblingService.js');

class NinetyNinePlus extends patron.Command {
  constructor() {
    super({
      name: '99+',
      group: 'gambling',
      description: 'Roll 99.00 or higher on a 100.00 sided die to win 90X your bet.',
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
    return gambling.gamble(context, args.bet, 99, 90);
  }
}

module.exports = new NinetyNinePlus();
