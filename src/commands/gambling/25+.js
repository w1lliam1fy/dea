const patron = require('patron.js');
const gambling = require('../../services/GamblingService.js');

class TwentyOnePlus extends patron.Command {
  constructor() {
    super({
      name: '25+',
      group: 'gambling',
      description: 'Roll 25.00 or higher on a 100.00 sided die to win 0.2X your bet.',
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
    return gambling.gamble(context, args.bet, 25, 0.2);
  }
}

module.exports = new TwentyOnePlus();
