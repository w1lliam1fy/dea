const patron = require('patron.js');
const BotOwner = require('../preconditions/BotOwner.js');

class BotOwners extends patron.Group {
  constructor() {
    super({ 
      name: 'botowners',
      description: 'These commands may only be used by the owners of DEA.',
      preconditions: [BotOwner] 
    });
  }
}

module.exports = new BotOwners();
